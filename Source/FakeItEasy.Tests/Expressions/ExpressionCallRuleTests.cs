namespace FakeItEasy.Tests.Expressions
{
    using System;
    using System.Linq.Expressions;
    using FakeItEasy.Configuration;
    using FakeItEasy.Core;
    using FakeItEasy.Expressions;
    using FakeItEasy.Tests.TestHelpers;
    using NUnit.Framework;

    [TestFixture]
    public class ExpressionCallRuleTests
    {
        private ExpressionCallMatcher callMatcher;

        [SetUp]
        public void Setup()
        {
            this.callMatcher = A.Fake<ExpressionCallMatcher>();
        }

        [Test]
        public void IsApplicableTo_should_pass_call_to_call_matcher()
        {
            var call = ExpressionHelper.CreateFakeCall<IFoo>(x => x.Bar());

            var rule = this.CreateRule<IFoo>();

            rule.IsApplicableTo(call);

            A.CallTo(() => this.callMatcher.Matches(call)).MustHaveHappened();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void IsApplicableTo_should_return_result_from_call_matcher(bool callMatcherResult)
        {
            var call = ExpressionHelper.CreateFakeCall<IFoo>(x => x.Bar());

            A.CallTo(() => this.callMatcher.Matches(call)).Returns(callMatcherResult);

            var rule = this.CreateRule<IFoo>();

            var result = rule.IsApplicableTo(call);

            Assert.That(result, Is.EqualTo(callMatcherResult));
        }

        [Test]
        public void Constructor_should_be_null_guarded()
        {
            NullGuardedConstraint.Assert(() =>
                new ExpressionCallRule(this.callMatcher));
        }
      
        [Test]
        public void Apply_should_call_the_applicator_with_the_incoming_call()
        {
            IInterceptedFakeObjectCall callPassedToApplicator = null;
            var callPassedToRule = FakeCall.Create<IFoo>("Bar");

            var rule = this.CreateRule<IFoo>();
            rule.Applicator = x => callPassedToApplicator = x;

            rule.Apply(callPassedToRule);

            Assert.That(callPassedToApplicator, Is.SameAs(callPassedToRule));
        }

        [Test]
        public void NumberOfTimesToCall_should_be_settable_and_gettable()
        {
            var rule = this.CreateRule<IFoo>();
            rule.NumberOfTimesToCall = 10;

            Assert.That(rule.NumberOfTimesToCall, Is.EqualTo(10));
        }

        [Test]
        public void DescriptionOfValidCall_should_return_expressionMatcher_ToString()
        {
            A.CallTo(() => this.callMatcher.DescriptionOfMatchingCall).Returns("foo");

            var rule = this.CreateRule<IFoo>();

            Assert.That(rule.DescriptionOfValidCall, Is.EqualTo("foo"));
        }

        [Test]
        public void UsePredicateToValidateArguments_should_pass_predicate_to_call_matcher()
        {
            Func<ArgumentCollection, bool> predicate = x => true;

            var rule = this.CreateRule<IFoo>();

            rule.UsePredicateToValidateArguments(predicate);

            A.CallTo(() => this.callMatcher.UsePredicateToValidateArguments(predicate)).MustHaveHappened();
        }

        private ExpressionCallRule CreateRule<T>()
        {
            return new ExpressionCallRule(this.callMatcher) { Applicator = x => { } };
        }
    }
}