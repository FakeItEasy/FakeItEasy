namespace FakeItEasy.Configuration
{
    /// <summary>
    /// Provides configuration for property setters and allows the user to specify validations for arguments.
    /// </summary>
    public interface IPropertySetterConfiguration
        : IExceptionThrowerConfiguration,
          ICallbackConfiguration<IPropertySetterConfiguration>,
          ICallBaseConfiguration,
          IAssertConfiguration,
          IDoNothingConfiguration
    {
    }
}
