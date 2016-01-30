namespace FakeItEasy.Configuration
{
    /// <summary>
    /// Configures a call that returns a value and allows the use to
    /// specify validations for arguments.
    /// </summary>
    /// <typeparam name="TMember">The type of the member.</typeparam>
    public interface IReturnValueArgumentValidationConfiguration<TMember>
        : IReturnValueConfiguration<TMember>,
          IArgumentValidationConfiguration<IReturnValueConfiguration<TMember>>
    {
    }
}
