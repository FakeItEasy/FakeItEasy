namespace FakeItEasy.Core
{
    using System.ComponentModel;

    /// <summary>
    /// Provides access to a call matcher.
    /// </summary>
    public interface ICallMatcherAccessor
    {
        /// <summary>
        /// Gets a call predicate that can be used to check if a fake object call matches
        /// the specified constraint.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        ICallMatcher Matcher { get; }
    }
}
