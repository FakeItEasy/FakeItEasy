namespace FakeItEasy.Tests.Core
{
    using System.Linq;
    using FakeItEasy.Core;
    using FakeItEasy.Creation;
    using FakeItEasy.SelfInitializedFakes;
    using FluentAssertions;
    using NUnit.Framework;

    [TestFixture]
    public class FakeWrapperConfiguratorTests
    {
        private IFoo faked;
        private FakeWrapperConfigurator<IFoo> wrapperConfigurator;

        [SetUp]
        public void Setup()
        {
            this.faked = A.Fake<IFoo>();

            IFoo wrapped = A.Fake<IFoo>();
            this.wrapperConfigurator = new FakeWrapperConfigurator<IFoo>(A.Fake<IFakeOptionsBuilder<IFoo>>(), wrapped);
        }

        [Test]
        public void ConfigureFakeToWrap_should_add_WrappedFakeObjectRule_to_fake_object()
        {
            // Arrange

            // Act
            this.wrapperConfigurator.ConfigureFakeToWrap(this.faked);

            // Assert
            Fake.GetFakeManager(this.faked).Rules
                .Should().Contain(item => item.GetType().IsAssignableFrom(typeof(WrappedObjectRule)));
        }

        [Test]
        public void ConfigureFakeToWrap_should_add_self_initialization_rule_when_recorder_is_specified()
        {
            // Arrange
            this.wrapperConfigurator.Recorder = A.Fake<ISelfInitializingFakeRecorder>();

            // Act
            this.wrapperConfigurator.ConfigureFakeToWrap(this.faked);

            // Assert
            Fake.GetFakeManager(this.faked).Rules.First()
                .Should().BeOfType<SelfInitializationRule>();
        }

        [Test]
        public void ConfigureFakeToWrap_should_not_add_self_initialization_rule_when_recorder_is_not_specified()
        {
            // Arrange

            // Act
            this.wrapperConfigurator.ConfigureFakeToWrap(this.faked);

            // Assert
            Fake.GetFakeManager(this.faked).Rules
                .Should().NotContain(item => item.GetType().IsAssignableFrom(typeof(SelfInitializationRule)));
        }
    }
}