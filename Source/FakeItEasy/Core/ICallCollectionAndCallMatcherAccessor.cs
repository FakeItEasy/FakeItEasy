namespace FakeItEasy.Core
{
    using System.Collections.Generic;

    /// <summary>
    /// Provides access to a set of calls and a call matcher for these calls.
    /// </summary>
    internal interface ICallCollectionAndCallMatcherAccessor
        : ICallMatcherAccessor
    {
        /// <summary>
        /// Gets the set of calls.
        /// </summary>
        IEnumerable<ICompletedFakeObjectCall> Calls { get; }
    }
}