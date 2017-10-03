namespace FakeItEasy.Tests
{
    using FakeItEasy.Core;
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
            var writer = ServiceLocator.Current.Resolve<StringBuilderOutputWriter>();

            this.Constraint.WriteDescription(writer);

            writer.Builder.ToString().Should().Be("<" + this.ExpectedDescription + ">");
        }
    }
}
