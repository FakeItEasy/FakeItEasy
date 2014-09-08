namespace FakeItEasy
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using FakeItEasy.Core;

    /// <summary>
    /// A class exposing an event handler to attach to an event of a faked object
    /// in order to raise that event.
    /// </summary>
    /// <typeparam name="TEventArgs">The type of the event args.</typeparam>
    public class Raise<TEventArgs>
        : IEventRaiserArguments where TEventArgs : EventArgs
    {
        private readonly TEventArgs eventArguments;
        private readonly object sender;

        internal Raise(object sender, TEventArgs e)
        {
            this.sender = sender;
            this.eventArguments = e;
        }

        /// <summary>
        /// Gets a generic event handler to attach to the event to raise.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public EventHandler<TEventArgs> Go
        {
            get { return this.Now; }
        }

        object IEventRaiserArguments.Sender
        {
            get { return this.sender; }
        }

        EventArgs IEventRaiserArguments.EventArguments
        {
            get { return this.eventArguments; }
        }

        /// <summary>
        /// Converts a raiser into a <see cref="EventHandler{TEventArgs}"/>
        /// </summary>
        /// <param name="raiser">The raiser to convert.</param>
        /// <returns>The new event handler</returns>
        public static implicit operator EventHandler<TEventArgs>(Raise<TEventArgs> raiser)
        {
            var eventHandler = new EventHandler<TEventArgs>(raiser.Now);
            Raise.EventHandlerArguments[eventHandler] =
                fake => new[] { raiser.sender ?? fake, raiser.eventArguments };
            return eventHandler;
        }

        /// <summary>
        /// Converts a raiser into a <see cref="EventHandler"/>
        /// </summary>
        /// <param name="raiser">The raiser to convert.</param>
        /// <returns>The new event handler</returns>
        public static implicit operator EventHandler(Raise<TEventArgs> raiser)
        {
            var eventHandler = new EventHandler(raiser.Now);
            Raise.EventHandlerArguments[eventHandler] = fake => new[] { raiser.sender ?? fake, raiser.eventArguments };
            return eventHandler;
        }

        /// <summary>
        /// Register this event handler to an event on a faked object in order to raise that event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">Event args for the event.</param>
        [SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers", Justification = "Must be visible to provide the event raising syntax.")]
        [SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId = "sender", Justification = "Unused parameter.")]
        public void Now(object sender, TEventArgs e)
        {
            throw new NotSupportedException(ExceptionMessages.NowCalledDirectly);
        }

        /// <summary>
        /// Register this event handler to an event on a faked object in order to raise that event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">Event args for the event.</param>
        private void Now(object sender, EventArgs e)
        {
            throw new NotSupportedException(ExceptionMessages.NowCalledDirectly);
        }
    }
}