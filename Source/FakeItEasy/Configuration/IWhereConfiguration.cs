namespace FakeItEasy.Configuration
{
    using System;
    using Core;

    /// <summary>
    /// Provides a way to configure predicates for when a call should be applied.
    /// </summary>
    /// <typeparam name="T">The type of fake object that is going to be configured..</typeparam>
    public interface IWhereConfiguration<out T>
    {
        /// <summary>
        /// Applies a predicate to constrain which calls will be considered for interception.
        /// </summary>
        /// <param name="predicate">A predicate for a fake object call.</param>
        /// <param name="descriptionWriter">An action that writes a description of the predicate
        /// to the output.</param>
        /// <returns>The configuration object.</returns>
        T Where(Func<IFakeObjectCall, bool> predicate, Action<IOutputWriter> descriptionWriter);
    }
}