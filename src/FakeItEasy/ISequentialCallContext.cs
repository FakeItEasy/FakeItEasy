namespace FakeItEasy
{
    using System;
    using System.ComponentModel;
    using FakeItEasy.Core;

    /// <summary>
    /// Represents a context to make assertions on the order of the calls made on a fake.
    /// </summary>
    public interface ISequentialCallContext : IHideObjectMembers
    {
        /// <summary>
        /// This method supports the FakeItEasy infrastructure and is not intended to be used directly from your code.
        /// Checks that the next call was made in order.
        /// </summary>
        /// <param name="fakeManager">The manager for the fake on which the call was made.</param>
        /// <param name="callPredicate">The predicate used to match the call.</param>
        /// <param name="callDescription">The description of the call.</param>
        /// <param name="repeatConstraint">The repeat constraint for the call.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        void CheckNextCall(FakeManager fakeManager, Func<IFakeObjectCall, bool> callPredicate, string callDescription, Repeated repeatConstraint);
    }
}
