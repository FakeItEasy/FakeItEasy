using FakeItEasy.Core;
using NUnit.Framework;

namespace FakeItEasy.Tests.Core
{
    [TestFixture]
    public class ArgumentConstraintTrapTests
    {
        private ArgumentConstraintTrap trap = new ArgumentConstraintTrap();

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

            // Asssert
            Assert.That(result, Is.EquivalentTo(new[] { constraint1, constraint2 }));
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

            // Asssert
            Assert.That(result, Is.EquivalentTo(new[] { constraint2 }));
        }

        [Test]
        public void Should_not_fail_when_reporting_trapped_constraint_outside_call_to_trap_constraints()
        {
            // Arrange

            // Act, Assert
            Assert.DoesNotThrow(() => ArgumentConstraintTrap.ReportTrappedConstraint(A.Dummy<IArgumentConstraint>()));
        }
    }
}