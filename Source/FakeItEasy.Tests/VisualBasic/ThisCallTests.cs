using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using FakeItEasy.VisualBasic;
using FakeItEasy.Core;
using FakeItEasy.Assertion;
using System.Linq.Expressions;
using FakeItEasy.Configuration;

namespace FakeItEasy.Tests.VisualBasic
{
    [TestFixture]
    public class ThisCallTests
        : ConfigurableServiceLocatorTestBase
    {
        private IConfigurationFactory builderFactory;

        protected override void OnSetUp()
        {
            this.builderFactory = A.Fake<IConfigurationFactory>();
        }

        [Test]
        public void To_should_be_properly_guarded()
        {
            NullGuardedConstraint.Assert(() => 
                ThisCall.To(A.Fake<IFoo>()));
        }

        [Test]
        public void To_should_return_builder_from_factory()
        {
            // Arrange
            var foo = A.Fake<IFoo>();
            var fake = Fake.GetFakeObject(foo);

            var recordedRule = A.Fake<RecordedCallRule>();
            this.StubResolve<RecordedCallRule.Factory>(() => recordedRule);

            var recordingRule = A.Fake<RecordingCallRule<IFoo>>(x => x.WithArgumentsForConstructor(() => new RecordingCallRule<IFoo>(fake, recordedRule, _ => null)));
            var recordingRuleFactory = A.Fake<IRecordingCallRuleFactory>();
            A.CallTo(() => recordingRuleFactory.Create<IFoo>(fake, recordedRule)).Returns(recordingRule);
            this.StubResolve<IRecordingCallRuleFactory>(recordingRuleFactory);

            var builder = new RuleBuilder(A.Fake<BuildableCallRule>(), fake, x => null);
            this.StubResolve<RuleBuilder.Factory>((r, f) =>
                {
                    return r.Equals(recordedRule) && f.Equals(fake) ? builder : null;
                });
            
            // Act
            var result = ThisCall.To(foo);

            // Assert
            Assert.That(result, Is.SameAs(builder));
            Assert.That(fake.Rules, Has.Some.SameAs(recordingRule));
        }
    }
}
