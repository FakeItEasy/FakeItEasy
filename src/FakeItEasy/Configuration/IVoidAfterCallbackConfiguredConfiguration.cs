namespace FakeItEasy.Configuration
{
    /// <summary>
    /// Provides configuration methods for methods that do not have a return value
    /// and that have already had at least one callback configured.
    /// </summary>
    public interface IVoidAfterCallbackConfiguredConfiguration :
        IExceptionThrowerConfiguration<IVoidConfiguration>,
        ICallbackConfiguration<IVoidAfterCallbackConfiguredConfiguration>,
        ICallBaseConfiguration<IVoidConfiguration>,
        ICallWrappedMethodConfiguration<IVoidConfiguration>,
        IOutAndRefParametersConfiguration<IVoidConfiguration>,
        IDoNothingConfiguration<IVoidConfiguration>,
        IBehaviorLifetimeConfiguration<IVoidConfiguration>
    {
    }
}
