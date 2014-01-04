namespace FakeItEasy
{
    using FakeItEasy.Configuration;

    /// <summary>
    /// Provides the Twice extension methods for configuring repeats on fake object call behavior.
    /// </summary>
    public static class TwiceExtensions
    {
        /// <summary>
        /// Specifies NumberOfTimes(2) to the IRepeatConfiguration{TFake}.
        /// </summary>
        /// <param name="configuration">The configuration to set repeat 2 to.</param>
        public static void Twice(this IRepeatConfiguration configuration)
        {
            Guard.AgainstNull(configuration, "configuration");

            configuration.NumberOfTimes(2);
        }
    }
}