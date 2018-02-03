namespace FakeItEasy.Configuration
{
    /// <summary>
    /// Provides configuration to specify the number of times a configured behavior should be applied.
    /// </summary>
    /// <typeparam name="TInterface">The type of configuration interface to return.</typeparam>
    public interface IRepeatConfiguration<out TInterface> : IHideObjectMembers
    {
        /// <summary>
        /// Specifies the number of times the configured behavior should be applied.
        /// </summary>
        /// <param name="numberOfTimes">The number of times the configured behavior should be applied.</param>
        /// <returns>A configuration object that lets you define the subsequent behavior.</returns>
        IThenConfiguration<TInterface> NumberOfTimes(int numberOfTimes);
    }
}
