namespace FakeItEasy.Tests
{
    using System;
    using System.Linq.Expressions;
    using FakeItEasy.Configuration;
    using Xunit;

    public class AssertConfigurationExtensionsTests
    {
        [Fact]
        public void MustHaveHappened_should_be_null_guarded()
        {
            // Arrange

            // Act

            // Assert
            Expression<Action> call = () => A.Fake<IAssertConfiguration>().MustHaveHappened();
            call.Should().BeNullGuarded();
        }

        [Fact]
        public void MustNotHaveHappened_should_be_null_guarded()
        {
            // Arrange

            // Act

            // Assert
            Expression<Action> call = () => A.Fake<IAssertConfiguration>().MustNotHaveHappened();
            call.Should().BeNullGuarded();
        }

        [Fact]
        public void MustHaveHappenedOnceExactly_should_be_null_guarded()
        {
            // Arrange

            // Act

            // Assert
            Expression<Action> call = () => A.Fake<IAssertConfiguration>().MustHaveHappenedOnceExactly();
            call.Should().BeNullGuarded();
        }

        [Fact]
        public void MustHaveHappenedOnceOrMore_should_be_null_guarded()
        {
            // Arrange

            // Act

            // Assert
            Expression<Action> call = () => A.Fake<IAssertConfiguration>().MustHaveHappenedOnceOrMore();
            call.Should().BeNullGuarded();
        }

        [Fact]
        public void MustHaveHappenedOnceOrLess_should_be_null_guarded()
        {
            // Arrange

            // Act

            // Assert
            Expression<Action> call = () => A.Fake<IAssertConfiguration>().MustHaveHappenedOnceOrLess();
            call.Should().BeNullGuarded();
        }

        [Fact]
        public void MustHaveHappenedTwiceExactly_should_be_null_guarded()
        {
            // Arrange

            // Act

            // Assert
            Expression<Action> call = () => A.Fake<IAssertConfiguration>().MustHaveHappenedTwiceExactly();
            call.Should().BeNullGuarded();
        }

        [Fact]
        public void MustHaveHappenedTwiceOrMore_should_be_null_guarded()
        {
            // Arrange

            // Act

            // Assert
            Expression<Action> call = () => A.Fake<IAssertConfiguration>().MustHaveHappenedTwiceOrMore();
            call.Should().BeNullGuarded();
        }

        [Fact]
        public void MustHaveHappenedTwiceOrLess_should_be_null_guarded()
        {
            // Arrange

            // Act

            // Assert
            Expression<Action> call = () => A.Fake<IAssertConfiguration>().MustHaveHappenedTwiceOrLess();
            call.Should().BeNullGuarded();
        }
    }
}
