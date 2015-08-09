namespace FakeItEasy.Tests.Core
{
    using System;
    using System.Reflection;
    using FakeItEasy.Core;
    using FakeItEasy.Creation;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using NUnit.Framework;

    [TestFixture]
    public class FakeManagerProviderTests
    {
        [Fake]
        private FakeManager.Factory fakeManagerFactory = null;

        [Fake]
        private IFakeManagerAccessor fakeManagerAccessor = null;

        [Fake]
        private IFakeObjectConfigurator fakeObjectConfigurator = null;

        [Fake]
        private Type typeOfFake = null;

        [Fake]
        private FakeOptions fakeOptions = null;

        [UnderTest]
        private FakeManagerProvider fakeManagerProvider = null;

        private object proxy;
        private FakeManager fakeManager;

        [SetUp]
        public void Setup()
        {
            Fake.InitializeFixture(this);

            this.proxy = new object();
            this.fakeManager = new FakeManager(typeof(int), this.proxy);
            A.CallTo(() => this.fakeManagerFactory(A<Type>._, this.proxy)).Returns(this.fakeManager);
        }

        [Test]
        public void Fetch_should_return_a_fake_manager_from_the_factory()
        {
            // Arrange

            // Act
            var fakeCallProcessor = this.fakeManagerProvider.Fetch(this.proxy);

            // Assert
            fakeCallProcessor.Should().BeSameAs(this.fakeManager);
        }

        [Test]
        public void Fetch_should_tag_the_proxy_with_the_manager()
        {
            // Arrange

            // Act
            this.fakeManagerProvider.Fetch(this.proxy);

            // Assert
            A.CallTo(() => this.fakeManagerAccessor.TagProxy(this.proxy, this.fakeManager)).MustHaveHappened();
        }

        [Test]
        public void Fetch_should_configure_the_proxy()
        {
            // Arrange

            // Act
            this.fakeManagerProvider.Fetch(this.proxy);

            // Assert
            A.CallTo(() => this.fakeObjectConfigurator.ConfigureFake(this.typeOfFake, this.proxy)).MustHaveHappened();
        }

        [Test]
        public void Fetch_should_configure_the_proxy_with_all_fake_configuration_actions()
        {
            // Arrange
            var fakeConfigurationAction1 = A.Fake<Action<object>>();
            var fakeConfigurationAction2 = A.Fake<Action<object>>();
            this.fakeOptions.AddProxyConfigurationAction(fakeConfigurationAction1);
            this.fakeOptions.AddProxyConfigurationAction(fakeConfigurationAction2);

            // Act
            this.fakeManagerProvider.Fetch(this.proxy);

            // Assert
            A.CallTo(() => fakeConfigurationAction1(this.proxy)).MustHaveHappened();
            A.CallTo(() => fakeConfigurationAction2(this.proxy)).MustHaveHappened();
        }

        [Test]
        public void Fetch_should_create_exactly_one_fake_manager_when_called_repeatedly()
        {
            // Arrange

            // Act
            var fakeCallProcessor1 = this.fakeManagerProvider.Fetch(this.proxy);
            var fakeCallProcessor2 = this.fakeManagerProvider.Fetch(this.proxy);
            var fakeCallProcessor3 = this.fakeManagerProvider.Fetch(this.proxy);

            // Assert
            fakeCallProcessor1.Should().BeSameAs(fakeCallProcessor2);
            fakeCallProcessor2.Should().BeSameAs(fakeCallProcessor3);

            A.CallTo(() => this.fakeManagerFactory(this.typeOfFake, this.proxy)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void EnsureInitialized_should_create_exactly_one_fake_manager_when_called_repeatedly()
        {
            // Arrange

            // Act
            this.fakeManagerProvider.EnsureInitialized(this.proxy);
            this.fakeManagerProvider.EnsureInitialized(this.proxy);
            this.fakeManagerProvider.EnsureInitialized(this.proxy);

            // Assert
            A.CallTo(() => this.fakeManagerFactory(this.typeOfFake, this.proxy)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void Should_be_able_to_fetch_a_serialized_and_deserialized_initialized_provider()
        {
            // Arrange
            this.fakeManagerProvider.EnsureInitialized(this.proxy);
            var deserializedFakeManagerProvider = BinarySerializationHelper.SerializeAndDeserialize(this.fakeManagerProvider);
            var deserializedFakeManager = GetInitializedFakeManager(deserializedFakeManagerProvider);
            
            // Act
            var exception = Record.Exception(() => deserializedFakeManagerProvider.Fetch(deserializedFakeManager.Object));

            // Assert
            exception.Should().BeNull();
        }

        private static FakeManager GetInitializedFakeManager(FakeManagerProvider fakeManagerProvider)
        {
            var fieldInfo = fakeManagerProvider.GetType().GetField("initializedFakeManager", BindingFlags.NonPublic | BindingFlags.Instance);
            return (FakeManager)fieldInfo.GetValue(fakeManagerProvider);
        }
    }
}