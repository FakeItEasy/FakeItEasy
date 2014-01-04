namespace FakeItEasy
{
    using FakeItEasy.Configuration;

    /// <summary>
    /// Provides the Once extension methods for configuring repeats on fake object call behavior.
    /// </summary>
    public static class OnceExtensions
    {
        /// <summary>
        /// Specifies NumberOfTimes(1) to the IRepeatConfiguration{TFake}.
        /// </summary>
        /// <param name="configuration">The configuration to set repeat 1 to.</param>
        public static void Once(this IRepeatConfiguration configuration)
        {
            Guard.AgainstNull(configuration, "configuration");

            configuration.NumberOfTimes(1);
        }
    }
}