namespace FakeItEasy
{
    using System;

    internal interface ILogger
    {
        /// <summary>
        /// Writes the specified message to the logger.
        /// </summary>
        /// <param name="logger">The logger to write to.</param>
        /// <param name="message">The message to write.</param>
        void Debug(Func<string> message);
    }
}
