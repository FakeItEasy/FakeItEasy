namespace FakeItEasy.Tests.Core
{
    using System;
    using System.Linq.Expressions;
    using FakeItEasy.Core;
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
        public void SetFakeManager_should_be_null_guarded()
        {
            // Arrange

            // Act

            // Assert
            Expression<Action> call = () => this.accessor.SetFakeManager(A.Fake<object>(), A.Fake<FakeManager>());
            call.Should().BeNullGuarded();
        }

        [Fact]
        public void GetFakeManager_should_be_null_guarded()
        {
            // Arrange

            // Act

            // Assert
            Expression<Action> call = () => this.accessor.GetFakeManager(new object());
            call.Should().BeNullGuarded();
        }

        [Fact]
        public void Should_be_able_to_set_and_retrieve_fake_manager_from_any_object()
        {
            // Arrange
            var proxy = new object();
            var fakeManager = A.Fake<FakeManager>();
            this.accessor.SetFakeManager(proxy, fakeManager);

            // Act
            var manager = this.accessor.GetFakeManager(proxy);

            // Assert
            manager.Should().BeSameAs(fakeManager);
        }

        [Fact]
        public void TryGetFakeManager_should_return_manager_set_by_SetFakeManager()
        {
            // Arrange
            var proxy = new object();
            var fakeManager = A.Fake<FakeManager>();

            // Act
            this.accessor.SetFakeManager(proxy, fakeManager);

            // Assert
            this.accessor.TryGetFakeManager(proxy).Should().BeSameAs(fakeManager);
        }

        [Fact]
        public void Should_fail_when_getting_manager_from_object_where_a_fake_manager_has_not_been_attached()
        {
            // Arrange
            var proxy = new object();

            // Act
            var exception = Record.Exception(() => this.accessor.GetFakeManager(proxy));

            // Assert
            exception.Should()
                .BeAnExceptionOfType<ArgumentException>()
                .And.Message.Should().Match("Object 'System.Object' of type System.Object* is not recognized as a fake object.");
        }
    }
}
