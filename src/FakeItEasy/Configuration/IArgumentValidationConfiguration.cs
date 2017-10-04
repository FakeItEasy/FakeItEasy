namespace FakeItEasy.Configuration
{
    using System;

    /// <summary>
    /// Provides configurations to validate arguments of a fake object call.
    /// </summary>
    /// <typeparam name="TInterface">The type of interface to return.</typeparam>
    public interface IArgumentValidationConfiguration<out TInterface> : IHideObjectMembers
    {
        /// <summary>
        /// Configures the call to be accepted when the specified predicate returns true.
        /// This method overrides any inline argument constraints (such as <see cref="A{T}.Ignored"/>,
        /// implicit equality matchers, or anything else); only <paramref name="argumentsPredicate"/>
        /// is considered when matching the call.
        /// </summary>
        /// <param name="argumentsPredicate">The argument predicate.</param>
        /// <returns>A configuration object.</returns>
        TInterface WhenArgumentsMatch(Func<ArgumentCollection, bool> argumentsPredicate);
    }
}
