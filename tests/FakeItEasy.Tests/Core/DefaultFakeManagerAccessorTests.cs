namespace FakeItEasy.Tests.Core
{
    using System;
    using System.Linq.Expressions;
    using FakeItEasy.Core;
    using FakeItEasy.Creation;
    using FakeItEasy.Tests;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xunit;

    public class DefaultFakeManagerAccessorTests
    {
        private readonly DefaultFakeManagerAccessor accessor;

        public DefaultFakeManagerAccessorTests()
        {
            this.accessor = new DefaultFakeManagerAccessor();
        }

        [Fact]
        public void TagProxy_should_be_null_guarded()
        {
            // Arrange

            // Act

            // Assert
            Expression<Action> call = () => this.accessor.TagProxy(A.Fake<object>(), A.Fake<FakeManager>());
            call.Should().BeNullGuarded();
        }

        [Fact]
        public void Should_set_fake_manager_to_tag()
        {
            // Arrange
            var proxy = A.Fake<ITaggable>();
            var fakeManager = A.Fake<FakeManager>();

            // Act
            this.accessor.TagProxy(proxy, fakeManager);

            // Assert
            proxy.Tag.Should().BeSameAs(fakeManager);
        }

        [Fact]
        public void Should_get_fake_manager_from_tag()
        {
            // Arrange
            var proxy = A.Fake<ITaggable>();
            var fakeManager = A.Fake<FakeManager>();
            proxy.Tag = fakeManager;

            // Act
            var result = this.accessor.GetFakeManager(proxy);

            // Assert
            result.Should().BeSameAs(fakeManager);
        }

        [Fact]
        public void GetFakeManager_should_be_null_guarded()
        {
            // Arrange

            // Act

            // Assert
            Expression<Action> call = () => this.accessor.GetFakeManager(A.Fake<ITaggable>());
            call.Should().BeNullGuarded();
        }

        [Fact]
        public void Should_be_able_to_set_and_retrieve_fake_manager_from_non_taggable_objects()
        {
            // Arrange
            var proxy = new object();
            var fakeManager = A.Fake<FakeManager>();
            this.accessor.TagProxy(proxy, fakeManager);

            // Act
            var manager = this.accessor.GetFakeManager(proxy);

            // Assert
            manager.Should().BeSameAs(fakeManager);
        }

        [Fact]
        public void Should_fail_when_getting_manager_from_object_where_a_fake_manager_has_not_been_attached()
        {
            // Arrange
            var proxy = A.Fake<ITaggable>();
            proxy.Tag = null;

            // Act
            var exception = Record.Exception(() => this.accessor.GetFakeManager(proxy));

            // Assert
            exception.Should()
                .BeAnExceptionOfType<ArgumentException>()
                .WithMessage("The specified object is not recognized as a fake object.");
        }

        [Fact]
        public void Should_fail_when_getting_manager_from_object_where_a_wrong_typed_fake_manager_has_been_attached()
        {
            // Arrange
            var proxy = A.Fake<ITaggable>();
            proxy.Tag = new object();

            // Act
            var exception = Record.Exception(() => this.accessor.GetFakeManager(proxy));

            // Assert
            exception.Should()
                .BeAnExceptionOfType<ArgumentException>()
                .WithMessage("The specified object is not recognized as a fake object.");
        }
    }
}
