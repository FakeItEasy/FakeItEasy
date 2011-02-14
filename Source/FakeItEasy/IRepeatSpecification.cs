namespace FakeItEasy
{
    /// <summary>
    /// Provides properties and methods to specify repeat.
    /// </summary>
    public interface IRepeatSpecification
    {
        /// <summary>
        /// Specifies once as the repeat.
        /// </summary>
        Repeated Once { get; }

        /// <summary>
        /// Specifies twice as the repeat.
        /// </summary>
        Repeated Twice { get; }

        /// <summary>
        /// Specifies the number of times as repeat.
        /// </summary>
        /// <param name="numberOfTimes">The number of times expected.</param>
        /// <returns>A Repeated instance.</returns>
        Repeated Times(int numberOfTimes);
    }
}