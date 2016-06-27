namespace FakeItEasy
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq.Expressions;
    using FakeItEasy.Configuration;
    using FakeItEasy.Core;

    /// <summary>
    /// Provides extension methods for <see cref="IWhereConfiguration{T}"/>.
    /// </summary>
    public static class WhereConfigurationExtensions
    {
        /// <summary>
        /// Applies a predicate to constrain which calls will be considered for interception.
        /// </summary>
        /// <typeparam name="T">The return type of the where method.</typeparam>
        /// <param name="configuration">The configuration object to extend.</param>
        /// <param name="predicate">A predicate for a fake object call.</param>
        /// <returns>The configuration object.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Appropriate for expressions.")]
        public static T Where<T>(this IWhereConfiguration<T> configuration, Expression<Func<IFakeObjectCall, bool>> predicate)
        {
            Guard.AgainstNull(configuration, nameof(configuration));
            Guard.AgainstNull(predicate, nameof(predicate));

            return configuration.Where(predicate.Compile(), x => x.Write(predicate.ToString()));
        }
    }
}
