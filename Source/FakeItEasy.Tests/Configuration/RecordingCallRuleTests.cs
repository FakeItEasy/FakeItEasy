namespace FakeItEasy.Tests.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FakeItEasy.Configuration;
    using FakeItEasy.Core;
    using NUnit.Framework;

    [TestFixture]
    public class RecordingCallRuleTests
    {
        private IFoo fakedObject;
        private FakeManager fakeObject;
        private RecordedCallRule recordedRule;
        private FakeAsserter.Factory asserterFactory;
        private IEnumerable<IFakeObjectCall> argumentUsedForAsserterFactory;
        private IFakeAsserter asserter;
        private IFakeObjectCallFormatter callFormatter;
        
        [SetUp]
        public void Setup()
        {
            this.fakedObject = A.Fake<IFoo>();
            this.fakeObject = Fake.GetFakeManager(this.fakedObject);
            this.recordedRule = A.Fake<RecordedCallRule>(x => x.WithArgumentsForConstructor(() => new RecordedCallRule(A.Fake<MethodInfoManager>())));
            this.callFormatter = A.Fake<IFakeObjectCallFormatter>();

            this.asserter = A.Fake<IFakeAsserter>();

            this.asserterFactory = x =>
                {
                    this.argumentUsedForAsserterFactory = x;
                    return this.asserter;
                };
        }

        [TearDown]
        public void Teardown()
        {
            this.argumentUsedForAsserterFactory = null;
        }

        [Test]
        public void Apply_should_call_DoNotRecordCall()
        {
            // Arrange
            var rule = this.CreateRule();
            var call = A.Fake<IInterceptedFakeObjectCall>();

            // Act
            rule.Apply(call);

            // Assert
            A.CallTo(() => call.DoNotRecordCall()).MustHaveHappened();
        }

        [Test]
        public void Apply_should_create_asserter_and_call_it_with_the_recorded_call_when_IsAssertion_is_set_to_true_on_recorded_rule()
        {
            this.recordedRule.IsAssertion = true;
            this.recordedRule.RepeatConstraint = Repeated.AtLeast.Once;

            var rule = this.CreateRule();

            var call = A.Fake<IInterceptedFakeObjectCall>();
            A.CallTo(() => this.callFormatter.GetDescription(call)).Returns("call description");

            rule.Apply(call);

            A.CallTo(() => this.asserter.AssertWasCalled(A<Func<IFakeObjectCall, bool>>._, "call description", A<Func<int, bool>>._, "the number of times specified by predicate"));
        }

        [Test]
        public void Apply_should_create_asserter_and_call_it_with_call_predicate_that_invokes_IsApplicableTo_on_the_recorded_rule()
        {
            this.recordedRule.IsAssertion = true;
            this.recordedRule.RepeatConstraint = Repeated.AtLeast.Once;

            var rule = this.CreateRule();

            var call = A.Fake<IInterceptedFakeObjectCall>();
            A.CallTo(() => this.callFormatter.GetDescription(call)).Returns("call description");

            rule.Apply(call);

            var asserterCall = Fake.GetCalls(this.asserter).Matching<IFakeAsserter>(x => x.AssertWasCalled(A<Func<IFakeObjectCall, bool>>._, "call description", A<Func<int, bool>>._, A<string>._)).Single();
            var callPredicate = asserterCall.Arguments.Get<Func<IFakeObjectCall, bool>>("callPredicate");

            callPredicate.Invoke(call);

            A.CallTo(() => this.recordedRule.IsApplicableTo(call)).MustHaveHappened();
        }

        [Test]
        public void Apply_should_call_asserter_with_repeat_predicate_from_recorded_rule()
        {
            var repeat = Repeated.AtLeast.Once;
            this.recordedRule.IsAssertion = true;
            this.recordedRule.RepeatConstraint = repeat;

            this.recordedRule.IsAssertion = true;

            var rule = this.CreateRule();

            var call = A.Fake<IInterceptedFakeObjectCall>();
            A.CallTo(() => this.callFormatter.GetDescription(call)).Returns("call description");

            rule.Apply(call);

            A.CallTo(() => this.asserter.AssertWasCalled(A<Func<IFakeObjectCall, bool>>._, "call description", A<Func<int, bool>>._, "at least once")).MustHaveHappened();
            
            var asserterCall = Fake.GetCalls(this.asserter).Matching<IFakeAsserter>(x => x.AssertWasCalled(A<Func<IFakeObjectCall, bool>>._, "call description", A<Func<int, bool>>._, A<string>._)).Single();
            var repeatPredicatePassedToAsserter = asserterCall.Arguments.Get<Func<int, bool>>("repeatPredicate");

            Assert.That(repeatPredicatePassedToAsserter.Invoke(0), Is.False);
            Assert.That(repeatPredicatePassedToAsserter.Invoke(1), Is.True);
        }

        [Test]
        public void Apply_should_pass_calls_from_fake_object_to_fake_asserter_factory()
        {
            this.fakedObject.Bar();
            this.fakedObject.Baz();

            this.recordedRule.IsAssertion = true;
            this.recordedRule.RepeatConstraint = Repeated.AtLeast.Once;

            var rule = this.CreateRule();

            rule.Apply(A.Fake<IInterceptedFakeObjectCall>());

            Assert.That(this.argumentUsedForAsserterFactory, Is.EquivalentTo(this.fakeObject.RecordedCallsInScope));
        }

        [Test]
        public void NumberOfTimesToCall_should_be_one()
        {
            var rule = this.CreateRule();

            Assert.That(rule.NumberOfTimesToCall, Is.EqualTo(1));
        }

        private RecordingCallRule<IFoo> CreateRule()
        {
            return new RecordingCallRule<IFoo>(this.fakeObject, this.recordedRule, this.asserterFactory, this.callFormatter);
        }
    }
}
