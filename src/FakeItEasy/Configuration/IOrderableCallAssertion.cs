namespace FakeItEasy.Configuration
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Allows clients to check that calls happened in the desired order.
    /// </summary>
    public interface IOrderableCallAssertion : IHideObjectMembers
    {
        /// <summary>
        /// Checks that the asserted call happened in order relative to others in the assertion chain.
        /// </summary>
        /// <param name="nextAssertion">An assertion describing the next call that should occur.</param>
        /// <returns>An object that can be used to assert that a following call was made in the expected order.</returns>
        /// <exception cref="ExpectationException">The call was not made in the expected order.</exception>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = nameof(Then), Justification = "There's no need for clients to implement the member.")]
        IOrderableCallAssertion Then(UnorderedCallAssertion nextAssertion);
    }
}
