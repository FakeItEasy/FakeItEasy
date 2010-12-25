namespace FakeItEasy.IntegrationTests
{
    using FakeItEasy.Expressions;
    
    public static class StringArgumentConstraints
    {
        public static ArgumentConstraint<string> StartsWith(this ArgumentConstraintScope<string> scope, string beginning)
        {
            return ArgumentConstraint.Create(scope, x => x.StartsWith(beginning), string.Format("Starts with \"{0}\"", beginning));
        }

        public static ArgumentConstraint<string> Contains(this ArgumentConstraintScope<string> scope, string value)
        {
            return ArgumentConstraint.Create(scope, x => x.Contains(value), string.Format("Contains \"{0}\"", value));
        }
    }
}
