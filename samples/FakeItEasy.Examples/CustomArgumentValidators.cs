namespace FakeItEasy.Examples
{
    public static class CustomArgumentValidators
    {
        public static string IsLongerThan(this IArgumentConstraintManager<string> validations, int length)
        {
            return validations.Matches(x => x.Length > length, $"Longer than {length}");
        }
    }
}
