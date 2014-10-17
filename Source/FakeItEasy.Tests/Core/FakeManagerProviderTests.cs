namespace FakeItEasy.Tests.Core
{
    using System;
    using System.Reflection;
    using FakeItEasy.Core;
    using FakeItEasy.Creation;
    using FakeItEasy.SelfInitializedFakes;
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
        private IFakeWrapperConfigurer wrapperConfigurer = null;

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
            A.CallTo(() => this.fakeManagerFactory(A<Type>._, A<object>._)).Returns(this.fakeManager);
        }

        [Test]
        public void Fetch_should_create_a_fake_manager_and_tag_and_configure_the_fake()
        {
            // Arrange
            var onFakeConfigurationAction1 = A.Fake<Action<object>>();
            var onFakeConfigurationAction2 = A.Fake<Action<object>>();

            this.fakeOptions.WrappedInstance = new object();
            this.fakeOptions.SelfInitializedFakeRecorder = A.Fake<ISelfInitializingFakeRecorder>();
            this.fakeOptions.OnFakeConfigurationActions = new[] { onFakeConfigurationAction1, onFakeConfigurationAction2 };

            // Act
            var fakeCallProcessor = this.fakeManagerProvider.Fetch(this.proxy);

            // Assert
            fakeCallProcessor.Should().BeSameAs(this.fakeManager);

            A.CallTo(() => this.fakeManagerFactory(this.typeOfFake, this.proxy)).MustHaveHappened();

            A.CallTo(() => this.fakeManagerAccessor.TagProxy(this.proxy, this.fakeManager)).MustHaveHappened();

            A.CallTo(() => this.fakeObjectConfigurator.ConfigureFake(this.typeOfFake, this.proxy)).MustHaveHappened();

            A.CallTo(() => this.wrapperConfigurer.ConfigureFakeToWrap(this.proxy, this.fakeOptions.WrappedInstance, this.fakeOptions.SelfInitializedFakeRecorder))
                .MustHaveHappened();

            A.CallTo(() => onFakeConfigurationAction1(this.proxy)).MustHaveHappened();
            A.CallTo(() => onFakeConfigurationAction2(this.proxy)).MustHaveHappened();
        }

        [Test]
        public void Fetch_should_create_exactly_one_fake_manager()
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
        public void EnsureInitialized_should_create_exactly_one_fake_manager()
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
        public void Fetch_should_throw_if_the_wrong_proxy_object_was_specified()
        {
            // Arrange
            this.fakeManagerProvider.EnsureInitialized(this.proxy);

            // Act
            Action act = () => this.fakeManagerProvider.Fetch(new object());

            // Assert
            act.ShouldThrow<ArgumentException>().WithMessage("The fake manager was initialized for a different proxy.*");
        }

        [Test]
        public void Should_be_able_to_serialize_and_deserialize_initialized_provider_and_fetch_should_still_work()
        {
            // Arrange
            this.fakeManagerProvider.EnsureInitialized(this.proxy);

            // Act
            var deserializedFakeManagerProvider = BinarySerializationHelper.SerializeAndDeserialize(this.fakeManagerProvider);
            var deserializedFakeManager = GetInitializedFakeManager(deserializedFakeManagerProvider);
            Action act = () => deserializedFakeManagerProvider.Fetch(deserializedFakeManager.Object);

            // Assert
            act.ShouldNotThrow();
        }

        private static FakeManager GetInitializedFakeManager(FakeManagerProvider fakeManagerProvider)
        {
            var fieldInfo = fakeManagerProvider.GetType().GetField("initializedFakeManager", BindingFlags.NonPublic | BindingFlags.Instance);
            return (FakeManager)fieldInfo.GetValue(fakeManagerProvider);
        }
    }
}