namespace FakeItEasy.Tests.Expressions
{
    using System;
    using System.Linq.Expressions;
    using FakeItEasy.Configuration;
    using FakeItEasy.Core;
    using FakeItEasy.Expressions;
    using FakeItEasy.Tests;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xunit;

    public class ExpressionCallRuleTests
    {
        private readonly ExpressionCallMatcher callMatcher;

        public ExpressionCallRuleTests()
        {
            this.callMatcher = A.Fake<ExpressionCallMatcher>();
        }

        [Fact]
        public void IsApplicableTo_should_pass_call_to_call_matcher()
        {
            var call = ExpressionHelper.CreateFakeCall<IFoo>(x => x.Bar());

            var rule = this.CreateRule();

            rule.IsApplicableTo(call);

            A.CallTo(() => this.callMatcher.Matches(call)).MustHaveHappened();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void IsApplicableTo_should_return_result_from_call_matcher(bool callMatcherResult)
        {
            var call = ExpressionHelper.CreateFakeCall<IFoo>(x => x.Bar());

            A.CallTo(() => this.callMatcher.Matches(call)).Returns(callMatcherResult);

            var rule = this.CreateRule();

            var result = rule.IsApplicableTo(call);

            result.Should().Be(callMatcherResult);
        }

        [Fact]
        public void Constructor_should_be_null_guarded()
        {
            Expression<Action> call = () => new ExpressionCallRule(this.callMatcher);
            call.Should().BeNullGuarded();
        }

        [Fact]
        public void Apply_should_call_the_applicator_with_the_incoming_call()
        {
            IInterceptedFakeObjectCall callPassedToApplicator = null;
            var callPassedToRule = FakeCall.Create<IFoo>("Bar");

            var rule = this.CreateRule();
            rule.UseApplicator(x => callPassedToApplicator = x);

            rule.Apply(callPassedToRule);

            callPassedToApplicator.Should().BeSameAs(callPassedToRule);
        }

        [Fact]
        public void NumberOfTimesToCall_should_be_settable_and_gettable()
        {
            var rule = this.CreateRule();
            rule.NumberOfTimesToCall = 10;

            rule.NumberOfTimesToCall.Should().Be(10);
        }

        [Fact]
        public void DescriptionOfValidCall_should_return_expressionMatcher_ToString()
        {
            A.CallTo(() => this.callMatcher.DescriptionOfMatchingCall).Returns("foo");

            var rule = this.CreateRule();

            rule.DescriptionOfValidCall.Should().Be("foo");
        }

        [Fact]
        public void UsePredicateToValidateArguments_should_pass_predicate_to_call_matcher()
        {
            Func<ArgumentCollection, bool> predicate = x => true;

            var rule = this.CreateRule();

            rule.UsePredicateToValidateArguments(predicate);

            A.CallTo(() => this.callMatcher.UsePredicateToValidateArguments(predicate)).MustHaveHappened();
        }

        private ExpressionCallRule CreateRule()
        {
            return new ExpressionCallRule(this.callMatcher) { };
        }
    }
}
