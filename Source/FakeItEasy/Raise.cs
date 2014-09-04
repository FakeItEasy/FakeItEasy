namespace FakeItEasy
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Allows the developer to raise an event on a faked object.
    /// </summary>
    public static class Raise
    {
        /// <summary>
        /// Holds a copy of all the arguments passed to (Delegate) event handlers.
        /// May move. May be expanded to hold ALL event handlers' arguments.
        /// </summary>
        public static readonly Dictionary<object, object[]> EventHandlerArguments = new Dictionary<object, object[]>();

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
            return new Raise<TEventArgs>(sender, e);
        }

        /// <summary>
        /// Raises an event with non-standard signature.
        /// </summary>
        /// <param name="arguments">The arguments to send to the event handlers.</param>
        /// <typeparam name="TEventHandler">The type of the event handler. Should be a <see cref="Delegate"/></typeparam>
        /// <returns>A new object that knows how to raise events.</returns>
        public static RaiseDelegate<TEventHandler> Event<TEventHandler>(params object[] arguments)
        {
            return new RaiseDelegate<TEventHandler>(arguments);
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
            return new Raise<TEventArgs>(null, e);
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
            return new Raise<EventArgs>(null, EventArgs.Empty);
        }

        /// <summary>
        /// Creates a new RaiseDelegate object.
        /// </summary>
        /// <typeparam name="TEventHandler">The type of the event handler. Should be a <see cref="Delegate"/></typeparam>
        public class RaiseDelegate<TEventHandler>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="RaiseDelegate{TEventHandler}"/> class.
            /// </summary>
            /// <param name="arguments">The arguments to be sent to the event handler.</param>
            public RaiseDelegate(object[] arguments)
            {
                this.EventArguments = arguments;
            }

            private object[] EventArguments { get; set; }

            /// <summary>
            /// Converts the RaiseDelegate to a <c>TEventHandler</c>.
            /// </summary>
            /// <param name="raiser">The RaiseDelegate to convert.</param>
            /// <returns>A new <c>TEventHandler</c> that can be attached to an event.</returns>
            public static implicit operator TEventHandler(RaiseDelegate<TEventHandler> raiser)
            {
                var fakeHandler = A.Fake<TEventHandler>();
                EventHandlerArguments[fakeHandler] = raiser.EventArguments;
                return fakeHandler;
            }
        }
    }
}