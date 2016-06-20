namespace FakeItEasy.Tests.Core
{
    using FakeItEasy.Core;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xunit;

    public class FakeCreationExceptionTests
        : ExceptionContractTests<FakeCreationException>
    {
        [Fact]
        [UsingCulture("en-US")]
        public void DefaultConstructor_should_set_correct_error_message()
        {
            // Arrange
            var exception = new FakeCreationException();

            // Act

            // Assert
            exception.Message.Should().Be("Unable to create fake object.");
        }

        protected override FakeCreationException CreateException()
        {
            return new FakeCreationException();
        }
    }
}
