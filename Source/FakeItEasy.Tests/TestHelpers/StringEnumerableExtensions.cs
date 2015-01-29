namespace FakeItEasy.Tests.TestHelpers
{
    using System.Collections.Generic;

    /// <summary>
    /// Provides extension methods for enumerable collections of <see cref="string"/>.
    /// </summary>
    public static class StringEnumerableExtensions
    {
        /// <summary>
        /// Joins a number of individual lines with Windows-style newline characters
        /// to make a text block.
        /// </summary>
        /// <param name="lines">The lines to join into a text block.</param>
        /// <returns>The formatted string.</returns>
        public static string AsTextBlock(this IEnumerable<string> lines)
        {
            return string.Join("\r\n", lines);
        }
    }
}