namespace FakeItEasy.Tests.ExpressionsConstraints
{
    using System;
    using System.Collections.Generic;
    using FakeItEasy.Expressions.ArgumentConstraints;
    using FluentAssertions;
    using Xunit;

    public class EqualityArgumentConstraintTests
        : ArgumentConstraintTestBase
    {
        public EqualityArgumentConstraintTests()
        {
            this.ConstraintField = new EqualityArgumentConstraint(1);
        }

        protected override string ExpectedDescription => "1";

        public static IEnumerable<object[]> InvalidValues()
        {
            return TestCases.FromObject(
                null,
                new object(),
                Guid.NewGuid(),
                "FOO",
                " foo ");
        }

        public static IEnumerable<object[]> ValidValues()
        {
            return TestCases.FromObject(
                1);
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
            this.ConstraintField.ToString().Should().Be("1");
        }

        [Fact]
        public void ToString_should_return_NULL_when_expected_value_is_null()
        {
            var validator = new EqualityArgumentConstraint(null);

            validator.ToString().Should().Be("<NULL>");
        }

        [Fact]
        public void ToString_should_put_accents_when_expected_value_is_string()
        {
            var validator = new EqualityArgumentConstraint("foo");

            validator.ToString().Should().Be("\"foo\"");
        }
    }
}
