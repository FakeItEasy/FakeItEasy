namespace FakeItEasy
{
    using System;

    /// <summary>
    /// An exception thrown when an expectation is not met (when asserting on fake object calls).
    /// </summary>
    public class ExpectationException
        : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpectationException"/> class.
        /// </summary>
        public ExpectationException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpectationException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public ExpectationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpectationException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public ExpectationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
