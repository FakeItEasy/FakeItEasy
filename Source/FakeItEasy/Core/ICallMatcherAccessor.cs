namespace FakeItEasy.Core
{
    /// <summary>
    /// Provides access to a call matcher.
    /// </summary>
    internal interface ICallMatcherAccessor
    {
        /// <summary>
        /// Gets a call predicate that can be used to check if a fake object call matches
        /// the specified constraint.
        /// </summary>
        ICallMatcher Matcher { get; }
    }
}