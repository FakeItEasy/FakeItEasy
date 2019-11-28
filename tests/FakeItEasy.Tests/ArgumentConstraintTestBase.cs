namespace FakeItEasy.Tests
{
    using System;
    using FakeItEasy.Core;
    using FluentAssertions;
    using Xunit;

    public abstract class ArgumentConstraintTestBase
    {
        private IArgumentConstraint? constraint;

        internal IArgumentConstraint Constraint
        {
            get => this.constraint ?? throw new InvalidOperationException($"{nameof(this.Constraint)} isn't set");
            set => this.constraint = value;
        }

        protected abstract string ExpectedDescription { get; }

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
            var writer = ServiceLocator.Resolve<StringBuilderOutputWriter.Factory>().Invoke();

            this.Constraint.WriteDescription(writer);

            writer.Builder.ToString().Should().Be("<" + this.ExpectedDescription + ">");
        }
    }
}
