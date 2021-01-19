namespace FakeItEasy.Configuration
{
    /// <summary>
    /// Provides configuration for property setters that have already had at least one callback configured.
    /// </summary>
    public interface IPropertySetterAfterCallbackConfiguredConfiguration :
        IExceptionThrowerConfiguration<IPropertySetterConfiguration>,
        ICallbackConfiguration<IPropertySetterAfterCallbackConfiguredConfiguration>,
        ICallBaseConfiguration<IPropertySetterConfiguration>,
        ICallWrappedMethodConfiguration<IPropertySetterConfiguration>,
        IDoNothingConfiguration<IPropertySetterConfiguration>,
        IBehaviorLifetimeConfiguration<IPropertySetterConfiguration>
    {
    }
}
