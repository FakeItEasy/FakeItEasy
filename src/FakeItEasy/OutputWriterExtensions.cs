namespace FakeItEasy
{
    using System;
    using System.Collections;
    using System.Linq;

    /// <summary>
    /// Provides extensions for <see cref="IOutputWriter"/>.
    /// </summary>
    public static class OutputWriterExtensions
    {
        /// <summary>
        /// Writes a new line to the writer.
        /// </summary>
        /// <param name="writer">The writer to write to.</param>
        /// <returns>The writer.</returns>
        public static IOutputWriter WriteLine(this IOutputWriter writer)
        {
            Guard.AgainstNull(writer, nameof(writer));

            writer.Write(Environment.NewLine);
            return writer;
        }

        /// <summary>
        /// Writes the format string to the writer.
        /// </summary>
        /// <param name="writer">The writer to write to.</param>
        /// <param name="format">The format string to write.</param>
        /// <param name="args">Replacements for the format string.</param>
        /// <returns>The writer.</returns>
        public static IOutputWriter Write(this IOutputWriter writer, string format, params object?[] args)
        {
            Guard.AgainstNull(writer, nameof(writer));
            Guard.AgainstNull(format, nameof(format));
            Guard.AgainstNull(args, nameof(args));

            writer.Write(string.Format(format, args));
            return writer;
        }

        /// <summary>
        /// Writes the specified object to the writer (using the ToString-method of the object).
        /// </summary>
        /// <param name="writer">The writer to write to.</param>
        /// <param name="value">The value to write to the writer.</param>
        /// <returns>The writer.</returns>
        public static IOutputWriter Write(this IOutputWriter writer, object value)
        {
            Guard.AgainstNull(writer, nameof(writer));
            Guard.AgainstNull(value, nameof(value));

            writer.Write(value.ToString());
            return writer;
        }

        /// <summary>
        /// Formats the specified argument values as strings and writes them to the output.
        /// </summary>
        /// <param name="writer">The writer to write to.</param>
        /// <param name="values">The values to write to the writer.</param>
        /// <returns>The writer.</returns>
        internal static IOutputWriter WriteArgumentValues(this IOutputWriter writer, IEnumerable values)
        {
            Guard.AgainstNull(writer, nameof(writer));
            Guard.AgainstNull(values, nameof(values));

            var list = values.AsList();
            if (list.Count <= 5)
            {
                writer.WriteArgumentValuesImpl(list);
            }
            else
            {
                writer.WriteArgumentValuesImpl(list.Take(2));
                int remainingCount = list.Count - 4;
                writer.Write($", … ({remainingCount} more elements) …, ");
                writer.WriteArgumentValuesImpl(list.Skip(list.Count - 2));
            }

            return writer;
        }

        /// <summary>
        /// Formats the specified argument values as strings and writes them to the output.
        /// </summary>
        /// <param name="writer">The writer to write to.</param>
        /// <param name="values">The values to write to the writer.</param>
        /// <returns>The writer.</returns>
        private static IOutputWriter WriteArgumentValuesImpl(this IOutputWriter writer, IEnumerable values)
        {
            bool first = true;
            foreach (var value in values)
            {
                if (!first)
                {
                    writer.Write(", ");
                }

                writer.WriteArgumentValue(value);

                first = false;
            }

            return writer;
        }
    }
}
