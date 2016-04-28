namespace FakeItEasy.Configuration
{
    /// <summary>
    /// Provides configuration methods for calls that should perform no action.
    /// </summary>
    public interface IDoNothingConfiguration
    {
        /// <summary>
        /// Configures the specified call to do nothing when called.
        /// </summary>
        /// <returns>A configuration object.</returns>
        IAfterCallSpecifiedConfiguration DoesNothing();
    }
}
