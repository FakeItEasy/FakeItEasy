namespace FakeItEasy
{
    using FakeItEasy.Configuration;

    /// <summary>
    /// Provides the MustHaveHappened extension method for asserting calls to fake objects.
    /// </summary>
    public static class MustHaveHappenedExtensions
    {
        /// <summary>
        /// Asserts that the specified call must have happened once or more.
        /// </summary>
        /// <param name="configuration">The configuration to assert on.</param>
        public static void MustHaveHappened(this IAssertConfiguration configuration)
        {
            Guard.AgainstNull(configuration, "configuration");

            configuration.MustHaveHappened(Repeated.AtLeast.Once);
        }
    }
}