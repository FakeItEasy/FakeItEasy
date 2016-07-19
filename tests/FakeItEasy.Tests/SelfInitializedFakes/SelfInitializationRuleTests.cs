namespace FakeItEasy.Tests.SelfInitializedFakes
{
    using FakeItEasy.Core;
    using FakeItEasy.SelfInitializedFakes;
    using FluentAssertions;
    using Xunit;

    public class SelfInitializationRuleTests
    {
        private readonly IFakeObjectCallRule wrappedRule;
        private readonly ISelfInitializingFakeRecorder recorder;

        public SelfInitializationRuleTests()
        {
            this.wrappedRule = A.Fake<IFakeObjectCallRule>();
            this.recorder = A.Fake<ISelfInitializingFakeRecorder>();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void IsApplicable_should_return_value_from_wrapped_rule(bool wrappedRuleIsApplicable)
        {
            var call = A.Fake<IInterceptedFakeObjectCall>();

            A.CallTo(() => this.wrappedRule.IsApplicableTo(call))
                .Returns(wrappedRuleIsApplicable);

            var rule = this.CreateRule();

            rule.IsApplicableTo(call).Should().Be(wrappedRuleIsApplicable);
        }

        [Fact]
        public void Apply_should_call_apply_next_on_recorder_when_recorder_is_not_recording()
        {
            // Arrange
            A.CallTo(() => this.recorder.IsRecording).Returns(false);

            var call = this.CreateFakeCall();

            // Act
            var rule = this.CreateRule();
            rule.Apply(call);

            // Assert
            A.CallTo(() => this.recorder.ApplyNext(call)).MustHaveHappened();
        }

        [Fact]
        public void Apply_should_call_apply_on_rule_when_recorder_is_recording()
        {
            // Arrange
            A.CallTo(() => this.recorder.IsRecording).Returns(true);
            var call = this.CreateFakeCall();

            // Act
            var rule = this.CreateRule();
            rule.Apply(call);

            // Assert
            A.CallTo(() => this.wrappedRule.Apply(call)).MustHaveHappened();
        }

        [Fact]
        public void Apply_should_call_record_on_recorder_when_recorder_is_recording()
        {
            // Arrange
            A.CallTo(() => this.recorder.IsRecording).Returns(true);

            var call = this.CreateFakeCall();
            var frozenCall = call.AsReadOnly();

            // Act
            var rule = this.CreateRule();
            rule.Apply(call);

            // Assert
            A.CallTo(() => this.recorder.RecordCall(frozenCall)).MustHaveHappened();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(null)]
        [InlineData(10)]
        [InlineData(100)]
        public void NumberOfTimesToCall_should_return_value_from_wrapped_rule(int? numberOfTimesToCallWrappedRule)
        {
            A.CallTo(() => this.wrappedRule.NumberOfTimesToCall).Returns(numberOfTimesToCallWrappedRule);

            var rule = this.CreateRule();

            rule.NumberOfTimesToCall.Should().Be(numberOfTimesToCallWrappedRule);
        }

        private SelfInitializationRule CreateRule()
        {
            return new SelfInitializationRule(this.wrappedRule, this.recorder);
        }

        private IInterceptedFakeObjectCall CreateFakeCall()
        {
            var call = A.Fake<IInterceptedFakeObjectCall>();
            var frozenCall = A.Fake<ICompletedFakeObjectCall>();

            A.CallTo(() => this.wrappedRule.IsApplicableTo(call)).Returns(true);
            A.CallTo(() => call.AsReadOnly()).Returns(frozenCall);

            return call;
        }
    }
}
