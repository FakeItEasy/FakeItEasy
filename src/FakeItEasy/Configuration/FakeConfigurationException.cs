namespace FakeItEasy.Configuration
{
    using System;

    /// <summary>
    /// An exception that can be thrown when something goes wrong with the configuration
    /// of a fake object.
    /// </summary>
    public class FakeConfigurationException
        : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FakeConfigurationException"/> class.
        /// </summary>
        public FakeConfigurationException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FakeConfigurationException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public FakeConfigurationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FakeConfigurationException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public FakeConfigurationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
