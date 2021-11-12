namespace FakeItEasy
{
    using FakeItEasy.Configuration;

    /// <summary>
    /// Provides extensions for <see cref="IBehaviorLifetimeConfiguration{TInterface}"/>.
    /// </summary>
    public static class BehaviorLifetimeConfigurationExtensions
    {
        /// <summary>
        /// Specifies that a call's configured behavior should apply only for 1 call.
        /// </summary>
        /// <typeparam name="TInterface">The type of configuration interface to return.</typeparam>
        /// <param name="configuration">The configuration on which to set the behavior lifetime to 1.</param>
        /// <returns>A configuration object that lets you define the subsequent behavior.</returns>
        public static IThenConfiguration<TInterface> Once<TInterface>(this IBehaviorLifetimeConfiguration<TInterface> configuration)
        {
            Guard.AgainstNull(configuration);

            return configuration.NumberOfTimes(1);
        }

        /// <summary>
        /// Specifies that a call's configured behavior should apply only for 2 calls.
        /// </summary>
        /// <typeparam name="TInterface">The type of configuration interface to return.</typeparam>
        /// <param name="configuration">The configuration on which to set the behavior lifetime to 2.</param>
        /// <returns>A configuration object that lets you define the subsequent behavior.</returns>
        public static IThenConfiguration<TInterface> Twice<TInterface>(this IBehaviorLifetimeConfiguration<TInterface> configuration)
        {
            Guard.AgainstNull(configuration);

            return configuration.NumberOfTimes(2);
        }
    }
}
