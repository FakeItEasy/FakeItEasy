namespace FakeItEasy.Configuration
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq.Expressions;
    using Core;

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