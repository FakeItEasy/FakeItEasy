namespace FakeItEasy.Tests
{
    using System;
    using System.Reflection;
    using FluentAssertions;
    using Xunit;

    public abstract class ExceptionContractTests<T> where T : Exception
    {
        [Fact]
        public void Constructor_that_takes_message_should_set_message()
        {
            // Arrange

            // Act
            var result = (T)Activator.CreateInstance(typeof(T), "A message")!;

            // Assert
            result.Message.Should().StartWith("A message");
        }

        [Fact]
        public void Constructor_that_takes_message_and_inner_exception_should_set_message()
        {
            // Arrange

            // Act
            var result = (T)Activator.CreateInstance(typeof(T), "A message", new InvalidOperationException())!;

            // Assert
            result.Message.Should().Be("A message");
        }

        [Fact]
        public void Constructor_that_takes_message_and_inner_exception_should_set_inner_exception()
        {
            // Arrange
            var innerException = new InvalidOperationException();

            // Act
            var result = (T)Activator.CreateInstance(typeof(T), string.Empty, innerException)!;

            // Assert
            result.InnerException.Should().Be(innerException);
        }

        [Fact]
        public void Constructor_without_parameters_should_work()
        {
            // Arrange

            // Act
            var result = (T)Activator.CreateInstance(typeof(T))!;

            // Assert
            result.Should().NotBeNull();
        }
    }
}
