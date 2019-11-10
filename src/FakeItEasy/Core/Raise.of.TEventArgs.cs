namespace FakeItEasy.Core
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// A class exposing an event handler to attach to an event of a faked object
    /// in order to raise that event.
    /// </summary>
    /// <typeparam name="TEventArgs">The type of the event args.</typeparam>
    public class Raise<TEventArgs> : IEventRaiserArgumentProvider
#if FEATURE_EVENT_ARGS_MUST_EXTEND_EVENTARGS
        where TEventArgs : EventArgs
#endif
    {
        private readonly TEventArgs eventArguments;
        private readonly object? eventSender;
        private readonly bool hasSender;

        /// <summary>
        /// Initializes a new instance of the <see cref="Raise{TEventArgs}"/> class.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event data.</param>
        /// <param name="argumentProviderMap">A map from event handlers to supplied arguments to use when raising.</param>
        internal Raise(object? sender, TEventArgs e, EventHandlerArgumentProviderMap argumentProviderMap)
        {
            this.eventSender = sender;
            this.hasSender = true;
            this.eventArguments = e;
            argumentProviderMap.AddArgumentProvider((EventHandler<TEventArgs>)this, this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Raise{TEventArgs}"/> class. The sender will be
        /// the fake that raises the event.
        /// </summary>
        /// <param name="e">The event data.</param>
        /// <param name="argumentProviderMap">A map from event handlers to supplied arguments to use when raising.</param>
        internal Raise(TEventArgs e, EventHandlerArgumentProviderMap argumentProviderMap)
        {
            this.hasSender = false;
            this.eventArguments = e;
            argumentProviderMap.AddArgumentProvider((EventHandler<TEventArgs>)this, this);
        }

        /// <summary>
        /// Converts a raiser into an <see cref="EventHandler{TEventArgs}"/>
        /// </summary>
        /// <param name="raiser">The raiser to convert.</param>
        /// <returns>The new event handler</returns>
        [SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates", Justification = "Provides the event raising syntax.")]
        public static implicit operator EventHandler<TEventArgs>(Raise<TEventArgs> raiser)
        {
            Guard.AgainstNull(raiser, nameof(raiser));

            return raiser.Now;
        }

        /// <summary>
        /// Converts a raiser into an <see cref="EventHandler"/>
        /// </summary>
        /// <param name="raiser">The raiser to convert.</param>
        /// <returns>The new event handler</returns>
        [SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates", Justification = "Provides the event raising syntax.")]
        public static implicit operator EventHandler(Raise<TEventArgs> raiser)
        {
            Guard.AgainstNull(raiser, nameof(raiser));

            return raiser.Now;
        }

        object?[] IEventRaiserArgumentProvider.GetEventArguments(object fake)
        {
            return new[] { this.hasSender ? this.eventSender : fake, this.eventArguments };
        }

#pragma warning disable CA1822 // Mark members as static
        private void Now(object sender, TEventArgs e)
#pragma warning restore CA1822 // Mark members as static
        {
            throw new NotSupportedException(ExceptionMessages.NowCalledDirectly);
        }

        private void Now(object sender, EventArgs e)
        {
            throw new NotSupportedException(ExceptionMessages.NowCalledDirectly);
        }
    }
}
