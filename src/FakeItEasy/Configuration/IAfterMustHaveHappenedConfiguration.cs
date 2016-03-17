namespace FakeItEasy.Configuration
{
    /// <summary>
    /// Allows the developer to check that an asserted call happened in order.
    /// </summary>
    public interface IAfterMustHaveHappenedConfiguration : IHideObjectMembers
    {
        /// <summary>
        /// Checks that the asserted call happens in order for the specified context.
        /// </summary>
        /// <param name="context">The context for which to check the call ordering.</param>
        /// <exception cref="ExpectationException">The call has not been made in the expected order.</exception>
        void InOrder(ISequentialCallContext context);
    }
}
