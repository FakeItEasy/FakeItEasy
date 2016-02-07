namespace FakeItEasy.IntegrationTests
{
    using System;
    using System.Globalization;

    public static class StringArgumentConstraints
    {
        public static string StartsWith(this IArgumentConstraintManager<string> scope, string beginning)
        {
            return scope.Matches(
                x => x.StartsWith(beginning, StringComparison.CurrentCulture), string.Format(CultureInfo.CurrentCulture, "Starts with \"{0}\"", beginning));
        }

        public static string Contains(this IArgumentConstraintManager<string> scope, string value)
        {
            return scope.Matches(x => x.Contains(value), string.Format(CultureInfo.CurrentCulture, "Contains \"{0}\"", value));
        }
    }
}
