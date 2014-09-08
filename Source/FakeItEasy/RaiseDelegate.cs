namespace FakeItEasy
{
    using System;

    /// <summary>
    /// A class exposing an event handler to attach to a delegate-type event of a faked object
    /// in order to raise that event.
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
            this.EventHandler = A.Fake<TEventHandler>();
        }

        private TEventHandler EventHandler { get; set; }

        private object[] EventArguments { get; set; }

        /// <summary>
        /// Converts the <c>RaiseDelegate</c> to a <c>TEventHandler</c>.
        /// </summary>
        /// <param name="raiser">The <c>RaiseDelegate</c> to convert.</param>
        /// <returns>A new <c>TEventHandler</c> that can be attached to an event.</returns>
        public static implicit operator TEventHandler(RaiseDelegate<TEventHandler> raiser)
        {
            Raise.EventHandlerArguments[raiser.EventHandler] = fake => raiser.EventArguments;
            return raiser.EventHandler;
        }
    }
}