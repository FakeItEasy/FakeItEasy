namespace FakeItEasy
{
    using FakeItEasy.Configuration;

    /// <summary>
    /// Provides extension methods for <see cref="IArgumentValidationConfiguration{TInterface}"/>.
    /// </summary>
    public static class ArgumentValidationConfigurationExtensions
    {
        /// <summary>
        /// Specifies that a call to the configured call should be applied no matter what arguments
        /// are used in the call to the faked object.
        /// </summary>
        /// <typeparam name="TInterface">The type of the interface.</typeparam>
        /// <param name="configuration">The configuration.</param>
        /// <returns>A configuration object.</returns>
        public static TInterface WithAnyArguments<TInterface>(this IArgumentValidationConfiguration<TInterface> configuration)
        {
            Guard.AgainstNull(configuration, "configuration");

            return configuration.WhenArgumentsMatch(x => true);
        }
    }
}