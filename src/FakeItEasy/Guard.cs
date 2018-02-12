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
        public static void AgainstNull([ValidatedNotNull]object argument, string argumentName)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(argumentName);
            }
        }

        /// <summary>
        /// When applied to a parameter, this attribute provides an indication to code analysis that the argument has been null checked.
        /// </summary>
        [AttributeUsage(AttributeTargets.Parameter)]
        private sealed class ValidatedNotNullAttribute : Attribute
        {
        }
    }
}
