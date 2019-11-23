using FakeItEasy.Core;
using FluentAssertions;

namespace FakeItEasy.Tests
{
    using System;
    using System.Linq.Expressions;
    using Xunit;

    public class FakeTests
    {
        [Fact]
        public void GetFakeManager_should_be_guarded()
        {
            // Arrange

            // Act

            // Assert
            Expression<Action> call = () => Fake.GetFakeManager(A.Dummy<IFoo>());
            call.Should().BeNullGuarded();
        }

        [Fact]
        public void GetCalls_should_be_guarded()
        {
            // Arrange

            // Act

            // Assert
            Expression<Action> call = () => Fake.GetCalls(A.Dummy<object>());
            call.Should().BeNullGuarded();
        }

        [Fact]
        public void ClearConfiguration_should_be_guarded()
        {
            // Arrange

            // Act

            // Assert
            Expression<Action> call = () => Fake.ClearConfiguration(A.Dummy<object>());
            call.Should().BeNullGuarded();
        }

        [Fact]
        public void ClearRecordedCalls_should_be_guarded()
        {
            // Arrange

            // Act

            // Assert
            Expression<Action> call = () => Fake.ClearRecordedCalls(A.Dummy<object>());
            call.Should().BeNullGuarded();
        }

        [Fact]
        public void TryGetFakeManager_should_be_guarded()
        {
            // Arrange

            // Act

            // Assert
            FakeManager _;
            Expression<Action> call = () => Fake.TryGetFakeManager(A.Dummy<object>(), out _);
            call.Should().BeNullGuarded();
        }

        [Fact]
        public void TryGetFakeManager_return_true_for_existing_fakemanager()
        {
            // Arrange
            var aFake = A.Fake<object>();

            // Act
            var result = Fake.TryGetFakeManager(aFake, out var manager);

            // Assert
            result.Should().BeTrue();
            manager.Should().NotBe(null);
        }

        [Fact]
        public void TryGetFakeManager_return_false_for_no_existing_fakemanager()
        {
            // Arrange
            var notAFake = new object();

            // Act
            var result = Fake.TryGetFakeManager(notAFake, out var manager);

            // Assert
            result.Should().BeFalse();
            manager.Should().Be(null);
        }

        [Fact]
        public void IsFake_return_true_for_fake()
        {
            // Arrange
            var aFake = A.Fake<object>();

            // Act
            var result = Fake.IsFake(aFake);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsFake_return_false_for_non_fake()
        {
            // Arrange
            var notAFake = new object();

            // Act
            var result = Fake.IsFake(notAFake);

            // Assert
            result.Should().BeFalse();
        }
    }
}
