namespace FakeItEasy
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// An exception thrown when exceptions occur regarding custom argument validations.
    /// </summary>
    [Serializable]
    public class ArgumentValidationException
        : Exception
    {
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public ArgumentValidationException() : base(ExceptionMessages.ArgumentValidationDefaultMessage)
        { }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="message">The message for the exception.</param>
        public ArgumentValidationException(string message)
            : base(message)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArgumentValidationException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public ArgumentValidationException(string message, Exception innerException)
            : base(message, innerException)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArgumentValidationException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="info"/> parameter is null.
        /// </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">
        /// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).
        /// </exception>
        protected ArgumentValidationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
