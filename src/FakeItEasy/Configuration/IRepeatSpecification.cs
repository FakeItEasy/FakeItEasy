namespace FakeItEasy.Configuration
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Provides properties and methods to specify repeat.
    /// </summary>
    [Obsolete("Assertions using the Repeated class will be removed in version 6.0.0. Use variants of MustHaveHappened that specify the number of calls instead.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1623:PropertySummaryDocumentationMustMatchAccessors", Justification = "Fluent API.")]
    public interface IRepeatSpecification : IHideObjectMembers
    {
        /// <summary>
        /// Specifies once as the repeat.
        /// </summary>
        [Obsolete("Assertions using the Repeated class will be removed in version 6.0.0. Use MustHaveHappenedOnceExactly, MustHaveHappenedOnceOrMore, or MustHaveHappenedOnceOrLess instead.")]
        Repeated Once { get; }

        /// <summary>
        /// Specifies twice as the repeat.
        /// </summary>
        [Obsolete("Assertions using the Repeated class will be removed in version 6.0.0. Use MustHaveHappenedTwiceExactly, MustHaveHappenedTwiceOrMore, or MustHaveHappenedTwiceOrLess instead.")]
        Repeated Twice { get; }

        /// <summary>
        /// Specifies the number of times as repeat.
        /// </summary>
        /// <param name="numberOfTimes">The number of times expected.</param>
        /// <returns>A Repeated instance.</returns>
        [Obsolete("Assertions using the Repeated class will be removed in version 6.0.0. Use MustHaveHappened(int, Times) instead.")]
        Repeated Times(int numberOfTimes);
    }
}
