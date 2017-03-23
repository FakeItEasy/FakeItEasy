namespace FakeItEasy.Tests
{
    using System;
    using FakeItEasy.Configuration;
    using FluentAssertions;
    using Xunit;

    public class RepeatConfigurationExtensionsTests
    {
        [Fact]
        public void Once_should_call_NumberOfTimes_with_1_as_argument()
        {
            // Arrange
            var repeatConfig = A.Fake<IRepeatConfiguration<IVoidConfiguration>>();

            // Act
            repeatConfig.Once();

            // Assert
            A.CallTo(() => repeatConfig.NumberOfTimes(1)).MustHaveHappened();
        }

        [Fact]
        public void Once_should_throw_when_configuration_is_null()
        {
            // Arrange
            IRepeatConfiguration<IVoidConfiguration> repeatConfig = null;

            // Act
            var exception = Record.Exception(() => repeatConfig.Once());

            // Assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Twice_should_call_NumberOfTimes_with_2_as_argument()
        {
            // Arrange
            var repeatConfig = A.Fake<IRepeatConfiguration<IVoidConfiguration>>();

            // Act
            repeatConfig.Twice();

            // Assert
            A.CallTo(() => repeatConfig.NumberOfTimes(2)).MustHaveHappened();
        }

        [Fact]
        public void Twice_should_throw_when_configuration_is_null()
        {
            // Arrange
            IRepeatConfiguration<IVoidConfiguration> repeatConfig = null;

            // Act
            var exception = Record.Exception(() => repeatConfig.Twice());

            // Assert
            exception.Should().BeOfType<ArgumentNullException>();
        }
    }
}
