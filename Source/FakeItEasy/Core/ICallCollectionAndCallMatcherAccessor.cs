namespace FakeItEasy.Core
{
    using System.Collections.Generic;

    /// <summary>
    /// Provides access to a set of calls and a call matcher for these calls.
    /// </summary>
    public interface ICallCollectionAndCallMatcherAccessor
    {
        /// <summary>
        /// A set of calls.
        /// </summary>
        IEnumerable<ICompletedFakeObjectCall> Calls { get; }

        /// <summary>
        /// A matcher used to select among the calls.
        /// </summary>
        ICallMatcher Matcher { get; }
    }
}
