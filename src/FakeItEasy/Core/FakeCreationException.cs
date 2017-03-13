namespace FakeItEasy.Core
{
    using System;
#if FEATURE_BINARY_SERIALIZATION
    using System.Runtime.Serialization;
#endif

    /// <summary>
    /// An exception that is thrown when there was an error creating a fake object.
    /// </summary>
#if FEATURE_BINARY_SERIALIZATION
    [Serializable]
#endif
    public class FakeCreationException
        : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FakeCreationException"/> class.
        /// </summary>
        public FakeCreationException()
            : base(ExceptionMessages.FakeCreationExceptionDefault)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FakeCreationException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public FakeCreationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FakeCreationException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public FakeCreationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

#if FEATURE_BINARY_SERIALIZATION
        /// <summary>
        /// Initializes a new instance of the <see cref="FakeCreationException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="info"/> parameter is null.
        /// </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">
        /// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).
        /// </exception>
        protected FakeCreationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
#endif
    }
}
