namespace FakeItEasy.Tests
{
    using System.Reflection;
    using System.Text;
    using FakeItEasy.Core;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xunit;

    public abstract class ArgumentConstraintTestBase
    {
        internal IArgumentConstraint ConstraintField { get; set; }

        protected abstract string ExpectedDescription { get; }

        private IArgumentConstraint Constraint => this.ConstraintField;

        public virtual void IsValid_should_return_false_for_invalid_values(object invalidValue)
        {
            this.Constraint.IsValid(invalidValue).Should().BeFalse();
        }

        public virtual void IsValid_should_return_true_for_valid_values(object validValue)
        {
            var result = this.Constraint.IsValid(validValue);

            result.Should().BeTrue();
        }

        [Fact]
        public virtual void Constraint_should_provide_correct_description()
        {
            var output = new StringBuilder();

            this.Constraint.WriteDescription(new StringBuilderOutputWriter(output));

            output.ToString().Should().Be("<" + this.ExpectedDescription + ">");
        }
    }
}
