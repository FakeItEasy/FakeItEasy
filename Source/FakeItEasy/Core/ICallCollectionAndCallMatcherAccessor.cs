namespace FakeItEasy.Core
{
    using System.Collections.Generic;

    /// <summary>
    /// Provides access to a set of calls and a call matcher for these calls.
    /// </summary>
    public interface ICallCollectionAndCallMatcherAccessor
        : ICallMatcherAccessor
    {
        /// <summary>
        /// A set of calls.
        /// </summary>
        IEnumerable<ICompletedFakeObjectCall> Calls { get; }
    }
}