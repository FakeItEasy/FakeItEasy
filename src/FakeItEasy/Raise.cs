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
        public static Raise<TEventArgs> With<TEventArgs>(object sender, TEventArgs e)
#if FEATURE_EVENT_ARGS_MUST_EXTEND_EVENTARGS
            where TEventArgs : EventArgs
#endif
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
        public static Raise<TEventArgs> With<TEventArgs>(TEventArgs e)
#if FEATURE_EVENT_ARGS_MUST_EXTEND_EVENTARGS
            where TEventArgs : EventArgs
#endif
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
        [Obsolete("Raise.With<TEventHandler> will be removed in version 5.0.0. Use Raise.FreeForm.With instead.")]
        public static TEventHandler With<TEventHandler>(params object[] arguments)
        {
            return new DelegateRaiser<TEventHandler>(arguments, ArgumentProviderMap);
        }

        /// <summary>
        /// Allows the developer to raise an event with a non-standard signature on a faked object.
        /// </summary>
        public static class FreeForm
        {
            /// <summary>
            /// Raises an event with non-standard signature, resolving the actual delegate type dynamically.
            /// </summary>
            /// <param name="arguments">The arguments to send to the event handlers.</param>
            /// <returns>A new object that knows how to raise events.</returns>
            public static dynamic With(params object[] arguments)
            {
                return new DynamicRaiser(arguments, ArgumentProviderMap);
            }
        }
    }
}
