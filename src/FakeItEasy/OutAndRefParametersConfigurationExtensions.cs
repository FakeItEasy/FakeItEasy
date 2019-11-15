namespace FakeItEasy
{
    using System;
#if FEATURE_NETCORE_REFLECTION
    using System.Reflection;
#endif
    using FakeItEasy.Configuration;

    /// <summary>
    /// Provides extension methods for <see cref="IOutAndRefParametersConfiguration{TInterface}"/>.
    /// </summary>
    public static class OutAndRefParametersConfigurationExtensions
    {
        /// <summary>
        /// Specifies output values for out and ref parameters. The values should appear
        /// in the same order as the out and ref parameters in the configured call.
        /// Any non out and ref parameters are ignored.
        /// The function will be called each time this call is made and can return different values.
        /// </summary>
        /// <typeparam name="TInterface">The type of configuration interface to return.</typeparam>
        /// <param name="configuration">The configuration to extend.</param>
        /// <param name="values">The values.</param>
        /// <returns>A configuration object.</returns>
        public static IAfterCallConfiguredConfiguration<TInterface> AssignsOutAndRefParameters<TInterface>(
            this IOutAndRefParametersConfiguration<TInterface> configuration, params object?[] values)
        {
            Guard.AgainstNull(configuration, nameof(configuration));
            Guard.AgainstNull(values, nameof(values));

            return configuration.AssignsOutAndRefParametersLazily(x => values);
        }
    }
}
