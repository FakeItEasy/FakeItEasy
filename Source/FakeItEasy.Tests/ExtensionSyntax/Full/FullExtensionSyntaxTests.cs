using System;
using System.Linq.Expressions;
using FakeItEasy.Configuration;
using FakeItEasy.Core;
using NUnit.Framework;
using ES = FakeItEasy.ExtensionSyntax.Full;

namespace FakeItEasy.Tests.ExtensionSyntax.Full
{
    [TestFixture]
    public class FullExtensionSyntaxTests : ConfigurableServiceLocatorTestBase
    {
        IStartConfigurationFactory fakeConfigurationFactory;
        IStartConfiguration<IFoo> fakeConfiguration;

        protected override void OnSetUp()
        {
            this.fakeConfiguration = A.Fake<IStartConfiguration<IFoo>>();
            this.fakeConfigurationFactory = A.Fake<IStartConfigurationFactory>();
            A.CallTo(() => this.fakeConfigurationFactory.CreateConfiguration<IFoo>(A<FakeManager>._))
                .Returns(this.fakeConfiguration);       
        }

        [Test]
        public void CallsTo_for_return_value_methods_should_be_null_guarded()
        {
            NullGuardedConstraint.Assert(() =>
                ES.FullExtensionSyntax.CallsTo("string", x => x.Length));
        }

        [Test]
        public void CallsTo_for_return_value_methods_should_return_fake_configuartion_from_factory()
        {
            var fake = A.Fake<IFoo>();

            var returnConfig = A.Fake<IReturnValueArgumentValidationConfiguration<int>>();
            A.CallTo(() => this.fakeConfiguration.CallsTo(A<Expression<Func<IFoo, int>>>._))
                .Returns(returnConfig);
            
            using (Fake.CreateScope())
            {
                this.StubResolve<IStartConfigurationFactory>(this.fakeConfigurationFactory);            
                
                var configuration = ES.FullExtensionSyntax.CallsTo(fake, x => x.Baz());
                Assert.That(configuration, Is.SameAs(returnConfig));
            }
        }

        [Test]
        public void CallsTo_for_void_methods_should_be_null_guarded()
        {
            NullGuardedConstraint.Assert(() =>
                ES.FullExtensionSyntax.CallsTo(A.Fake<IFoo>(), x => x.Bar()));
        }

        [Test]
        public void CallsTo_for_void_methods_should_return_fake_configuration_for_fake()
        {
            var fake = A.Fake<IFoo>();

            var callConfig = A.Fake<IVoidArgumentValidationConfiguration>();
            A.CallTo(() => this.fakeConfiguration.CallsTo(A<Expression<Action<IFoo>>>._))
                .Returns(callConfig);

            using (Fake.CreateScope())
            {
                this.StubResolve<IStartConfigurationFactory>(this.fakeConfigurationFactory);

                var configuration = ES.FullExtensionSyntax.CallsTo(fake, x => x.Bar());
                Assert.That(configuration, Is.SameAs(callConfig));
            }
        }

        [Test]
        public void AnyCall_should_return_fake_configuration_for_fake()
        {
            var fake = A.Fake<IFoo>();

            var callConfig = A.Fake<IAnyCallConfigurationWithNoReturnTypeSpecified>();
            A.CallTo(() => this.fakeConfiguration.AnyCall()).Returns(callConfig);

            using (Fake.CreateScope())
            {
                this.StubResolve<IStartConfigurationFactory>(this.fakeConfigurationFactory);

                var configuration = ES.FullExtensionSyntax.AnyCall(fake);
                Assert.That(configuration, Is.SameAs(callConfig));
            }            
        }

        [Test]
        public void AnyCall_should_be_null_guarded()
        {
            NullGuardedConstraint.Assert(() =>
                ES.FullExtensionSyntax.AnyCall(A.Fake<IFoo>()));
        }
    }
}
