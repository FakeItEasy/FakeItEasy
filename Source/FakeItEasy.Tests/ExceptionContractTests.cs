namespace FakeItEasy.Tests
{
    using System;
    using System.Reflection;
    using System.Runtime.Serialization;
    using NUnit.Framework;

    public abstract class ExceptionContractTests<T> where T : Exception
    {
        private T exception;

        [SetUp]
        public void Setup()
        {
            this.exception = this.CreateException();
        }

        [Test]
        public void Exception_should_provide_serialization_constructor()
        {
            // Arrange

            // Act
            var constructor = typeof(T).GetConstructor(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(SerializationInfo), typeof(StreamingContext) }, null);

            // Assert
            Assert.That(constructor, Is.Not.Null, "Exception classes should implement a constructor serialization constructor");
            Assert.That(!constructor.IsPublic && !constructor.IsPrivate, "Serialization constructor should be protected.");
        }

        [Test]
        public void Exception_should_provide_message_only_constructor()
        {
            // Arrange

            // Act
            var constructor = this.GetMessageOnlyConstructor();

            // Assert
            Assert.That(constructor, Is.Not.Null, "Exception classes should provide a public message only constructor.");
        }

        [Test]
        public void Constructor_that_takes_message_should_set_message()
        {
            // Arrange
            var constructor = this.GetMessageOnlyConstructor();

            // Act
            var result = (T)constructor.Invoke(new object[] { "A message" });

            // Assert
            Assert.That(result.Message, Is.StringStarting("A message"));
        }

        [Test]
        public void Exception_should_be_serializable()
        {
            Assert.That(this.exception, Is.BinarySerializable);
        }

        [Test]
        public void Exception_should_provide_message_and_inner_exception_constructor()
        {
            // Arrange
            var constructor = this.GetMessageAndInnerExceptionConstructor();

            // Act

            // Assert
            Assert.That(constructor, Is.Not.Null, "Exception classes should provide a public constructor that takes message and inner exception.");
        }

        [Test]
        public void Constructor_that_takes_message_and_inner_exception_should_set_message()
        {
            // Arrange
            var constructor = this.GetMessageAndInnerExceptionConstructor();

            // Act
            var result = (T)constructor.Invoke(new object[] { "A message", new InvalidOperationException() });

            // Assert
            Assert.That(result.Message, Is.EqualTo("A message"));
        }

        [Test]
        public void Constructor_that_takes_message_and_inner_exception_should_set_inner_exception()
        {
            // Arrange
            var constructor = this.GetMessageAndInnerExceptionConstructor();
            var innerException = new InvalidOperationException();

            // Act
            var result = (T)constructor.Invoke(new object[] { string.Empty, innerException });

            // Assert
            Assert.That(result.InnerException, Is.EqualTo(innerException));
        }

        [Test]
        public void Exception_should_provide_default_constructor()
        {
            // Arrange
            var constructor = typeof(T).GetConstructor(new Type[] { });

            // Act
            constructor.Invoke(new object[] { });

            // Assert
            Assert.That(constructor, Is.Not.Null, "Exception classes should provide a public default constructor.");
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