namespace FakeItEasy
{
    using FakeItEasy.Configuration;

    /// <summary>
    /// Provides extension methods for <see cref="IOutAndRefParametersConfiguration"/>.
    /// </summary>
    public static class OutAndRefParametersConfigurationExtensions
    {
        /// <summary>
        /// Specifies output values for out and ref parameters. The values should appear 
        /// in the same order as the out and ref parameters in the configured call.
        /// Any non out and ref parameters are ignored.
        /// The function will be called each time this call is made and can return different values.
        /// </summary>
        /// <param name="configuration">The configuration to extend.</param>
        /// <param name="values">The values.</param>
        /// <returns>A configuration object.</returns>
        public static IAfterCallSpecifiedConfiguration AssignsOutAndRefParameters(this IOutAndRefParametersConfiguration configuration, params object[] values)
        {
            Guard.AgainstNull(configuration, "configuration");
            Guard.AgainstNull(values, "values");

            return configuration.AssignsOutAndRefParametersLazily(x => values);
        }
    }
}