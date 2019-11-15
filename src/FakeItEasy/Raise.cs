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
            ServiceLocator.Resolve<EventHandlerArgumentProviderMap>();

        /// <summary>
        /// Raises an event on a faked object by attaching the event handler produced by the method
        /// to the event that is to be raised.
        /// </summary>
        /// <typeparam name="TEventArgs">The type of the event args.</typeparam>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <returns>A Raise(TEventArgs)-object that exposes the event handler to attach.</returns>
        [SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers", Justification = "Must be visible to provide the event raising syntax.")]
        public static Raise<TEventArgs> With<TEventArgs>(object? sender, TEventArgs e)
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
        /// Allows the developer to raise an event with a non-standard signature on a faked object.
        /// Uses late binding, so requires a reference to Microsoft.CSharp when called from C#, and is not compatible
        /// with all CLR languages (for example Visual Basic).
        /// To raise non-standard events from other languages, use <see cref="Raise.FreeForm{TEventHandler}"/>.
        /// </summary>
#pragma warning disable CA1034 // Do not nest type
        public static class FreeForm
#pragma warning restore CA1034 // Do not nest type
        {
            /// <summary>
            /// Raises an event with non-standard signature, resolving the actual delegate type dynamically.
            /// </summary>
            /// <param name="arguments">The arguments to send to the event handlers.</param>
            /// <returns>A new object that knows how to raise events.</returns>
            public static dynamic With(params object?[] arguments)
            {
                return new DynamicRaiser(arguments, ArgumentProviderMap);
            }
        }

        /// <summary>
        /// Allows the developer to raise an event with a non-standard signature on a faked object.
        /// Intended to be used from languages, such as Visual Basic, that do not support late binding via dynamic
        /// objects, or when a reference to Microsoft.CSharp is not desired.
        /// Otherwise, prefer <see cref="Raise.FreeForm" />.
        /// </summary>
        /// <typeparam name="TEventHandler">The type of the event handler.</typeparam>
#pragma warning disable CA1034 // Do not nest type
        public static class FreeForm<TEventHandler> where TEventHandler : Delegate
#pragma warning restore CA1034 // Do not nest type
        {
            /// <summary>
            /// Raises an event with non-standard signature.
            /// </summary>
            /// <param name="arguments">The arguments to send to the event handlers.</param>
            /// <returns>A new object that knows how to raise events.</returns>
#pragma warning disable CA1000 // Do not declare static members on generic types
            public static TEventHandler With(params object?[] arguments)
#pragma warning restore CA1000 // Do not declare static members on generic types
            {
                return new DelegateRaiser<TEventHandler>(arguments, ArgumentProviderMap);
            }
        }
    }
}
