namespace FakeItEasy.Configuration
{
    /// <summary>
    /// Provides configuration methods for methods that does not have a return value.
    /// </summary>
    public interface IVoidConfiguration :
        IExceptionThrowerConfiguration<IVoidConfiguration>,
        ICallbackConfiguration<IVoidConfiguration>,
        ICallBaseConfiguration<IVoidConfiguration>,
        IOutAndRefParametersConfiguration<IVoidConfiguration>,
        IAssertConfiguration,
        IDoNothingConfiguration<IVoidConfiguration>
    {
    }
}
