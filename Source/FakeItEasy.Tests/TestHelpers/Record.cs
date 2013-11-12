namespace FakeItEasy.Tests.TestHelpers
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Records actions for a test.
    /// </summary>
    public static class Record
    {
        /// <summary>
        /// Invokes <paramref name="action"/> and records any exception that's raised.
        /// </summary>
        /// <param name="action">The action to invoke.</param>
        /// <returns>The captured exception, or <c>null</c> if no exception was raised.</returns>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Required for testing.")]
        public static Exception Exception(Action action)
        {
            Guard.AgainstNull(action, "action");

            try
            {
                action();
                return null;
            }
            catch (Exception e)
            {
                return e;
            }
        }

        /// <summary>
        /// Invokes <paramref name="function"/> and records any exception that's raised.
        /// </summary>
        /// <param name="function">The function to invoke.</param>
        /// <returns>The captured exception, or <c>null</c> if no exception was raised.</returns>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Required for testing.")]
        public static Exception Exception(Func<object> function)
        {
            Guard.AgainstNull(function, "function");

            try
            {
                function();
                return null;
            }
            catch (Exception e)
            {
                return e;
            }
        }
    }
}