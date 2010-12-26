namespace FakeItEasy.Configuration
{
    /// <summary>
    /// Allows the developer to assert on a call that's configured.
    /// </summary>
    public interface IAssertConfiguration
        : IHideObjectMembers
    {
        /// <summary>
        /// Asserts that the configured call has happened the number of times
        /// constrained by the repeatConstraint parameter.
        /// </summary>
        /// <param name="repeatConstraint">A constraint for how many times the call
        /// must have happened.</param>
        /// <exception cref="ExpectationException">The call has not been called a number of times
        /// that passes the repeat constraint.</exception>
        void MustHaveHappened(Repeated repeatConstraint);
    }
}