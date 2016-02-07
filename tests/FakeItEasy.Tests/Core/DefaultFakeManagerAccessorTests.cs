namespace FakeItEasy.Tests.Core
{
    using FakeItEasy.Core;
    using FakeItEasy.Creation;
    using NUnit.Framework;

    [TestFixture]
    public class DefaultFakeManagerAccessorTests
    {
        private DefaultFakeManagerAccessor accessor;

        [SetUp]
        public void Setup()
        {
            this.accessor = new DefaultFakeManagerAccessor();
        }

        [Test]
        public void TagProxy_should_be_null_guarded()
        {
            // Arrange

            // Act

            // Assert
            NullGuardedConstraint.Assert(() =>
                this.accessor.TagProxy(A.Fake<object>(), A.Fake<FakeManager>()));
        }

        [Test]
        public void Should_set_fake_manager_to_tag()
        {
            // Arrange
            var proxy = A.Fake<ITaggable>();
            var fakeManager = A.Fake<FakeManager>();

            // Act
            this.accessor.TagProxy(proxy, fakeManager);

            // Assert
            Assert.That(proxy.Tag, Is.SameAs(fakeManager));
        }

        [Test]
        public void Should_get_fake_manager_from_tag()
        {
            // Arrange
            var proxy = A.Fake<ITaggable>();
            var fakeManager = A.Fake<FakeManager>();
            proxy.Tag = fakeManager;

            // Act
            var result = this.accessor.GetFakeManager(proxy);

            // Assert
            Assert.That(result, Is.SameAs(fakeManager));
        }

        [Test]
        public void GetFakeManager_should_be_null_guarded()
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
            var fakeManager = A.Fake<FakeManager>();
            this.accessor.TagProxy(proxy, fakeManager);

            // Act
            var manager = this.accessor.GetFakeManager(proxy);

            // Assert
            Assert.That(manager, Is.SameAs(fakeManager));
        }

        [Test]
        public void Should_fail_when_getting_manager_from_object_where_a_fake_manager_has_not_been_attached()
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

        [Test]
        public void Should_fail_when_getting_manager_from_object_where_a_wrong_typed_fake_manager_has_been_attached()
        {
            // Arrange
            var proxy = A.Fake<ITaggable>();
            proxy.Tag = new object();

            // Act

            // Assert
            Assert.That(
                () => this.accessor.GetFakeManager(proxy),
                Throws.ArgumentException.With.Message.EqualTo("The specified object is not recognized as a fake object."));
        }
    }
}
