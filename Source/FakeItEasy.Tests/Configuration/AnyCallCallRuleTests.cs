namespace FakeItEasy.Tests.Configuration
{
    using System;
    using FakeItEasy.Configuration;
    using FakeItEasy.Tests.TestHelpers;
    using NUnit.Framework;

    [TestFixture]
    public class AnyCallCallRuleTests
    {
        [TestCase(typeof(int), Result = true)]
        [TestCase(typeof(string), Result = false)]
        [TestCase(null, Result = true)]
        public bool IsApplicableTo_should_check_the_ApplicableToMembersWithReturnType_property(Type type)
        {
            // Arrange
            var rule = this.CreateRule();
            rule.ApplicableToMembersWithReturnType = type;

            var call = ExpressionHelper.CreateFakeCall<IFoo>(x => x.Baz());

            // Act

            // Assert
            return rule.IsApplicableTo(call);
        }

        [Test]
        public void IsApplicableTo_should_use_predicate_set_by_UsePredicateToValidateArguments(
            [Values(true, false)] bool predicateReturnValue)
        {
            // Arrange
            Func<ArgumentCollection, bool> argumentsPredicate = x => predicateReturnValue;

            var call = ExpressionHelper.CreateFakeCall<IFoo>(x => x.Bar("a", "b"));

            var rule = this.CreateRule();
            rule.UsePredicateToValidateArguments(argumentsPredicate);

            // Act
            rule.IsApplicableTo(call);

            // Assert
            Assert.That(rule.IsApplicableTo(call), Is.EqualTo(predicateReturnValue)); 
        }

        [Test]
        public void ToString_when_no_member_type_is_specified_should_return_correct_description()
        {
            // Arrange
            var rule = this.CreateRule();

            // Act

            // Assert
            Assert.That(rule.DescriptionOfValidCall, Is.EqualTo("Any call made to the fake object."));
        }

        [Test]
        public void ToString_when_member_type_is_set_should_return_correct_description()
        {
            // Arrange
            var rule = this.CreateRule();

            // Act
            rule.ApplicableToMembersWithReturnType = typeof(string);

            // Assert
            Assert.That(rule.DescriptionOfValidCall, Is.EqualTo("Any call with return type System.String to the fake object."));
        }

        private AnyCallCallRule CreateRule()
        {
            return new AnyCallCallRule();
        }
    }
}
