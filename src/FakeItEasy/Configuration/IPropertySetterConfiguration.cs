namespace FakeItEasy.Configuration;

/// <summary>
/// Provides configuration for property setters and allows the user to specify validations for arguments.
/// </summary>
public interface IPropertySetterConfiguration :
    IExceptionThrowerConfiguration<IPropertySetterConfiguration>,
    ICallbackConfiguration<IPropertySetterAfterCallbackConfiguredConfiguration>,
    ICallBaseConfiguration<IPropertySetterConfiguration>,
    ICallWrappedMethodConfiguration<IPropertySetterConfiguration>,
    IAssertConfiguration,
    IDoNothingConfiguration<IPropertySetterConfiguration>
{
}
