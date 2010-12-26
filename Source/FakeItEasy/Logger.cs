namespace FakeItEasy
{
    using System;
    using System.Diagnostics;

    internal abstract class Logger
    {
        /// <summary>
        /// Writes the specified message to the logger.
        /// </summary>
        /// <param name="message">The message to write.</param>
        [Conditional("DEBUG")]
        public abstract void Debug(Func<string> message);
    }
}