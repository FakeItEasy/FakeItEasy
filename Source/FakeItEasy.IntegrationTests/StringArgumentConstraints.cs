namespace FakeItEasy.IntegrationTests
{
    using FakeItEasy.Expressions;
    using FakeItEasy.Core;
    
    public static class StringArgumentConstraints
    {
        public static string StartsWith(this IArgumentConstraintManager<string> scope, string beginning)
        {
            return scope.Matches(x => x.StartsWith(beginning), string.Format("Starts with \"{0}\"", beginning));
        }

        public static string Contains(this IArgumentConstraintManager<string> scope, string value)
        {
            return scope.Matches(x => x.Contains(value), string.Format("Contains \"{0}\"", value));
        }
    }
}
