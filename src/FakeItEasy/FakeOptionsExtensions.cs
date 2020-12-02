namespace FakeItEasy
{
    using FakeItEasy.Core;
    using FakeItEasy.Creation;

    /// <summary>
    /// Provides extension methods for <see cref="IFakeOptions{T}"/>.
    /// </summary>
    public static class FakeOptionsExtensions
    {
        /// <summary>
        /// Makes the fake strict. This means that any call to the fake
        /// that has not been explicitly configured will throw an exception.
        /// </summary>
        /// <typeparam name="T">The type of fake object.</typeparam>
        /// <param name="options">Options used to create the fake object.</param>
        /// <returns>An options object.</returns>
        public static IFakeOptions<T> Strict<T>(this IFakeOptions<T> options) where T : class
        {
            return options.Strict(StrictFakeOptions.None);
        }

        /// <summary>
        /// Makes the fake strict. This means that any call to the fake
        /// that has not been explicitly configured will throw an exception,
        /// except calls to the <see cref="object"/> methods specified
        /// in <paramref name="strictOptions"/>.
        /// </summary>
        /// <typeparam name="T">The type of fake object.</typeparam>
        /// <param name="options">Options used to create the fake object.</param>
        /// <param name="strictOptions">Strict fake options.</param>
        /// <returns>An options object.</returns>
        public static IFakeOptions<T> Strict<T>(this IFakeOptions<T> options, StrictFakeOptions strictOptions) where T : class
        {
            Guard.AgainstNull(options, nameof(options));

            return options.ConfigureFake(fake =>
            {
                var manager = Fake.GetFakeManager(fake);
                manager.AddRuleFirst(new StrictFakeRule(strictOptions));
            });
        }

        /// <summary>
        /// Makes the fake strict. This means that any call to the fake
        /// that has not been explicitly configured will throw an exception.
        /// </summary>
        /// <param name="options">Options used to create the fake object.</param>
        /// <returns>An options object.</returns>
        public static IFakeOptions Strict(this IFakeOptions options)
        {
            return options.Strict(StrictFakeOptions.None);
        }

        /// <summary>
        /// Makes the fake strict. This means that any call to the fake
        /// that has not been explicitly configured will throw an exception,
        /// except calls to the <see cref="object"/> methods specified
        /// in <paramref name="strictOptions"/>.
        /// </summary>
        /// <param name="options">Options used to create the fake object.</param>
        /// <param name="strictOptions">Strict fake options.</param>
        /// <returns>An options object.</returns>
        public static IFakeOptions Strict(this IFakeOptions options, StrictFakeOptions strictOptions)
        {
            Guard.AgainstNull(options, nameof(options));

            return options.ConfigureFake(fake =>
            {
                var manager = Fake.GetFakeManager(fake);
                manager.AddRuleFirst(new StrictFakeRule(strictOptions));
            });
        }

        /// <summary>
        /// Makes the fake default to calling base methods, so long as they aren't abstract.
        /// </summary>
        /// <typeparam name="T">The type of fake object.</typeparam>
        /// <param name="options">Options used to create the fake object.</param>
        /// <returns>An options object.</returns>
        public static IFakeOptions<T> CallsBaseMethods<T>(this IFakeOptions<T> options) where T : class
        {
            Guard.AgainstNull(options, nameof(options));

            return options.ConfigureFake(fake => A.CallTo(fake)
                                                  .Where(call => !call.Method.IsAbstract)
                                                  .CallsBaseMethod());
        }

        /// <summary>
        /// Makes the fake default to calling base methods, so long as they aren't abstract.
        /// </summary>
        /// <param name="options">Options used to create the fake object.</param>
        /// <returns>An options object.</returns>
        public static IFakeOptions CallsBaseMethods(this IFakeOptions options)
        {
            Guard.AgainstNull(options, nameof(options));

            return options.ConfigureFake(fake => A.CallTo(fake)
                                                  .Where(call => !call.Method.IsAbstract)
                                                  .CallsBaseMethod());
        }
    }
}
