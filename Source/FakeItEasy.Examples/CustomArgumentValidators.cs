namespace FakeItEasy.Examples
{
    using System.Globalization;
    using FakeItEasy.Expressions;

    public static class CustomArgumentValidators
    {
        public static ArgumentConstraint<string> IsLongerThan(this ArgumentConstraintScope<string> validations, int length)
        {
            return ArgumentConstraint.Create(validations, x => x.Length > length, string.Format(CultureInfo.InvariantCulture, "Longer than {0}", length));
        }
    }
}
