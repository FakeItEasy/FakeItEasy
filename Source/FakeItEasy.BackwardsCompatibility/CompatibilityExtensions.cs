using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FakeItEasy.Configuration;
using System.Linq.Expressions;
using System.ComponentModel;
using FakeItEasy.Assertion;
using FakeItEasy.Expressions;

namespace FakeItEasy
{
    public static class CompatibilityExtensions
    {
        [Obsolete, EditorBrowsable(EditorBrowsableState.Never)]
        public static void MustHaveHappened(this IAssertConfiguration configuration, Expression<Func<int, bool>> repeatPredicate)
        {
            configuration.MustHaveHappened(Repeated.Like(repeatPredicate));
        }

        [Obsolete, EditorBrowsable(EditorBrowsableState.Never)]
        public static void Assert(this IAssertConfiguration configuration, Repeated repeatValidation)
        {
            configuration.MustHaveHappened(repeatValidation);
        }

        /// <summary>
        /// Specifies a function used to produce a return value when the configured call is made.
        /// The function will be called each time this call is made and can return different values
        /// each time.
        /// </summary>
        /// <param name="valueProducer">A function that produces the return value.</param>
        /// <returns>A configuration object.</returns>
        [Obsolete]
        public static IAfterCallSpecifiedWithOutAndRefParametersConfiguration Returns<TMember>(this IReturnValueConfiguration<TMember> configuration, Func<TMember> valueProducer)
        {
            return configuration.ReturnsLazily(x => valueProducer.Invoke());
        }

        /// <summary>
        /// Specifies that the configured call/calls should return null when called.
        /// </summary>
        /// <typeparam name="TMember">The type of the faked member.</typeparam>
        /// <param name="configuration">The configuration to apply to.</param>
        /// <returns>A configuration object.</returns>
        [Obsolete]
        public static IAfterCallSpecifiedConfiguration ReturnsNull<TMember>(this IReturnValueConfiguration<TMember> configuration) where TMember : class
        {
            Guard.AgainstNull(configuration, "configuration");

            return configuration.Returns((TMember)null);
        }
    }
}