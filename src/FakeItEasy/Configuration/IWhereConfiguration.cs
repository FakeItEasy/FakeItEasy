namespace FakeItEasy.Configuration
{
    using System;
    using FakeItEasy.Core;

    /// <summary>
    /// Provides a way to configure predicates for when a call should be applied.
    /// </summary>
    /// <typeparam name="TInterface">The type of configuration interface to return.</typeparam>
    public interface IWhereConfiguration<out TInterface> : IHideObjectMembers
    {
        /// <summary>
        /// Applies a predicate to constrain which calls will be considered for interception.
        /// </summary>
        /// <param name="predicate">A predicate for a fake object call.</param>
        /// <param name="descriptionWriter">An action that writes a description of the predicate
        /// to the output.</param>
        /// <returns>The configuration object.</returns>
        TInterface Where(Func<IFakeObjectCall, bool> predicate, Action<IOutputWriter> descriptionWriter);
    }
}
