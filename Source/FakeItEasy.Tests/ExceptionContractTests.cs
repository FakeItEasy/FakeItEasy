using System;
using System.Reflection;
using System.Runtime.Serialization;
using NUnit.Framework;

namespace FakeItEasy.Tests
{
    public abstract class ExceptionContractTests<T> where T : Exception
    {
        private T exception;

        [SetUp]
        public void SetUp()
        {
            this.exception = this.CreateException();
        }

        protected abstract T CreateException();

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
            var exception = (T)constructor.Invoke(new object[] { "A message" });

            // Assert
            Assert.That(exception.Message, Text.StartsWith("A message"));
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
            var exception = (T)constructor.Invoke(new object[] { "A message", new Exception() });

            // Assert
            Assert.That(exception.Message, Is.EqualTo("A message"));
        }

        [Test]
        public void Constructor_that_takes_message_and_inner_exception_should_set_inner_exception()
        {
            // Arrange
            var constructor = this.GetMessageAndInnerExceptionConstructor();
            var innerException = new Exception();

            // Act
            var exception = (T)constructor.Invoke(new object[] { "", innerException });

            // Assert
            Assert.That(exception.InnerException, Is.EqualTo(innerException));
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

        private ConstructorInfo GetMessageAndInnerExceptionConstructor()
        {
            return typeof(T).GetConstructor(new[] { typeof(string), typeof(Exception) });
        }

        private ConstructorInfo GetMessageOnlyConstructor()
        {
            return typeof(T).GetConstructor(new[] { typeof(string) });
        }
    }

    [Serializable]
    public class DummyException
        : Exception
    {
        public DummyException()
        {

        }

        public DummyException(string message)
            : base(message)
        { 
        
        }

        public DummyException(string message, Exception innerException)
            : base(message, innerException)
        { }

        protected DummyException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            
        }
    }
}
