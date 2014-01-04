namespace FakeItEasy.Tests
{
    using System;
    using FakeItEasy.Configuration;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using NUnit.Framework;

    [TestFixture]
    public class OnceExtensionsTests : ConfigurableServiceLocatorTestBase
    {
        [Test]
        public void Should_call_NumberOfTimes_with_1_as_argument()
        {
            // Arrange
            var repeatConfig = A.Fake<IRepeatConfiguration>();

            // Act
            repeatConfig.Once();

            // Assert
            A.CallTo(() => repeatConfig.NumberOfTimes(1)).MustHaveHappened();
        }

        [Test]
        public void Should_throw_when_configuration_is_null()
        {
            // Arrange
            IRepeatConfiguration repeatConfig = null;

            // Act
            var exception = Record.Exception(() => repeatConfig.Once());

            // Assert
            exception.Should().BeOfType<ArgumentNullException>();
        }
    }
}
