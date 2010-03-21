using FakeItEasy.Core;
using FakeItEasy.SelfInitializedFakes;
using NUnit.Framework;
using System.Reflection;
using System.Linq;
using System;

namespace FakeItEasy.Tests.SelfInitializedFakes
{
    [TestFixture]
    public class SelfInitializationRuleTests
    {
        private IFakeObjectCallRule wrappedRule;
        private ISelfInitializingFakeRecorder recorder;

        [SetUp]
        public void SetUp()
        {
            this.wrappedRule = A.Fake<IFakeObjectCallRule>();
            this.recorder = A.Fake<ISelfInitializingFakeRecorder>();
        }

        private SelfInitializationRule CreateRule()
        {
            return new SelfInitializationRule(this.wrappedRule, this.recorder);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void IsApplicable_should_return_value_from_wrapped_rule(bool wrappedRuleIsApplicable)
        {
            var call = A.Fake<IWritableFakeObjectCall>();

            Configure.Fake(this.wrappedRule)
                .CallsTo(x => x.IsApplicableTo(call))
                .Returns(wrappedRuleIsApplicable);

            var rule = this.CreateRule();

            Assert.That(rule.IsApplicableTo(call), Is.EqualTo(wrappedRuleIsApplicable));
        }

        [Test]
        public void Apply_should_call_apply_next_on_recorder_when_recorder_is_not_recording()
        {
            // Arrange
            Configure.Fake(this.recorder)
                .CallsTo(x => x.IsRecording)
                .Returns(false);

            var call = this.CreateFakeCall();

            // Act
            var rule = this.CreateRule();
            rule.Apply(call);

            // Assert
            OldFake.Assert(this.recorder)
                .WasCalled(x => x.ApplyNext(call));
        }

        [Test]
        public void Apply_should_call_apply_on_rule_when_recorder_is_recording()
        {
            // Arrange
            Configure.Fake(this.recorder)
                .CallsTo(x => x.IsRecording)
                .Returns(true);
            var call = this.CreateFakeCall();
            
            // Act
            var rule = this.CreateRule();
            rule.Apply(call);

            // Assert
            OldFake.Assert(this.wrappedRule)
                .WasCalled(x => x.Apply(call));
        }

        [Test]
        public void Apply_should_call_record_on_recorder_when_recorder_is_recording()
        {
            // Arrange
            Configure.Fake(this.recorder)
                .CallsTo(x => x.IsRecording)
                .Returns(true);
            
            var call = this.CreateFakeCall();
            var frozenCall = call.AsReadOnly();

            // Act
            var rule = this.CreateRule();
            rule.Apply(call);

            // Assert
            OldFake.Assert(this.recorder)
                .WasCalled(x => x.RecordCall(frozenCall));
        }

        [TestCase(1)]
        [TestCase(null)]
        [TestCase(10)]
        [TestCase(100)]
        public void NumberOfTimesToCall_should_return_value_from_wrapped_rule(int? numberOfTimesToCallWrappedRule)
        {
            Configure.Fake(this.wrappedRule)
                .CallsTo(x => x.NumberOfTimesToCall)
                .Returns(numberOfTimesToCallWrappedRule);

            var rule = this.CreateRule();

            Assert.That(rule.NumberOfTimesToCall, Is.EqualTo(numberOfTimesToCallWrappedRule));
        }

        private IWritableFakeObjectCall CreateFakeCall()
        {
            var call = A.Fake<IWritableFakeObjectCall>();
            var frozenCall = A.Fake<ICompletedFakeObjectCall>();

            Configure.Fake(this.wrappedRule)
                .CallsTo(x => x.IsApplicableTo(call))
                .Returns(true);
            Configure.Fake(call)
                .CallsTo(x => x.AsReadOnly())
                .Returns(frozenCall);

            return call;
        }

    }
}
