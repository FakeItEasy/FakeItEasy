namespace FakeItEasy.Configuration
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq.Expressions;
    using Core;

    /// <summary>
    /// Configuration for any call to a faked object.
    /// </summary>
    public interface IAnyCallConfiguration
        : IVoidArgumentValidationConfiguration
    {
        /// <summary>
        /// Matches calls that has the return type specified in the generic type parameter.
        /// </summary>
        /// <typeparam name="TMember">The return type of the members to configure.</typeparam>
        /// <returns>A configuration object.</returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Used to provide a strongly typed fluent API.")]
        IReturnValueArgumentValidationConfiguration<TMember> WithReturnType<TMember>();

        /// <summary>
        /// Applies a predicate to constrain which calls will be considered for interception.
        /// </summary>
        /// <param name="predicate">A predicate for a fake object call.</param>
        /// <returns>The configuration object.</returns>
        IAnyCallConfiguration Where(Expression<Func<IFakeObjectCall, bool>> predicate);
    }
}