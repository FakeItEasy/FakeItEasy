namespace FakeItEasy.Tests.Core
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using FakeItEasy.Core;
    using FluentAssertions;
    using NUnit.Framework;
    using TestHelpers;

    [TestFixture]
    public class ArgumentConstraintTrapTests
    {
        private readonly ArgumentConstraintTrap trap = new ArgumentConstraintTrap();

        [Test]
        public void Should_return_constraints_that_has_been_trapped()
        {
            // Arrange
            var constraint1 = A.Dummy<IArgumentConstraint>();
            var constraint2 = A.Dummy<IArgumentConstraint>();

            // Act
            var result = this.trap.TrapConstraints(() =>
                                                       {
                                                           ArgumentConstraintTrap.ReportTrappedConstraint(constraint1);
                                                           ArgumentConstraintTrap.ReportTrappedConstraint(constraint2);
                                                       });

            // Assert
            result.Should().BeEquivalentTo(constraint1, constraint2);
        }

        [Test]
        public void Should_not_return_constraints_from_previous_call()
        {
            // Arrange
            var constraint1 = A.Dummy<IArgumentConstraint>();
            var constraint2 = A.Dummy<IArgumentConstraint>();

            this.trap.TrapConstraints(() => ArgumentConstraintTrap.ReportTrappedConstraint(constraint1));

            // Act
            var result = this.trap.TrapConstraints(() =>
                                                       {
                                                           ArgumentConstraintTrap.ReportTrappedConstraint(constraint2);
                                                       });

            // Assert
            result.Should().BeEquivalentTo(constraint2);
        }

        [Test]
        public void Should_not_fail_when_reporting_trapped_constraint_outside_call_to_trap_constraints()
        {
            // Act
            var exception = Record.Exception(
                () => ArgumentConstraintTrap.ReportTrappedConstraint(A.Dummy<IArgumentConstraint>()));

            // Assert
            exception.Should().BeNull();
        }

        [Test]
        public void Should_track_constraints_supplied_in_calls_made_from_overlapping_threads()
        {
            // Ensures that constraints are properly trapped even when two constraint-trapping threads
            // overlap. Uses the reset events to ensure that the threads consistently execute like this:
            // |-----------------------------|
            //       |---------------|
            // The thread that starts first will register to trap constraints before the second thread
            // but will not actually report the constraint until after the second thread has reported
            // its constraint and finished.
            // Without per-thread constraint trapping, this would mean that the first thread's constraint
            // would be lost.
            
            // Arrange
            var lateStartingLock = new ManualResetEventSlim(false);
            var lateEndingLock = new ManualResetEventSlim(false);

            var earlyStartingConstraint = A.Fake<IArgumentConstraint>();
            A.CallTo(() => earlyStartingConstraint.ToString()).Returns("earlyStarter");
            IArgumentConstraint earlyStartingResult = null;

            var lateStartingConstraint = A.Fake<IArgumentConstraint>();
            A.CallTo(() => lateStartingConstraint.ToString()).Returns("lateStarter");
            IArgumentConstraint lateStartingResult = null;

            // Act
            var earlyStartingTask = Task.Factory.StartNew(() =>
            {
                earlyStartingResult = this.trap.TrapConstraints(() =>
                {
                    lateStartingLock.Set();
                    lateEndingLock.Wait();

                    ArgumentConstraintTrap.ReportTrappedConstraint(earlyStartingConstraint);
                }).SingleOrDefault();
            });

            var lateStartingTask = Task.Factory.StartNew(() =>
            {
                lateStartingLock.Wait();
                
                lateStartingResult = this.trap.TrapConstraints(() =>
                {
                    ArgumentConstraintTrap.ReportTrappedConstraint(lateStartingConstraint);
                }).SingleOrDefault();
                
                lateEndingLock.Set();
            });

            Task.WaitAll(earlyStartingTask, lateStartingTask);

            // Assert
            new[] { earlyStartingResult, lateStartingResult }
                .Should().Equal(earlyStartingConstraint, lateStartingConstraint);
        }
    }
}