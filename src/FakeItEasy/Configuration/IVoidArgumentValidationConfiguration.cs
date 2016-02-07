namespace FakeItEasy.Configuration
{
    /// <summary>
    /// Provides configuration methods for methods that does not have a return value and
    /// allows the use to specify validations for arguments.
    /// </summary>
    public interface IVoidArgumentValidationConfiguration
        : IVoidConfiguration,
          IArgumentValidationConfiguration<IVoidConfiguration>
    {
    }
}
