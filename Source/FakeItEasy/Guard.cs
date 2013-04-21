namespace FakeItEasy
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Provides methods for guarding method arguments.
    /// </summary>
    internal static class Guard
    {
        /// <summary>
        /// Throws an exception if the specified argument is null.
        /// </summary>
        /// <param name="argument">The argument.</param>
        /// <param name="argumentName">Name of the argument.</param>
        /// <exception cref="ArgumentNullException">The specified argument was null.</exception>
        [DebuggerStepThrough]
        public static void AgainstNull(object argument, string argumentName)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(argumentName);
            }
        }
    }
}