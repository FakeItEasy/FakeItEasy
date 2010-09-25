namespace FakeItEasy.Tests.Core
{
    using System;
    using FakeItEasy.Core;
    using FakeItEasy.Creation;
    using NUnit.Framework;

    [TestFixture]
    public class DefaultFakeManagerAccessorTests
    {
        private DefaultFakeManagerAccessor accessor;
        private FakeManager.Factory managerFactory;
        private FakeManager managerToReturnFromFactory;

        [SetUp]
        public void SetUp()
        {
            this.managerFactory = () => this.managerToReturnFromFactory;
            this.managerToReturnFromFactory = A.Fake<FakeManager>();

            this.accessor = new DefaultFakeManagerAccessor(this.managerFactory);
        }

        [Test]
        public void Should_set_manager_from_proxy_to_tag()
        {
            // Arrange
            var proxy = A.Fake<ITaggable>();
            
            // Act
            this.accessor.AttachFakeManagerToProxy(typeof(object), proxy, A.Dummy<ICallInterceptedEventRaiser>());

            // Assert
            Assert.That(proxy.Tag, Is.EqualTo(this.managerToReturnFromFactory));
        }

        [Test]
        [SetCulture("en-US")]
        public void Should_throw_when_proxy_is_not_taggable()
        {
            // Arrange
            var proxy = new object();

            // Act, Assert
            Assert.That(() =>
            {
                this.accessor.AttachFakeManagerToProxy(typeof(object), proxy, A.Dummy<ICallInterceptedEventRaiser>());
            },
            Throws.Exception.InstanceOf<ArgumentException>()
                .With.Message.EqualTo("The specified object is not recognized as a fake."));
        }

        [Test]
        public void Should_set_proxy_and_event_raiser_to_manager()
        {
            // Arrange
            this.managerToReturnFromFactory = A.Fake<FakeManager>();

            var proxy = A.Dummy<ITaggable>();
            var eventRaiser = A.Dummy<ICallInterceptedEventRaiser>();

            // Act
            this.accessor.AttachFakeManagerToProxy(typeof(object), proxy, eventRaiser);

            // Assert
            A.CallTo(() => this.managerToReturnFromFactory.AttachProxy(typeof(object), proxy, eventRaiser)).MustHaveHappened();
        }

        [Test]
        public void Should_get_fake_manager_from_tag()
        {
            // Arrange
            var proxy = A.Fake<ITaggable>();
            proxy.Tag = this.managerToReturnFromFactory;

            // Act
            var result = this.accessor.GetFakeManager(proxy);

            // Assert
            Assert.That(result, Is.SameAs(this.managerToReturnFromFactory));
        }

        [Test]
        public void Should_throw_when_proxy_is_not_taggable_when_getting_fake_manager()
        {
            // Arrange
            var proxy = new object();

            // Act, Assert
            Assert.That(() =>
            {
                this.accessor.GetFakeManager(proxy);
            },
            Throws.Exception.InstanceOf<ArgumentException>()
                .With.Message.EqualTo("The specified object is not recognized as a fake."));
        }

        [Test]
        public void Should_be_null_guarded()
        {
            // Arrange

            // Act

            // Assert
            NullGuardedConstraint.Assert(() =>
                this.accessor.GetFakeManager(A.Fake<ITaggable>()));
        }
    }
}
