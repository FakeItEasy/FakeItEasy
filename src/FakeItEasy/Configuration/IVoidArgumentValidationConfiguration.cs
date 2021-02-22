namespace FakeItEasy.Configuration
{
    /// <summary>
    /// Provides configuration methods for methods that do not have a return value and
    /// allows the user to specify validations for arguments.
    /// </summary>
    public interface IVoidArgumentValidationConfiguration
        : IVoidConfiguration,
          IArgumentValidationConfiguration<IVoidConfiguration>
    {
    }
}
