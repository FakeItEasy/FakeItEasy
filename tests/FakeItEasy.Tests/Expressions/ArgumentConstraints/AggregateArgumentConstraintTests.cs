namespace FakeItEasy.Tests.Expressions.ArgumentConstraints
{
    using System.Collections.Generic;
    using System.Text;
    using FakeItEasy.Expressions.ArgumentConstraints;
    using FluentAssertions;
    using Xunit;

    public class AggregateArgumentConstraintTests
        : ArgumentConstraintTestBase
    {
        public AggregateArgumentConstraintTests()
        {
            this.ConstraintField = new AggregateArgumentConstraint(new[] { new EqualityArgumentConstraint("foo"), new EqualityArgumentConstraint("bar") });
        }

        public interface ITypeWithMethod
        {
            void Method(string firstArgument, params object[] args);
        }

        protected override string ExpectedDescription => "[\"foo\", \"bar\"]";

        public static IEnumerable<object[]> InvalidValues()
        {
            return TestCases.FromObject(
                new object(),
                null,
                new[] { "one", "two" },
                new[] { "foo", "bar", "biz" });
        }

        public static IEnumerable<object[]> ValidValues()
        {
            return TestCases.FromObject(
                new[] { "foo", "bar" },
                new List<string>(new[] { "foo", "bar" }));
        }

        [Theory]
        [MemberData(nameof(InvalidValues))]
        public override void IsValid_should_return_false_for_invalid_values(object invalidValue)
        {
            base.IsValid_should_return_false_for_invalid_values(invalidValue);
        }

        [Theory]
        [MemberData(nameof(ValidValues))]
        public override void IsValid_should_return_true_for_valid_values(object validValue)
        {
            base.IsValid_should_return_true_for_valid_values(validValue);
        }

        [Fact]
        public override void Constraint_should_provide_correct_description()
        {
            var output = new StringBuilder();

            this.ConstraintField.WriteDescription(new StringBuilderOutputWriter(output));

            output.ToString().Should().Be(this.ExpectedDescription);
        }
    }
}
