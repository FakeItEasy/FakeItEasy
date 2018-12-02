namespace FakeItEasy.Tests
{
    using System;
    using FakeItEasy.Configuration;
    using FluentAssertions;
    using Xunit;

    public class BehaviorLifetimeConfigurationExtensionsTests
    {
        [Fact]
        public void Once_should_call_NumberOfTimes_with_1_as_argument()
        {
            // Arrange
            var behaviorLifetimeConfiguration = A.Fake<IBehaviorLifetimeConfiguration<IVoidConfiguration>>();

            // Act
            behaviorLifetimeConfiguration.Once();

            // Assert
            A.CallTo(() => behaviorLifetimeConfiguration.NumberOfTimes(1)).MustHaveHappened();
        }

        [Fact]
        public void Once_should_throw_when_configuration_is_null()
        {
            // Arrange
            IBehaviorLifetimeConfiguration<IVoidConfiguration> behaviorLifetimeConfiguration = null;

            // Act
            var exception = Record.Exception(() => behaviorLifetimeConfiguration.Once());

            // Assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Twice_should_call_NumberOfTimes_with_2_as_argument()
        {
            // Arrange
            var behaviorLifetimeConfiguration = A.Fake<IBehaviorLifetimeConfiguration<IVoidConfiguration>>();

            // Act
            behaviorLifetimeConfiguration.Twice();

            // Assert
            A.CallTo(() => behaviorLifetimeConfiguration.NumberOfTimes(2)).MustHaveHappened();
        }

        [Fact]
        public void Twice_should_throw_when_configuration_is_null()
        {
            // Arrange
            IBehaviorLifetimeConfiguration<IVoidConfiguration> behaviorLifetimeConfiguration = null;

            // Act
            var exception = Record.Exception(() => behaviorLifetimeConfiguration.Twice());

            // Assert
            exception.Should().BeOfType<ArgumentNullException>();
        }
    }
}
