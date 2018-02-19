namespace FakeItEasy
{
    using System.Text;
    using FakeItEasy.Compatibility;

    /// <summary>
    /// Provides extension methods for <see cref="StringBuilder"/>.
    /// </summary>
    internal static class StringBuilderExtensions
    {
        public static StringBuilder AppendIndented(this StringBuilder builder, string indentString, string value)
        {
            var lines = value?.Split('\n') ?? ArrayHelper.Empty<string>();

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
