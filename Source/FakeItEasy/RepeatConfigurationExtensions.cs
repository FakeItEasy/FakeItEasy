namespace FakeItEasy
{
    using FakeItEasy.Configuration;

    /// <summary>
    /// Provides extensions for <see cref="IRepeatConfiguration"/>.
    /// </summary>
    public static class RepeatConfigurationExtensions
    {
        /// <summary>
        /// Specifies NumberOfTimes(1) to the repeat configuration.
        /// </summary>
        /// <param name="configuration">The configuration to set repeat 1 to.</param>
        public static void Once(this IRepeatConfiguration configuration)
        {
            Guard.AgainstNull(configuration, "configuration");

            configuration.NumberOfTimes(1);
        }

        /// <summary>
        /// Specifies NumberOfTimes(2) to the repeat configuration.
        /// </summary>
        /// <param name="configuration">The configuration to set repeat 2 to.</param>
        public static void Twice(this IRepeatConfiguration configuration)
        {
            Guard.AgainstNull(configuration, "configuration");

            configuration.NumberOfTimes(2);
        }
    }
}