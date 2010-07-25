namespace FakeItEasy.Core
{
    using System.Collections.Generic;
using System;

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

    public interface ICallMatcherAccessor
    {
        /// <summary>
        /// Gets a call predicate that can be used to check if a fake object call matches
        /// the specified constraint.
        /// </summary>
        ICallMatcher Matcher { get; }
    }

    public static class OrderedAssertion
    {
        //public static void MustHaveHappenedInSequence(this IEnumerable<ICompletedFakeObjectCall> calls, params ICallMatcherAccessor[] callSpecifications)
        //{

        //}

        public static IDisposable OrderedAssertions(this IEnumerable<ICompletedFakeObjectCall> calls)
        {
            return null;
        }
    }

    public interface IFakeScope
        : IDisposable, IEnumerable<ICompletedFakeObjectCall>
    {

    }
}
