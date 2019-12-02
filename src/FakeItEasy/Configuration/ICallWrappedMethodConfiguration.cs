namespace FakeItEasy.Configuration
{
    /// <summary>
    /// Configuration that lets you specify that a fake object call should call the corresponding
    /// method on its wrapped object, assuming the fake is a wrapping fake.
    /// </summary>
    /// <typeparam name="TInterface">The type of configuration interface to return.</typeparam>
    public interface ICallWrappedMethodConfiguration<out TInterface> : IHideObjectMembers
    {
        /// <summary>
        /// When the configured method or methods are called the call
        /// will be delegated to the fake's wrapped object, assuming
        /// it's a wrapping fake.
        /// </summary>
        /// <returns>A configuration object.</returns>
        /// <exception cref="FakeConfigurationException">The fake object
        /// is not a wrapping fake.</exception>
        IAfterCallConfiguredConfiguration<TInterface> CallsWrappedMethod();
    }
}
