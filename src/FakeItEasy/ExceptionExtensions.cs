namespace FakeItEasy
{
    using System;
#if FEATURE_EXCEPTION_DISPATCH_INFO
    using System.Runtime.ExceptionServices;
#else
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
#endif

    internal static class ExceptionExtensions
    {
#if !FEATURE_EXCEPTION_DISPATCH_INFO
        private static readonly Action<Exception> PreserveStackTrace = CreatePreserveStackTrace();
#endif

        /// <summary>
        /// Re-throws an exception, trying to preserve its stack trace.
        /// </summary>
        /// <param name="exception">The exception to rethrow.</param>
#if FEATURE_EXCEPTION_DISPATCH_INFO
        public static void Rethrow(this Exception exception)
        {
            ExceptionDispatchInfo.Capture(exception).Throw();
        }
#else
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Appropriate in this case, since we can't do anything but ignore the exception.")]
        public static void Rethrow(this Exception exception)
        {
            try
            {
                PreserveStackTrace(exception);
            }
            catch
            {
            }

            throw exception;
        }

        private static Action<Exception> CreatePreserveStackTrace()
        {
            var method = typeof(Exception).GetMethod(
                "InternalPreserveStackTrace",
                BindingFlags.Instance | BindingFlags.NonPublic);
            return (Action<Exception>)Delegate.CreateDelegate(typeof(Action<Exception>), null, method);
        }
#endif
    }
}
