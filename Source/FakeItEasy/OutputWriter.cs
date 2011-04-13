namespace FakeItEasy
{
    using System;
    using System.Globalization;

    public static class OutputWriter
    {
        public static IOutputWriter WriteLine(this IOutputWriter writer)
        {
            writer.Write(Environment.NewLine);
            return writer;
        }

        public static IOutputWriter Write(this IOutputWriter writer, string format, params object[] args)
        {
            writer.Write(string.Format(CultureInfo.InvariantCulture, format, args));
            return writer;
        }

        public static IOutputWriter Write(this IOutputWriter writer, object value)
        {
            writer.Write(value.ToString());
            return writer;
        }
    }
}