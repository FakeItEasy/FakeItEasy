namespace FakeItEasy.Configuration
{
    using System;
    using System.Linq.Expressions;

    /// <summary>
    /// Allows the developer to assert on a call that's configured.
    /// </summary>
    public interface IAssertConfiguration : IHideObjectMembers
    {
        /// <summary>
        /// Asserts that the configured call has happened the number of times
        /// specified by <paramref name="repeatConstraint"/>.
        /// </summary>
        /// <param name="repeatConstraint">A constraint for how many times the call
        /// must have happened.</param>
        /// <exception cref="ExpectationException">The call has not been called a number of times
        /// that passes the repeat constraint.</exception>
        /// <returns>An object to assert the call order.</returns>
        /// <remarks>
        /// Assertions using the <see cref="Repeated"/> class are being phased out and will be deprecated in
        /// version 5.0.0 and removed in version 6.0.0.
        /// Prefer methods that do not use <c>Repeated</c>.
        /// </remarks>
        UnorderedCallAssertion MustHaveHappened(Repeated repeatConstraint);

        /// <summary>
        /// Asserts that the specified call must have happened the specified number of times exactly, or more, or less,
        /// depending on the <paramref name="timesOption"/>.
        /// </summary>
        /// <param name="numberOfTimes">The the number of times to expect the call to have happened, modified by <paramref name="timesOption"/>.</param>
        /// <param name="timesOption">Whether to expect the call to happen exactly the specified number of times, or at least the
        /// specified number of times, or at most the specified number of times.</param>
        /// <returns>An object to assert the call order.</returns>
        /// <example><code>
        /// A.CallTo(() => fake.Method()).MustHaveHappened(7, Times.Exactly)
        /// </code></example>
        /// <example><code>
        /// A.CallTo(() => fake.Method()).MustHaveHappened(2, Times.OrMore)
        /// </code></example>
        /// <example><code>
        /// A.CallTo(() => fake.Method()).MustHaveHappened(4, Times.OrLess)
        /// </code></example>
        UnorderedCallAssertion MustHaveHappened(int numberOfTimes, Times timesOption);

        /// <summary>
        /// Asserts that the specified call must have happened a number of times that matches the supplied <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">A function that returns <c>true</c> if and only if the actual number of calls that happened is the desired amount.</param>
        /// <returns>An object to assert the call order.</returns>
        /// <example>Check to see if a method was called an even number of times:<code>
        /// A.CallTo(() => fake.Method()).MustHaveHappenedANumberOfTimesMatching(n => n % 2 == 0)
        /// </code></example>
        UnorderedCallAssertion MustHaveHappenedANumberOfTimesMatching(Expression<Func<int, bool>> predicate);
    }
}
