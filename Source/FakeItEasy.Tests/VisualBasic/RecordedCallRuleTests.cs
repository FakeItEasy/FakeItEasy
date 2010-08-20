namespace FakeItEasy.Tests.VisualBasic
{
    using System;
    using FakeItEasy.Core;
    using NUnit.Framework;

    [TestFixture]
    public class RecordedCallRuleTests
    {
        private RecordedCallRule CreateRule()
        {
            return new RecordedCallRule(A.Fake<MethodInfoManager>());
        }

        [Test]
        public void UsePredicateToValidateArguments_should_set_predicate_to_IsApplicableToArguments()
        {
            var rule = this.CreateRule();

            Func<ArgumentCollection, bool> predicate = x => true;

            rule.UsePredicateToValidateArguments(predicate);

            Assert.That(rule.IsApplicableToArguments, Is.SameAs(predicate));
        }

        [Test]
        public void DescriptionOfValidCall_should_be_tested()
        {
            // Arrange

            // Act

            // Assert
            Assert.Fail("The test has not yet been implemented.");
        }
    }
}
