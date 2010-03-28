namespace FakeItEasy.Tests.Core
{
    using System.Linq;
    using FakeItEasy.Core;
    using NUnit.Framework;
    using FakeItEasy.SelfInitializedFakes;

    [TestFixture]
    public class DefaultFakeWrapperConfiguratorTests
    {
        private IFoo faked;
        private DefaultFakeWrapperConfigurator wrapperConfigurator;

        [SetUp]
        public void SetUp()
        {
            this.faked = A.Fake<IFoo>();

            this.wrapperConfigurator = new DefaultFakeWrapperConfigurator();
        }

        [Test]
        public void ConfigureFakeToWrap_should_add_WrappedFakeObjectRule_to_fake_object()
        {
            // Arrange
            var wrapped = A.Fake<IFoo>();

            // Act
            this.wrapperConfigurator.ConfigureFakeToWrap(this.faked, wrapped, null);

            // Assert
            Assert.That(Fake.GetFakeObject(this.faked).Rules.ToArray(), Has.Some.InstanceOf<WrappedObjectRule>());
        }

        [Test]
        public void ConfigureFakeToWrap_should_add_self_initialization_rule_when_recorder_is_specified()
        {
            // Arrange
            var recorder = A.Fake<ISelfInitializingFakeRecorder>();
            var wrapped = A.Fake<IFoo>();

            // Act
            this.wrapperConfigurator.ConfigureFakeToWrap(this.faked, wrapped, recorder);

            // Assert
            Assert.That(Fake.GetFakeObject(this.faked).Rules.First(), Is.InstanceOf<SelfInitializationRule>());
        }

        [Test]
        public void ConfigureFakeToWrap_should_not_add_self_initialization_rule_when_recorder_is_not_specified()
        {
            // Arrange
            var wrapped = A.Fake<IFoo>();

            // Act
            this.wrapperConfigurator.ConfigureFakeToWrap(this.faked, wrapped, null);

            // Assert
            Assert.That(Fake.GetFakeObject(this.faked).Rules, Has.None.InstanceOf<SelfInitializationRule>());
        }
    }
}
