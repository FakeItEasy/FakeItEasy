namespace FakeItEasy.Configuration
{
    /// <summary>
    /// Provides configuration methods for calls that should perform no action.
    /// </summary>
    /// <typeparam name="TInterface">The type of configuration interface to return.</typeparam>
    public interface IDoNothingConfiguration<out TInterface>
    {
        /// <summary>
        /// Configures the specified call to do nothing when called.
        /// </summary>
        /// <returns>A configuration object.</returns>
        IAfterCallSpecifiedConfiguration<TInterface> DoesNothing();
    }
}
