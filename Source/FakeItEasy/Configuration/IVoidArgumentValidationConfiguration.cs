namespace FakeItEasy.Configuration
{
    /// <summary>
    /// Provides configuration methods for methods that does not have a return value and
    /// allows the use to specify validations for arguments.
    /// </summary>
    /// <typeparam name="TFake">The type of the fake.</typeparam>
    public interface IVoidArgumentValidationConfiguration
        : IVoidConfiguration,
          IArgumentValidationConfiguration<IVoidConfiguration>
    {

    }
}
