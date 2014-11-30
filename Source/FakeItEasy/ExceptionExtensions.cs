namespace FakeItEasy
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;

    internal static class ExceptionExtensions
    {
        /// <summary>
        /// Attempts to preserve the stack trace of an existing exception for re-throwing.
        /// </summary>
        /// <remarks>Nicked from <see href="http://weblogs.asp.net/fmarguerie/archive/2008/01/02/rethrowing-exceptions-and-preserving-the-full-call-stack-trace.aspx"/>.
        /// If reduced trust context (for example) precludes invoking internal members on <see cref="Exception"/>,
        /// the stack trace will not be preserved.
        /// </remarks>
        /// <param name="exception">The exception whose stack trace needs preserving.</param>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Appropriate in try method.")]
        public static void TryPreserveStackTrace(this Exception exception)
        {
            Guard.AgainstNull(exception, "exception");

            try
            {
                typeof(Exception)
                    .GetMethod("InternalPreserveStackTrace", BindingFlags.Instance | BindingFlags.NonPublic)
                    .Invoke(exception, null);
            }
            catch
            {
            }
        }
    }
}
