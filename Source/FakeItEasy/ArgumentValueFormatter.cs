namespace FakeItEasy
{
    using System;
    /// <summary>
    /// Provides string formatting for arguments of type T when written in 
    /// call lists.
    /// </summary>
    public abstract class ArgumentValueFormatter<T>
        : IArgumentValueFormatter
    {
        /// <summary>
        /// The type of arguments this formatter works on.
        /// </summary>
        /// <value></value>
        public Type ForType
        {
            get { return typeof(T); }
        }

        /// <summary>
        /// The priority of the formatter, when two formatters are
        /// registered for the same type the one with the highest
        /// priority is used.
        /// </summary>
        /// <value></value>
        public virtual int Priority
        {
            get { return int.MinValue; }
        }

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
