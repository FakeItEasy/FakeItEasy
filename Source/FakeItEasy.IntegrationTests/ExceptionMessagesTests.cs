namespace FakeItEasy.IntegrationTests
{
    using System.Collections.Generic;
    using System.Linq;
    using FakeItEasy.Configuration;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using NUnit.Framework;

    [TestFixture]
    public class ExceptionMessagesTests
    {
        [Test]
        public void Should_give_pretty_message_when_trying_to_fake_static_method()
        {
            // Act
            var exception = Record.Exception(() => A.CallTo(() => object.Equals(null, null)));

            // Assert
            var expectedMessage =
@"

  The current proxy generator can not intercept the specified method for the following reason:
    - Static methods can not be intercepted.

";

            exception.Should().BeAnExceptionOfType<FakeConfigurationException>().And.Message.Should().Be(expectedMessage);
        }

        [Test]
        public void Should_give_pretty_message_when_trying_to_fake_extension_method()
        {
            // Arrange
            var fake = A.Fake<List<int>>();

            // Act
            var exception = Record.Exception(() => A.CallTo(() => fake.Any()));

            // Assert
            var expectedMessage =
@"

  The current proxy generator can not intercept the specified method for the following reason:
    - Extension methods can not be intercepted since they're static.

";

            exception.Should().BeAnExceptionOfType<FakeConfigurationException>().And.Message.Should().Be(expectedMessage);
        }
    }
}
