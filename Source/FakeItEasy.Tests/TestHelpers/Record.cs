namespace FakeItEasy.Tests.TestHelpers
{
    using System;

    /// <summary>
    /// Records actions for a test.
    /// </summary>
    internal static class Record
    {
        /// <summary>
        /// Invokes <paramref name="action"/> and records any exception that's raised.
        /// </summary>
        /// <param name="action">The action to invoke.</param>
        /// <returns>The captured exception, or <c>null</c> if no exception was raised.</returns>
        public static Exception Exception(Action action)
        {
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
    }
}