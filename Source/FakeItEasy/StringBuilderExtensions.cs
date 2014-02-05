namespace FakeItEasy
{
    using System.Text;

    /// <summary>
    /// Provides extension methods for <see cref="StringBuilder"/>.
    /// </summary>
    internal static class StringBuilderExtensions
    {
        public static StringBuilder AppendIndented(this StringBuilder builder, string indentString, string value)
        {
            var lines = value == null ? new string[] { } : value.Split('\n');

            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                builder.Append(indentString).Append(line);

                if (i != lines.Length - 1)
                {
                    builder.Append('\n');
                }
            }

            return builder;
        }
    }
}