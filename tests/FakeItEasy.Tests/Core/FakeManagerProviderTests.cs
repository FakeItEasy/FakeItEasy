namespace FakeItEasy.Tests.Core
{
    using System;
    using System.Reflection;
    using FakeItEasy.Core;
    using FakeItEasy.Creation;
    using FluentAssertions;
    using Xunit;

    public class FakeManagerProviderTests
    {
        private readonly object proxy;
        private readonly FakeManager fakeManager;

        private readonly FakeManager.Factory fakeManagerFactory = null;
        private readonly IFakeManagerAccessor fakeManagerAccessor = null;
        private readonly Type typeOfFake = null;
        private readonly IProxyOptions proxyOptions = null;
        private readonly FakeManagerProvider fakeManagerProvider = null;

        public FakeManagerProviderTests()
        {
            this.fakeManagerFactory = A.Fake<FakeManager.Factory>();
            this.fakeManagerAccessor = A.Fake<IFakeManagerAccessor>();
            this.typeOfFake = A.Fake<Type>();
            this.proxyOptions = A.Fake<IProxyOptions>();

            this.fakeManagerProvider = new FakeManagerProvider(
                this.fakeManagerFactory,
                this.fakeManagerAccessor,
                this.typeOfFake,
                this.proxyOptions);

            this.proxy = new object();
            this.fakeManager = new FakeManager(typeof(int), this.proxy, null);
            A.CallTo(() => this.fakeManagerFactory(A<Type>._, this.proxy)).Returns(this.fakeManager);
        }

        [Fact]
        public void Fetch_should_return_a_fake_manager_from_the_factory()
        {
            // Arrange

            // Act
            var fakeCallProcessor = this.fakeManagerProvider.Fetch(this.proxy);

            // Assert
            fakeCallProcessor.Should().BeSameAs(this.fakeManager);
        }

        [Fact]
        public void Fetch_should_tag_the_proxy_with_the_manager()
        {
            // Arrange

            // Act
            this.fakeManagerProvider.Fetch(this.proxy);

            // Assert
            A.CallTo(() => this.fakeManagerAccessor.SetFakeManager(this.proxy, this.fakeManager)).MustHaveHappened();
        }

        [Fact]
        public void Fetch_should_configure_the_proxy_with_all_fake_configuration_actions()
        {
            // Arrange
            var fakeConfigurationAction1 = A.Fake<Action<object>>();
            var fakeConfigurationAction2 = A.Fake<Action<object>>();

            A.CallTo(() => this.proxyOptions.ProxyConfigurationActions)
                .Returns(new[] { fakeConfigurationAction1, fakeConfigurationAction2 });

            // Act
            this.fakeManagerProvider.Fetch(this.proxy);

            // Assert
            A.CallTo(() => fakeConfigurationAction1(this.proxy)).MustHaveHappened();
            A.CallTo(() => fakeConfigurationAction2(this.proxy)).MustHaveHappened();
        }

        [Fact]
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

            A.CallTo(() => this.fakeManagerFactory(this.typeOfFake, this.proxy)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void EnsureInitialized_should_create_exactly_one_fake_manager_when_called_repeatedly()
        {
            // Arrange

            // Act
            this.fakeManagerProvider.EnsureInitialized(this.proxy);
            this.fakeManagerProvider.EnsureInitialized(this.proxy);
            this.fakeManagerProvider.EnsureInitialized(this.proxy);

            // Assert
            A.CallTo(() => this.fakeManagerFactory(this.typeOfFake, this.proxy)).MustHaveHappenedOnceExactly();
        }

        private static FakeManager GetInitializedFakeManager(FakeManagerProvider fakeManagerProvider)
        {
            var fieldInfo = fakeManagerProvider.GetType().GetField("initializedFakeManager", BindingFlags.NonPublic | BindingFlags.Instance);
            return (FakeManager)fieldInfo.GetValue(fakeManagerProvider);
        }
    }
}
