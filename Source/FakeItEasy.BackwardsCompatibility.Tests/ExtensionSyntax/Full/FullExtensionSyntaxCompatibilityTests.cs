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
            A.CallTo(() => this.fakeAssertionsFactory.CreateAsserter<IFoo>(A<FakeManager>.Ignored)).Returns(this.fakeAssertions);

            this.fakeConfiguration = A.Fake<IStartConfiguration<IFoo>>();
            this.fakeConfigurationFactory = A.Fake<IStartConfigurationFactory>();
            A.CallTo(() => this.fakeConfigurationFactory.CreateConfiguration<IFoo>(A<FakeManager>.Ignored)).Returns(this.fakeConfiguration);
        }

        [Test]
        public void Assert_should_be_null_guarded()
        {
            NullGuardedConstraint.Assert(() =>
                FullExtensionSyntax.Assert(A.Fake<IFoo>()));
        }
    }
}
