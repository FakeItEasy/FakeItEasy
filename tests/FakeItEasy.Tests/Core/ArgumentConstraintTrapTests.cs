namespace FakeItEasy.Tests.Core
{
    using System.Threading;
    using System.Threading.Tasks;
    using FakeItEasy.Core;
    using FluentAssertions;
    using Xunit;

    public class ArgumentConstraintTrapTests
    {
        private readonly ArgumentConstraintTrap trap = new ArgumentConstraintTrap();

        [Fact]
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
            var earlyStartingTask = Task.Run(() =>
            {
                earlyStartingResult = this.trap.TrapConstraint(() =>
                {
                    lateStartingLock.Set();
                    lateEndingLock.Wait();

                    ArgumentConstraintTrap.ReportTrappedConstraint(earlyStartingConstraint);
                });
            });

            var lateStartingTask = Task.Run(() =>
            {
                lateStartingLock.Wait();

                lateStartingResult = this.trap.TrapConstraint(() =>
                {
                    ArgumentConstraintTrap.ReportTrappedConstraint(lateStartingConstraint);
                });

                lateEndingLock.Set();
            });

            Task.WaitAll(earlyStartingTask, lateStartingTask);

            // Assert
            new[] { earlyStartingResult, lateStartingResult }
                .Should().Equal(earlyStartingConstraint, lateStartingConstraint);
        }
    }
}
