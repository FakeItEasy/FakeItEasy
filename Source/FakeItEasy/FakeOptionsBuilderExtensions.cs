namespace FakeItEasy
{
    using System;

    using FakeItEasy.Core;
    using FakeItEasy.Creation;

    /// <summary>
    /// Provides extension methods for <see cref="IFakeOptionsBuilder{T}"/>.
    /// </summary>
    public static class FakeOptionsBuilderExtensions
    {
        /// <summary>
        /// Makes the fake strict, this means that any call to the fake
        /// that has not been explicitly configured will throw an exception.
        /// </summary>
        /// <typeparam name="T">The type of fake object.</typeparam>
        /// <param name="options">The configuration.</param>
        /// <returns>A configuration object.</returns>
        public static IFakeOptionsBuilder<T> Strict<T>(this IFakeOptionsBuilder<T> options)
        {
            Guard.AgainstNull(options, "options");

            Action<IFakeObjectCall> thrower = c =>
                {
                    throw new ExpectationException("Call to non configured method \"{0}\" of strict fake.".FormatInvariant(c.Method.Name));
                };

            return options.OnFakeCreated(
                x => A.CallTo(x).Invokes(thrower));
        }
    }
}