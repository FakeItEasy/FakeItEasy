namespace FakeItEasy.Configuration
{
    /// <summary>
    /// Provides configuration methods for methods that does not have a return value.
    /// </summary>
    public interface IVoidConfiguration
        : IExceptionThrowerConfiguration, 
          ICallbackConfiguration<IVoidConfiguration>, 
          ICallBaseConfiguration, 
          IOutAndRefParametersConfiguration, 
          IAssertConfiguration, 
          IHideObjectMembers
    {
        /// <summary>
        /// Configures the specified call to do nothing when called.
        /// </summary>
        /// <returns>A configuration object.</returns>
        IAfterCallSpecifiedConfiguration DoesNothing();
    }
}