namespace FakeItEasy.Tests
{
    using FakeItEasy.Configuration;
    using FakeItEasy.Core;
    using NUnit.Framework;

    [TestFixture]
    public class NextCallTests
        : ConfigurableServiceLocatorTestBase
    {
        [Test]
        public void To_should_be_properly_guarded()
        {
            NullGuardedConstraint.Assert(() => 
                NextCall.To(A.Fake<IFoo>()));
        }

        [Test]
        public void To_should_return_builder_from_factory()
        {
            // Arrange
            var foo = A.Fake<IFoo>();
            var fake = Fake.GetFakeManager(foo);

            var recordedRule = A.Fake<RecordedCallRule>();
            this.StubResolve<RecordedCallRule.Factory>(() => recordedRule);

            var recordingRule = A.Fake<RecordingCallRule<IFoo>>(x => x.WithArgumentsForConstructor(() => new RecordingCallRule<IFoo>(fake, recordedRule, _ => null, A.Fake<IFakeObjectCallFormatter>())));
            var recordingRuleFactory = A.Fake<IRecordingCallRuleFactory>();
            A.CallTo(() => recordingRuleFactory.Create<IFoo>(fake, recordedRule)).Returns(recordingRule);
            this.StubResolve<IRecordingCallRuleFactory>(recordingRuleFactory);

            var builder = this.CreateFakeVisualBasicRuleBuilder();
            this.StubResolve<RecordingRuleBuilder.Factory>((r, f) =>
                {
                    return r.Equals(recordedRule) && f.Equals(fake) ? builder : null;
                });
            
            // Act
            var result = NextCall.To(foo);

            // Assert
            Assert.That(result, Is.SameAs(builder));
            Assert.That(fake.Rules, Has.Some.SameAs(recordingRule));
        }

        protected override void OnSetup()
        {
            A.Fake<IConfigurationFactory>();
        }

        private RecordingRuleBuilder CreateFakeVisualBasicRuleBuilder()
        {
            var rule = A.Fake<RecordedCallRule>();

            var wrapped = A.Fake<RuleBuilder>(x => x.WithArgumentsForConstructor(() =>
                new RuleBuilder(rule, A.Fake<FakeManager>(), c => A.Fake<IFakeAsserter>())));
            var result = A.Fake<RecordingRuleBuilder>(x => x.WithArgumentsForConstructor(() =>
                new RecordingRuleBuilder(rule, wrapped)));

            return result;
        }
    }
}
