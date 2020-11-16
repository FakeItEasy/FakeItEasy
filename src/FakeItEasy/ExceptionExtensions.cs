namespace FakeItEasy
{
    using System;
    using System.Runtime.ExceptionServices;

    internal static class ExceptionExtensions
    {
        /// <summary>
        /// Re-throws an exception, trying to preserve its stack trace.
        /// </summary>
        /// <param name="exception">The exception to rethrow.</param>
        public static void Rethrow(this Exception exception)
        {
            ExceptionDispatchInfo.Capture(exception).Throw();
        }
    }
}
