namespace FakeItEasy.Tests
{
    using System;
    using System.Reflection;
    using System.Runtime.Serialization;
    using FluentAssertions;
    using Xunit;

    public abstract class ExceptionContractTests<T> where T : Exception
    {
        [Fact]
        public void Exception_should_provide_serialization_constructor()
        {
            // Arrange

            // Act
            var constructor = typeof(T).GetConstructor(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(SerializationInfo), typeof(StreamingContext) }, null);

            // Assert
            constructor.Should().NotBeNull("exception classes should implement a constructor serialization constructor");
            constructor.IsPublic.Should().BeFalse("serialization constructor should be protected, not public");
            constructor.IsPrivate.Should().BeFalse("serialization constructor should be protected, not private");
        }

        [Fact]
        public void Exception_should_provide_message_only_constructor()
        {
            // Arrange

            // Act
            var constructor = this.GetMessageOnlyConstructor();

            // Assert
            constructor.Should().NotBeNull("Exception classes should provide a public message only constructor.");
        }

        [Fact]
        public void Constructor_that_takes_message_should_set_message()
        {
            // Arrange
            var constructor = this.GetMessageOnlyConstructor();

            // Act
            var result = (T)constructor.Invoke(new object[] { "A message" });

            // Assert
            result.Message.Should().StartWith("A message");
        }

        [Fact]
        public void Exception_should_be_serializable()
        {
            this.CreateException().Should().BeBinarySerializable();
        }

        [Fact]
        public void Exception_should_provide_message_and_inner_exception_constructor()
        {
            // Arrange
            var constructor = this.GetMessageAndInnerExceptionConstructor();

            // Act

            // Assert
            constructor.Should().NotBeNull("exception classes should provide a public constructor that takes message and inner exception.");
        }

        [Fact]
        public void Constructor_that_takes_message_and_inner_exception_should_set_message()
        {
            // Arrange
            var constructor = this.GetMessageAndInnerExceptionConstructor();

            // Act
            var result = (T)constructor.Invoke(new object[] { "A message", new InvalidOperationException() });

            // Assert
            result.Message.Should().Be("A message");
        }

        [Fact]
        public void Constructor_that_takes_message_and_inner_exception_should_set_inner_exception()
        {
            // Arrange
            var constructor = this.GetMessageAndInnerExceptionConstructor();
            var innerException = new InvalidOperationException();

            // Act
            var result = (T)constructor.Invoke(new object[] { string.Empty, innerException });

            // Assert
            result.InnerException.Should().Be(innerException);
        }

        [Fact]
        public void Exception_should_provide_default_constructor()
        {
            // Arrange
            var constructor = typeof(T).GetConstructor(new Type[] { });

            // Act
            constructor.Invoke(new object[] { });

            // Assert
            constructor.Should().NotBeNull("exception classes should provide a public default constructor.");
        }

        protected abstract T CreateException();

        private ConstructorInfo GetMessageAndInnerExceptionConstructor()
        {
            return typeof(T).GetConstructor(new[] { typeof(string), typeof(Exception) });
        }

        private ConstructorInfo GetMessageOnlyConstructor()
        {
            return typeof(T).GetConstructor(new[] { typeof(string) });
        }
    }
}
