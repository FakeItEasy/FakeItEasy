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
        /// <param name="configuration">The configuration to set repeat 1 to.</param>
        public static void Once<TInterface>(this IRepeatConfiguration<TInterface> configuration)
        {
            Guard.AgainstNull(configuration, nameof(configuration));

            configuration.NumberOfTimes(1);
        }

        /// <summary>
        /// Specifies NumberOfTimes(2) to the repeat configuration.
        /// </summary>
        /// <param name="configuration">The configuration to set repeat 2 to.</param>
        public static void Twice<TInterface>(this IRepeatConfiguration<TInterface> configuration)
        {
            Guard.AgainstNull(configuration, nameof(configuration));

            configuration.NumberOfTimes(2);
        }
    }
}
