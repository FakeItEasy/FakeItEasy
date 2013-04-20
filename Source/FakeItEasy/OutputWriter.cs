﻿namespace FakeItEasy
{
    using System;
    using System.Globalization;

    /// <summary>
    /// Provides static methods for the IOutputWriter-interface.
    /// </summary>
    public static class OutputWriter
    {
        /// <summary>
        /// Writes a new line to the writer.
        /// </summary>
        /// <param name="writer">The writer to write to.</param>
        /// <returns>The writer.</returns>
        public static IOutputWriter WriteLine(this IOutputWriter writer)
        {
            Guard.AgainstNull(writer, "writer");

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
        public static IOutputWriter Write(this IOutputWriter writer, string format, params object[] args)
        {
            Guard.AgainstNull(writer, "writer");

            writer.Write(string.Format(CultureInfo.InvariantCulture, format, args));
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
            Guard.AgainstNull(writer, "writer");
            Guard.AgainstNull(value, "value");

            writer.Write(value.ToString());
            return writer;
        }
    }
}