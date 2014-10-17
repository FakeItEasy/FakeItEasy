namespace FakeItEasy.Tests.Core
{
    using System;
    using FakeItEasy.Core;
    using FluentAssertions;
    using NUnit.Framework;

    [TestFixture]
    public class LazyFakeManagerProviderTests
    {
        [Fake]
        private FakeManager.Factory fakeManagerFactory = null;

        [Fake]
        private IFakeManagerAccessor fakeManagerAccessor = null;

        [Fake]
        private IFakeObjectConfigurator configurer = null;

        [Fake]
        private Type typeOfFake = null;

        [UnderTest]
        private LazyFakeManagerProvider lazyFakeManagerProvider = null;

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

            // Act
            var interceptionSink = this.lazyFakeManagerProvider.Fetch(this.proxy);

            // Assert
            interceptionSink.Should().BeSameAs(this.fakeManager);

            A.CallTo(() => this.fakeManagerFactory(this.typeOfFake, this.proxy)).MustHaveHappened();

            A.CallTo(() => this.fakeManagerAccessor.TagProxy(this.proxy, this.fakeManager)).MustHaveHappened();

            A.CallTo(() => this.configurer.ConfigureFake(this.typeOfFake, this.proxy)).MustHaveHappened();
        }

        [Test]
        public void Fetch_should_create_exactly_one_fake_manager()
        {
            // Arrange

            // Act
            var interceptionSink1 = this.lazyFakeManagerProvider.Fetch(this.proxy);
            var interceptionSink2 = this.lazyFakeManagerProvider.Fetch(this.proxy);
            var interceptionSink3 = this.lazyFakeManagerProvider.Fetch(this.proxy);

            // Assert
            interceptionSink1.Should().BeSameAs(interceptionSink2);
            interceptionSink2.Should().BeSameAs(interceptionSink3);

            A.CallTo(() => this.fakeManagerFactory(this.typeOfFake, this.proxy)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void EnsureInitialized_should_create_exactly_one_fake_manager()
        {
            // Arrange

            // Act
            this.lazyFakeManagerProvider.EnsureInitialized(this.proxy);
            this.lazyFakeManagerProvider.EnsureInitialized(this.proxy);
            this.lazyFakeManagerProvider.EnsureInitialized(this.proxy);

            // Assert
            A.CallTo(() => this.fakeManagerFactory(this.typeOfFake, this.proxy)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void Fetch_should_throw_if_the_wrong_proxy_object_was_specified()
        {
            // Arrange
            this.lazyFakeManagerProvider.EnsureInitialized(this.proxy);

            // Act
            Action act = () => this.lazyFakeManagerProvider.Fetch(new object());

            // Assert
            act.ShouldThrow<ArgumentException>().WithMessage("The fake manager was initialized for a different proxy.*");
        }
    }
}