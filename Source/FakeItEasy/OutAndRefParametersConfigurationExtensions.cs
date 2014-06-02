namespace FakeItEasy
{
    using System;
    using System.Collections.Generic;
#if NET40
    using System.Diagnostics.CodeAnalysis;
#endif
    using System.Linq;
#if NET40

    using System.Threading.Tasks;
#endif

    using FakeItEasy.Configuration;

    /// <summary>
    /// Provides extension methods for <see cref="IOutAndRefParametersConfiguration"/>.
    /// </summary>
    public static class OutAndRefParametersConfigurationExtensions
    {
        /// <summary>
        /// Specifies output values for out and ref parameters. Specify the values in the order
        /// the ref and out parameters has in the configured call, any non out and ref parameters are ignored.
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