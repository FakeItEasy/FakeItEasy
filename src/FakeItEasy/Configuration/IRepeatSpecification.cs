namespace FakeItEasy.Configuration
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Provides properties and methods to specify repeat.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1623:PropertySummaryDocumentationMustMatchAccessors", Justification = "Fluent API.")]
    public interface IRepeatSpecification : IHideObjectMembers
    {
        /// <summary>
        /// Specifies once as the repeat.
        /// </summary>
        /// <remarks>
        /// Assertions using the <see cref="Repeated"/> class are being phased out and will be deprecated in
        /// version 5.0.0 and removed in version 6.0.0.
        /// Prefer <see cref="AssertConfigurationExtensions.MustHaveHappenedOnceExactly"/>,
        /// <see cref="AssertConfigurationExtensions.MustHaveHappenedOnceOrMore"/>, or
        /// <see cref="AssertConfigurationExtensions.MustHaveHappenedOnceOrLess"/>.
        /// </remarks>
        Repeated Once { get; }

        /// <summary>
        /// Specifies twice as the repeat.
        /// </summary>
        /// <remarks>
        /// Assertions using the <see cref="Repeated"/> class are being phased out and will be deprecated in
        /// version 5.0.0 and removed in version 6.0.0.
        /// Prefer <see cref="AssertConfigurationExtensions.MustHaveHappenedTwiceExactly"/>,
        /// <see cref="AssertConfigurationExtensions.MustHaveHappenedTwiceOrMore"/>, or
        /// <see cref="AssertConfigurationExtensions.MustHaveHappenedTwiceOrLess"/>.
        /// </remarks>
        Repeated Twice { get; }

        /// <summary>
        /// Specifies the number of times as repeat.
        /// </summary>
        /// <param name="numberOfTimes">The number of times expected.</param>
        /// <returns>A Repeated instance.</returns>
        /// <remarks>
        /// Assertions using the <see cref="Repeated"/> class are being phased out and will be deprecated in
        /// version 5.0.0 and removed in version 6.0.0.
        /// Prefer <see cref="IAssertConfiguration.MustHaveHappened(System.Int32, FakeItEasy.Times)"/>.
        /// </remarks>
        Repeated Times(int numberOfTimes);
    }
}
