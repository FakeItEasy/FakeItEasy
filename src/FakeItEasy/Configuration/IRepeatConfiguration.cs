namespace FakeItEasy.Configuration
{
    /// <summary>
    /// Provides configuration for method calls that has a return value.
    /// </summary>
    /// <typeparam name="TInterface">The type of configuration interface to return.</typeparam>
    public interface IRepeatConfiguration<out TInterface> : IHideObjectMembers
    {
        /// <summary>
        /// Specifies the number of times for the configured event.
        /// </summary>
        /// <param name="numberOfTimesToRepeat">The number of times to repeat.</param>
        /// <returns>A configuration object that lets you define the subsequent behavior.</returns>
        IThenConfiguration<TInterface> NumberOfTimes(int numberOfTimesToRepeat);
    }
}
