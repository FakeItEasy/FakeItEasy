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

        public override string ExpectedDescription
        {
            get { return "[\"foo\", \"bar\"]"; }
        }

        public override IEnumerable<object[]> InvalidValues()
        {
            return TestCases.FromObject(
                new object(),
                null,
                new[] { "one", "two" },
                new[] { "foo", "bar", "biz" });
        }

        public override IEnumerable<object[]> ValidValues()
        {
            return TestCases.FromObject(
                new[] { "foo", "bar" },
                new List<string>(new[] { "foo", "bar" }));
        }

        [Fact]
        public void Constraint_should_provide_correct_description()
        {
            var output = new StringBuilder();

            this.ConstraintField.WriteDescription(new StringBuilderOutputWriter(output));

            output.ToString().Should().Be(this.ExpectedDescription);
        }
    }
}
