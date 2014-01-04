namespace FakeItEasy
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    using FakeItEasy.Configuration;
    using FakeItEasy.Core;
    using FakeItEasy.Creation;
    using FakeItEasy.Expressions;

    /// <summary>
    /// Provides a WithAnyArguments extension methods matching calls to fake objects.
    /// </summary>
    public static class WithAnyArgumentsExtensions
    {
        /// <summary>
        /// Specifies that a call to the configured call should be applied no matter what arguments
        /// are used in the call to the faked object.
        /// </summary>
        /// <typeparam name="TInterface">The type of the interface.</typeparam>
        /// <param name="configuration">The configuration.</param>
        /// <returns>A configuration object.</returns>
        public static TInterface WithAnyArguments<TInterface>(this IArgumentValidationConfiguration<TInterface> configuration)
        {
            Guard.AgainstNull(configuration, "configuration");

            return configuration.WhenArgumentsMatch(x => true);
        }
    }
}