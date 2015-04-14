namespace FakeItEasy.Tests.Core
{
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
        public void Should_track_constraints_supplied_in_calls_made_from_different_threads()
        {
            var exception = Record.Exception(() =>
            {
                var tasks = new Task[12];
                for (int i = 0; i < tasks.Length; i++)
                {
                    var taskNumber = i;
                    tasks[i] = Task.Factory.StartNew(() =>
                    {
                        var constraint = A.Fake<IArgumentConstraint>();
                        A.CallTo(() => constraint.ToString()).Returns("constraint " + taskNumber);

                        var result = this.trap.TrapConstraints(() =>
                        {
                            ArgumentConstraintTrap.ReportTrappedConstraint(constraint);
                        });

                        result.Should().BeEquivalentTo(constraint);
                    });
                }

                Task.WaitAll(tasks);
            });

            exception.Should().BeNull();
        }
    }
}