namespace FakeItEasy.BackwardsCompatibility.Tests.ExtensionSyntax.Full
{
    using FakeItEasy.Core;
    using FakeItEasy.Assertion;
    using FakeItEasy.Configuration;
    using FakeItEasy.Tests;
    using NUnit.Framework;
    using FullExtensionSyntax = FakeItEasy.ExtensionSyntax.Full.FullExtensionSyntaxCompatibilityExtensions;

    [TestFixture]
    public class FullExtensionSyntaxCompatibilityTests
        : ConfigurableServiceLocatorTestBase
    {
        IFakeAssertionsFactory fakeAssertionsFactory;
        IFakeAssertions<IFoo> fakeAssertions;
        IStartConfigurationFactory fakeConfigurationFactory;
        IStartConfiguration<IFoo> fakeConfiguration;

        protected override void OnSetUp()
        {
            this.fakeAssertions = A.Fake<IFakeAssertions<IFoo>>();
            this.fakeAssertionsFactory = A.Fake<IFakeAssertionsFactory>();
            A.CallTo(() => this.fakeAssertionsFactory.CreateAsserter<IFoo>(A<FakeObject>.Ignored)).Returns(this.fakeAssertions);

            this.fakeConfiguration = A.Fake<IStartConfiguration<IFoo>>();
            this.fakeConfigurationFactory = A.Fake<IStartConfigurationFactory>();
            A.CallTo(() => this.fakeConfigurationFactory.CreateConfiguration<IFoo>(A<FakeObject>.Ignored)).Returns(this.fakeConfiguration);
        }

        [Test]
        public void Assert_should_be_null_guarded()
        {
            NullGuardedConstraint.Assert(() =>
                FullExtensionSyntax.Assert(A.Fake<IFoo>()));
        }

        [Test]
        public void Assert_should_return_fake_assertion_object_from_factory()
        {
            var fake = A.Fake<IFoo>();


            using (Fake.CreateScope())
            {
                this.StubResolve<IFakeAssertionsFactory>(this.fakeAssertionsFactory);
                var assertions = FullExtensionSyntax.Assert(fake);

                Assert.That(assertions, Is.SameAs(this.fakeAssertions));
            }
        }
    }
}
