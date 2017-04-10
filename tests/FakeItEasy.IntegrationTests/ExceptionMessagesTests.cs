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

  The current proxy generator can not intercept the method System.Linq.Enumerable.Any`1[System.Int32](System.Collections.Generic.IEnumerable`1[System.Int32] source) for the following reason:
    - Extension methods can not be intercepted since they're static.

";

            exception.Should().BeAnExceptionOfType<FakeConfigurationException>().And.Message.Should().Be(expectedMessage);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "astatic", Justification = "Refers to the two words 'a static'.")]
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
        public void Should_give_pretty_message_when_trying_to_fake_an_indexed_property_that_is_not_virtual()
        {
            // Arrange
            var fake = A.Fake<Dictionary<string, int>>();

            // Act
            var exception = Record.Exception(() => A.CallTo(() => fake["foo"]));

            // Assert
            var expectedMessage =
                @"

  The current proxy generator can not intercept the property System.Collections.Generic.Dictionary`2[System.String,System.Int32].Item[System.String key] for the following reason:
    - Non-virtual members can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.

";

            exception.Should().BeAnExceptionOfType<FakeConfigurationException>().And.Message.Should().Be(expectedMessage);
        }
    }
}
