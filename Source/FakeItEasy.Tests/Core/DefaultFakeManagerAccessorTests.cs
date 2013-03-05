namespace FakeItEasy.Tests.Core
{
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
        public void Should_be_null_guarded()
        {
            // Arrange

            // Act

            // Assert
            NullGuardedConstraint.Assert(() =>
                this.accessor.GetFakeManager(A.Fake<ITaggable>()));
        }

        [Test]
        public void Should_be_able_to_set_and_retrieve_fake_manager_from_non_taggable_objects()
        {
            // Arrange
            var proxy = new object();
            this.accessor.AttachFakeManagerToProxy(typeof(object), proxy, A.Dummy<ICallInterceptedEventRaiser>());

            // Act
            var manager = this.accessor.GetFakeManager(proxy);

            // Assert
            Assert.That(manager, Is.SameAs(this.managerToReturnFromFactory));
        }

        [Test]
        public void Should_fail_when_getting_manager_from_object_where_a_manager_has_not_been_attached()
        {
            // Arrange
            var proxy = A.Fake<ITaggable>();
            proxy.Tag = null;

            // Act

            // Assert
            Assert.That(
                () => this.accessor.GetFakeManager(proxy),
                Throws.ArgumentException.With.Message.EqualTo("The specified object is not recognized as a fake object."));
        }
    }
}
