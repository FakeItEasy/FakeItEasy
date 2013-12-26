namespace FakeItEasy
{
    using FakeItEasy.Configuration;

    /// <summary>
    /// Provides the MustNotHaveHappened extension method for asserting calls to fake objects.
    /// </summary>
    public static class MustNotHaveHappenedExtensions
    {
        /// <summary>
        /// Asserts that the specified call has not happened.
        /// </summary>
        /// <param name="configuration">The configuration to assert on.</param>
        public static void MustNotHaveHappened(this IAssertConfiguration configuration)
        {
            Guard.AgainstNull(configuration, "configuration");

            configuration.MustHaveHappened(Repeated.Never);
        }
    }
}