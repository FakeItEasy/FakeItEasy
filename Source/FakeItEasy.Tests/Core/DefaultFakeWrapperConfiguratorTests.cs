namespace FakeItEasy.Tests.Core
{
    using System.Linq;
    using FakeItEasy.Core;
    using FakeItEasy.SelfInitializedFakes;
    using NUnit.Framework;

    [TestFixture]
    public class DefaultFakeWrapperConfiguratorTests
    {
        private IFoo faked;
        private DefaultFakeWrapperConfigurer wrapperConfigurator;

        [SetUp]
        public void Setup()
        {
            this.faked = A.Fake<IFoo>();

            IFoo wrapped = A.Fake<IFoo>();
            this.wrapperConfigurator = new DefaultFakeWrapperConfigurer(wrapped);
        }

        [Test]
        public void ConfigureFakeToWrap_should_add_WrappedFakeObjectRule_to_fake_object()
        {
            // Arrange

            // Act
            this.wrapperConfigurator.ConfigureFakeToWrap(this.faked);

            // Assert
            Assert.That(Fake.GetFakeManager(this.faked).Rules.ToArray(), Has.Some.InstanceOf<WrappedObjectRule>());
        }

        [Test]
        public void ConfigureFakeToWrap_should_add_self_initialization_rule_when_recorder_is_specified()
        {
            // Arrange
            this.wrapperConfigurator.Recorder = A.Fake<ISelfInitializingFakeRecorder>();

            // Act
            this.wrapperConfigurator.ConfigureFakeToWrap(this.faked);

            // Assert
            Assert.That(Fake.GetFakeManager(this.faked).Rules.First(), Is.InstanceOf<SelfInitializationRule>());
        }

        [Test]
        public void ConfigureFakeToWrap_should_not_add_self_initialization_rule_when_recorder_is_not_specified()
        {
            // Arrange

            // Act
            this.wrapperConfigurator.ConfigureFakeToWrap(this.faked);

            // Assert
            Assert.That(Fake.GetFakeManager(this.faked).Rules, Has.None.InstanceOf<SelfInitializationRule>());
        }
    }
}
