namespace FakeItEasy
{
    using System;

    /// <summary>
    /// Represents a text writer that writes to the output.
    /// </summary>
    public interface IOutputWriter
    {
        /// <summary>
        /// Writes the specified value to the output.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <returns>The writer for method chaining.</returns>
        IOutputWriter Write(string value);

        /// <summary>
        /// Formats the specified argument value as a string and writes
        /// it to the output.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <returns>The writer for method chainging.</returns>
        IOutputWriter WriteArgumentValue(object value);

        IDisposable Indent();
    }
}