namespace FakeItEasy
{
    using System.Collections.Generic;

    using FakeItEasy.Configuration;

    /// <summary>
    /// Provides the ReturnsNextFromSequence extension method for specifying return values of fake object calls.
    /// </summary>
    public static class ReturnsNextFromSequenceExtensions
    {
        /// <summary>
        /// Configures the call to return the next value from the specified sequence each time it's called. Null will
        /// be returned when all the values in the sequence has been returned.
        /// </summary>
        /// <typeparam name="T">
        /// The type of return value.
        /// </typeparam>
        /// <param name="configuration">
        /// The call configuration to extend.
        /// </param>
        /// <param name="values">
        /// The values to return in sequence.
        /// </param>
        public static void ReturnsNextFromSequence<T>(this IReturnValueConfiguration<T> configuration, params T[] values)
        {
            Guard.AgainstNull(configuration, "configuration");

            var queue = new Queue<T>(values);
            configuration.ReturnsLazily(x => queue.Dequeue()).NumberOfTimes(queue.Count);
        }
    }
}