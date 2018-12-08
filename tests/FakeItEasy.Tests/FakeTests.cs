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
    }
}
