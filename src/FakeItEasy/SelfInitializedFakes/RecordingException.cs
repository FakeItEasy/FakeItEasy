namespace FakeItEasy.SelfInitializedFakes
{
    using System;
#if FEATURE_BINARY_SERIALIZATION
    using System.Runtime.Serialization;
#endif

    /// <summary>
    /// An exception that can be thrown when recording for self initialized
    /// fakes fails or when playback fails.
    /// </summary>
#if FEATURE_BINARY_SERIALIZATION
    [Serializable]
#endif
    public class RecordingException
        : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RecordingException"/> class.
        /// </summary>
        public RecordingException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordingException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public RecordingException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordingException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public RecordingException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

#if FEATURE_BINARY_SERIALIZATION
        /// <summary>
        /// Initializes a new instance of the <see cref="RecordingException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="info"/> parameter is null.
        /// </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">
        /// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).
        /// </exception>
        protected RecordingException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif
    }
}
