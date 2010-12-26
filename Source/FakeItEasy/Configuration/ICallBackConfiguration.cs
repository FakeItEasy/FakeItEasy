namespace FakeItEasy.Configuration
{
    using System;
    using FakeItEasy.Core;

    /// <summary>
    /// Configuration for callbacks of fake object calls.
    /// </summary>
    /// <typeparam name="TInterface">The type of interface to return.</typeparam>
    public interface ICallbackConfiguration<out TInterface> : IHideObjectMembers
    {
        /// <summary>
        /// Executes the specified action when a matching call is being made.
        /// </summary>
        /// <param name="action">The action to invoke.</param>
        /// <returns>A configuration object.</returns>
        TInterface Invokes(Action<IFakeObjectCall> action);
    }
}