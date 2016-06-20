namespace FakeItEasy.Tests.Configuration
{
    using System;
    using FakeItEasy.Configuration;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xunit;

    public class AnyCallCallRuleTests
    {
        [Theory]
        [InlineData(typeof(int), true)]
        [InlineData(typeof(string), false)]
        [InlineData(null, true)]
        public void IsApplicableTo_should_check_the_ApplicableToMembersWithReturnType_property(Type type, bool expectedResult)
        {
            // Arrange
            var rule = this.CreateRule();
            rule.ApplicableToMembersWithReturnType = type;

            var call = ExpressionHelper.CreateFakeCall<IFoo>(x => x.Baz());

            // Act
            var result = rule.IsApplicableTo(call);

            // Assert
            result.Should().Be(expectedResult);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void IsApplicableTo_should_use_predicate_set_by_UsePredicateToValidateArguments(bool predicateReturnValue)
        {
            // Arrange
            Func<ArgumentCollection, bool> argumentsPredicate = x => predicateReturnValue;

            var call = ExpressionHelper.CreateFakeCall<IFoo>(x => x.Bar("a", "b"));

            var rule = this.CreateRule();
            rule.UsePredicateToValidateArguments(argumentsPredicate);

            // Act
            rule.IsApplicableTo(call);

            // Assert
            rule.IsApplicableTo(call).Should().Be(predicateReturnValue);
        }

        [Fact]
        public void ToString_when_no_member_type_is_specified_should_return_correct_description()
        {
            // Arrange
            var rule = this.CreateRule();

            // Act

            // Assert
            rule.DescriptionOfValidCall.Should().Be("Any call made to the fake object.");
        }

        [Fact]
        public void ToString_when_member_type_is_set_should_return_correct_description()
        {
            // Arrange
            var rule = this.CreateRule();

            // Act
            rule.ApplicableToMembersWithReturnType = typeof(string);

            // Assert
            rule.DescriptionOfValidCall.Should().Be("Any call with return type System.String to the fake object.");
        }

        private AnyCallCallRule CreateRule()
        {
            return new AnyCallCallRule();
        }
    }
}
