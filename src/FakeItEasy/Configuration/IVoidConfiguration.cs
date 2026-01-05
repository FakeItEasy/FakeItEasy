namespace FakeItEasy.Configuration;

/// <summary>
/// Provides configuration methods for methods that do not have a return value.
/// </summary>
public interface IVoidConfiguration :
    IExceptionThrowerConfiguration<IVoidConfiguration>,
    ICallbackConfiguration<IVoidAfterCallbackConfiguredConfiguration>,
    ICallBaseConfiguration<IVoidConfiguration>,
    ICallWrappedMethodConfiguration<IVoidConfiguration>,
    IOutAndRefParametersConfiguration<IVoidConfiguration>,
    IAssertConfiguration,
    IDoNothingConfiguration<IVoidConfiguration>
{
}
