namespace FakeItEasy
{
    using System;
    
    /// <summary>
    /// Provides string formatting for arguments when written in 
    /// call lists.
    /// </summary>
    public interface IArgumentValueFormatter
    {
        /// <summary>
        /// Gets the type of arguments this formatter works on.
        /// </summary>
        Type ForType { get; }

        /// <summary>
        /// Gets the priority of the formatter, when two formatters are
        /// registered for the same type the one with the highest
        /// priority is used.
        /// </summary>
        int Priority { get; }

        /// <summary>
        /// Gets a string representing the specified argument value.
        /// </summary>
        /// <param name="argumentValue">The argument value to get as a string.</param>
        /// <returns>A string representation of the value.</returns>
        string GetArgumentValueAsString(object argumentValue);
    }
}