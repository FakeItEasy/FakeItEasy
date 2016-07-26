namespace FakeItEasy
{
    using FakeItEasy.Configuration;

    /// <summary>
    /// Provides extensions for <see cref="IRepeatConfiguration{TInterface}"/>.
    /// </summary>
    public static class RepeatConfigurationExtensions
    {
        /// <summary>
        /// Specifies NumberOfTimes(1) to the repeat configuration.
        /// </summary>
        /// <typeparam name="TInterface">The type of configuration interface to return.</typeparam>
        /// <param name="configuration">The configuration to set repeat 1 to.</param>
        /// <returns>A configuration object that lets you define the subsequent behavior.</returns>
        public static IThenConfiguration<TInterface> Once<TInterface>(this IRepeatConfiguration<TInterface> configuration)
        {
            Guard.AgainstNull(configuration, nameof(configuration));

            return configuration.NumberOfTimes(1);
        }

        /// <summary>
        /// Specifies NumberOfTimes(2) to the repeat configuration.
        /// </summary>
        /// <typeparam name="TInterface">The type of configuration interface to return.</typeparam>
        /// <param name="configuration">The configuration to set repeat 2 to.</param>
        /// <returns>A configuration object that lets you define the subsequent behavior.</returns>
        public static IThenConfiguration<TInterface> Twice<TInterface>(this IRepeatConfiguration<TInterface> configuration)
        {
            Guard.AgainstNull(configuration, nameof(configuration));

            return configuration.NumberOfTimes(2);
        }
    }
}
