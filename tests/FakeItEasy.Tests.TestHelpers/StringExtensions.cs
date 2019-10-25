namespace FakeItEasy.Tests.TestHelpers
{
    using System;
    using System.Text.RegularExpressions;

    public static class StringExtensions
    {
        public static string NormalizeLineEndings(this string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return s;
            }

#if FEATURE_STRING_CONTAINS_COMPARISONTYPE
            if (!s.Contains('\n', StringComparison.Ordinal))
#else
            if (s.IndexOf('\n') < 0)
#endif
            {
                return s;
            }

            return Regex.Replace(s, "\r?\n", Environment.NewLine);
        }
    }
}
