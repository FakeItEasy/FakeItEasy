namespace FakeItEasy
{
    using System;
#if FEATURE_NETCORE_REFLECTION
    using System.Reflection;
#endif

    /// <summary>
    /// Provides string formatting for arguments of type T when written in call lists.
    /// </summary>
    /// <typeparam name="T">The type of the arguments which will be formatted by this instance.</typeparam>
    public abstract class ArgumentValueFormatter<T>
        : IArgumentValueFormatter
    {
        /// <summary>
        /// Gets the type of arguments this formatter works on.
        /// </summary>
        public Type ForType => typeof(T);

        /// <summary>
        /// Gets the priority of the formatter, when two formatters are
        /// registered for the same type the one with the highest priority value is used.
        /// </summary>
        /// <remarks>The default implementation returns <see cref="FakeItEasy.Priority.Default"/>.</remarks>
        public virtual Priority Priority => Priority.Default;

        /// <summary>
        /// Gets a string representing the specified argument value.
        /// </summary>
        /// <param name="argumentValue">The argument value to get as a string.</param>
        /// <returns>A string representation of the value.</returns>
        public string GetArgumentValueAsString(object argumentValue)
        {
            return this.GetStringValue((T)argumentValue);
        }

        /// <summary>
        /// Gets a string representing the specified argument value.
        /// </summary>
        /// <param name="argumentValue">The argument value to get as a string.</param>
        /// <returns>A string representation of the value.</returns>
        protected abstract string GetStringValue(T argumentValue);
    }
}
