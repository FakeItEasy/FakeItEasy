namespace FakeItEasy
{
    using System;

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
        public static IFakeOptions<T> Strict<T>(this IFakeOptions<T> options)
        {
            Guard.AgainstNull(options, "options");

            Action<IFakeObjectCall> thrower = call =>
                {
                    throw new ExpectationException("Call to non configured method \"{0}\" of strict fake.".FormatInvariant(call.Method.Name));
                };

            return options.ConfigureFake(fake => A.CallTo(fake).Invokes(thrower));
        }

        /// <summary>
        /// Makes the fake strict. This means that any call to the fake
        /// that has not been explicitly configured will throw an exception.
        /// </summary>
        /// <param name="optionsBuilder">Action that builds options used to create the fake object.</param>
        /// <returns>A configuration object.</returns>
        public static IFakeOptions Strict(this IFakeOptions optionsBuilder)
        {
            Guard.AgainstNull(optionsBuilder, "optionsBuilder");

            Action<IFakeObjectCall> thrower = call =>
                {
                    throw new ExpectationException("Call to non configured method \"{0}\" of strict fake.".FormatInvariant(call.Method.Name));
                };

            return optionsBuilder.ConfigureFake(fake => A.CallTo(fake).Invokes(thrower));
        }

        /// <summary>
        /// Makes the fake default to calling base methods, so long as they aren't abstract.
        /// </summary>
        /// <typeparam name="T">The type of fake object.</typeparam>
        /// <param name="options">Options used to create the fake object.</param>
        /// <returns>An options object.</returns>
        public static IFakeOptions<T> CallsBaseMethods<T>(this IFakeOptions<T> options)
        {
            Guard.AgainstNull(options, "options");

            return options.ConfigureFake(fake => A.CallTo(fake)
                                                  .Where(call => !call.Method.IsAbstract)
                                                  .CallsBaseMethod());
        }
    }
}