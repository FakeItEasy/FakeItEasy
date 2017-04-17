namespace FakeItEasy.IntegrationTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FakeItEasy.Configuration;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xunit;

    public class ExceptionMessagesTests
    {
        [Fact]
        public void Should_give_pretty_message_when_trying_to_fake_static_method()
        {
            // Act
            var exception = Record.Exception(() => A.CallTo(() => object.Equals(null, null)));

            // Assert
            var expectedMessage =
@"

  The current proxy generator can not intercept the method System.Object.Equals(System.Object objA, System.Object objB) for the following reason:
    - Static methods can not be intercepted.

";

            exception.Should().BeAnExceptionOfType<FakeConfigurationException>().And.Message.Should().Be(expectedMessage);
        }

        [Fact]
        public void Should_give_pretty_message_when_trying_to_fake_extension_method()
        {
            // Arrange
            var fake = A.Fake<List<int>>();

            // Act
            var exception = Record.Exception(() => A.CallTo(() => fake.Any()));

            // Assert
            var expectedMessage =
@"

  The current proxy generator can not intercept the method System.Linq.Enumerable.Any<System.Int32>(System.Collections.Generic.IEnumerable<System.Int32> source) for the following reason:
    - Extension methods can not be intercepted since they're static.

";

            exception.Should().BeAnExceptionOfType<FakeConfigurationException>().And.Message.Should().Be(expectedMessage);
        }

        [Fact]
        public void Should_give_pretty_message_when_trying_to_fake_a_static_property()
        {
            // Act
            var exception = Record.Exception(() => A.CallTo(() => Environment.NewLine));

            // Assert
            var expectedMessage =
                @"

  The current proxy generator can not intercept the property System.Environment.NewLine for the following reason:
    - Static methods can not be intercepted.

";

            exception.Should().BeAnExceptionOfType<FakeConfigurationException>().And.Message.Should().Be(expectedMessage);
        }

        [Fact]
        public void Should_give_pretty_message_when_trying_to_fake_a_non_virtual_indexed_property()
        {
            // Arrange
            var fake = A.Fake<Dictionary<string, int>>();

            // Act
            var exception = Record.Exception(() => A.CallTo(() => fake["foo"]));

            // Assert
            var expectedMessage =
                @"

  The current proxy generator can not intercept the property System.Collections.Generic.Dictionary<System.String, System.Int32>.Item[System.String key] for the following reason:
    - Sealed methods can not be intercepted.

";

            exception.Should().BeAnExceptionOfType<FakeConfigurationException>().And.Message.Should().Be(expectedMessage);
        }
    }
}
