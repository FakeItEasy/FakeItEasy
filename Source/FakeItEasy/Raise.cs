namespace FakeItEasy
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using FakeItEasy.Core;

    /// <summary>
    /// Allows the developer to raise an event on a faked object.
    /// </summary>
    public static class Raise
    {
        private static readonly EventHandlerArgumentProviderMap ArgumentProviderMap =
            ServiceLocator.Current.Resolve<EventHandlerArgumentProviderMap>();

        /// <summary>
        /// Raises an event on a faked object by attaching the event handler produced by the method
        /// to the event that is to be raised.
        /// </summary>
        /// <typeparam name="TEventArgs">The type of the event args.</typeparam>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <returns>A Raise(TEventArgs)-object that exposes the event handler to attach.</returns>
        [SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers", Justification = "Must be visible to provide the event raising syntax.")]
        public static Raise<TEventArgs> With<TEventArgs>(object sender, TEventArgs e) where TEventArgs : EventArgs
        {
            return new Raise<TEventArgs>(sender, e, ArgumentProviderMap);
        }

        /// <summary>
        /// Raises an event on a faked object by attaching the event handler produced by the method
        /// to the event that is to be raised.
        /// </summary>
        /// <typeparam name="TEventArgs">The type of the event arguments.</typeparam>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        /// <returns>
        /// A Raise(TEventArgs)-object that exposes the event handler to attach.
        /// </returns>
        public static Raise<TEventArgs> With<TEventArgs>(TEventArgs e) where TEventArgs : EventArgs
        {
            return new Raise<TEventArgs>(e, ArgumentProviderMap);
        }

        /// <summary>
        /// Raises an event with empty event arguments on a faked object by attaching the event handler produced by the method
        /// to the event that is to be raised.
        /// </summary>
        /// <returns>
        /// A Raise(TEventArgs)-object that exposes the event handler to attach.
        /// </returns>
        public static Raise<EventArgs> WithEmpty()
        {
            return new Raise<EventArgs>(EventArgs.Empty, ArgumentProviderMap);
        }

        /// <summary>
        /// Raises an event with non-standard signature.
        /// </summary>
        /// <param name="arguments">The arguments to send to the event handlers.</param>
        /// <typeparam name="TEventHandler">The type of the event handler. Should be a <see cref="Delegate"/></typeparam>
        /// <returns>A new object that knows how to raise events.</returns>
        public static TEventHandler With<TEventHandler>(params object[] arguments)
        {
            return new DelegateRaiser<TEventHandler>(arguments, ArgumentProviderMap);
        }
    }
}