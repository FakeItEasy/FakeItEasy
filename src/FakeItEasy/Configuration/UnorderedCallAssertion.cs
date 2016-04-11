namespace FakeItEasy.Configuration
{
    using FakeItEasy.Core;

    /// <summary>
    /// Allows clients to check that calls happened in the desired order.
    /// </summary>
    public sealed class UnorderedCallAssertion : IOrderableCallAssertion
    {
        private readonly FakeManager fakeManager;
        private readonly ICallMatcher matcher;
        private readonly string callDescription;
        private readonly Repeated repeatConstraint;

        internal UnorderedCallAssertion(FakeManager fakeManager, ICallMatcher matcher, string callDescription, Repeated repeatConstraint)
        {
            this.fakeManager = fakeManager;
            this.matcher = matcher;
            this.callDescription = callDescription;
            this.repeatConstraint = repeatConstraint;
        }

        /// <summary>
        /// Checks that the asserted call happened in order relative to others in the assertion chain.
        /// </summary>
        /// <param name="nextAssertion">An assertion describing the next call that should occur.</param>
        /// <returns>An object that can be used to assert that a following call was made in the expected order.</returns>
        /// <exception cref="ExpectationException">The call was not made in the expected order.</exception>
        public IOrderableCallAssertion Then(UnorderedCallAssertion nextAssertion)
        {
            var context = ServiceLocator.Current.Resolve<SequentialCallContext>();
            this.CheckCallHappenedInOrder(context);
            return new OrderedCallAssertion(context).Then(nextAssertion);
        }

        private void CheckCallHappenedInOrder(SequentialCallContext context)
        {
            context.CheckNextCall(this.fakeManager, this.matcher.Matches, this.callDescription, this.repeatConstraint);
        }

        private class OrderedCallAssertion : IOrderableCallAssertion
        {
            private readonly SequentialCallContext context;

            public OrderedCallAssertion(SequentialCallContext context)
            {
                this.context = context;
            }

            public IOrderableCallAssertion Then(UnorderedCallAssertion nextAssertion)
            {
                Guard.AgainstNull(nextAssertion, nameof(nextAssertion));
                nextAssertion.CheckCallHappenedInOrder(this.context);
                return this;
            }
        }
    }
}
