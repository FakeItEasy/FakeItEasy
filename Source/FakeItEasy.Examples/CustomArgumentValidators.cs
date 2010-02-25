namespace FakeItEasy.Examples
{
    using System.Globalization;
    using FakeItEasy.Expressions;

    public static class CustomArgumentValidators
    {
        public static ArgumentValidator<string> IsLongerThan(this ArgumentValidatorScope<string> validations, int length)
        {
            return ArgumentValidator.Create(validations, x => x.Length > length, string.Format(CultureInfo.InvariantCulture, "Longer than {0}", length));
        }
    }
}
