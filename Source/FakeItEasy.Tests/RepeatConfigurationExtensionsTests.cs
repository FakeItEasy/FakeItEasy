namespace FakeItEasy.Tests
{
    using System;
    using FakeItEasy.Configuration;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using NUnit.Framework;

    [TestFixture]
    public class RepeatConfigurationExtensionsTests : ConfigurableServiceLocatorTestBase
    {
        [Test]
        public void Once_should_call_NumberOfTimes_with_1_as_argument()
        {
            // Arrange
            var repeatConfig = A.Fake<IRepeatConfiguration>();

            // Act
            repeatConfig.Once();

            // Assert
            A.CallTo(() => repeatConfig.NumberOfTimes(1)).MustHaveHappened();
        }

        [Test]
        public void Once_should_throw_when_configuration_is_null()
        {
            // Arrange
            IRepeatConfiguration repeatConfig = null;

            // Act
            var exception = Record.Exception(() => repeatConfig.Once());

            // Assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Test]
        public void Twice_should_call_NumberOfTimes_with_2_as_argument()
        {
            // Arrange
            var repeatConfig = A.Fake<IRepeatConfiguration>();

            // Act
            repeatConfig.Twice();

            // Assert
            A.CallTo(() => repeatConfig.NumberOfTimes(2)).MustHaveHappened();
        }

        [Test]
        public void Twice_should_throw_when_configuration_is_null()
        {
            // Arrange
            IRepeatConfiguration repeatConfig = null;

            // Act
            var exception = Record.Exception(() => repeatConfig.Twice());

            // Assert
            exception.Should().BeOfType<ArgumentNullException>();
        }
    }
}
