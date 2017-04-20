namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;

    using FakeItEasy.Configuration;

    /// <summary>
    /// A class exposing an event handler to attach to a delegate-type event of a faked object
    /// in order to raise that event.
    /// </summary>
    /// <typeparam name="TEventHandler">The type of the event handler. Should be a <see cref="Delegate"/></typeparam>
    internal class DelegateRaiser<TEventHandler> : IEventRaiserArgumentProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateRaiser{TEventHandler}"/> class.
        /// </summary>
        /// <param name="arguments">The arguments to be sent to the event handler.</param>
        /// <param name="argumentProviderMap">A map from event handlers to supplied arguments to use when raising.</param>
        public DelegateRaiser(object[] arguments, EventHandlerArgumentProviderMap argumentProviderMap)
        {
            var delegateMethod = typeof(TEventHandler).GetMethod("Invoke");

            ValueProducerSignatureHelper.AssertThatValuesSatisfyCallSignature(delegateMethod, arguments);

            this.EventHandler = A.Fake<TEventHandler>();
            this.EventArguments = arguments;

            argumentProviderMap.AddArgumentProvider(this.EventHandler as Delegate, this);
        }

        private TEventHandler EventHandler { get; }

        private object[] EventArguments { get; }

        /// <summary>
        /// Converts the <c>DelegateRaiser</c> to a <c>TEventHandler</c>.
        /// </summary>
        /// <param name="raiser">The <c>DelegateRaiser</c> to convert.</param>
        /// <returns>A new <c>TEventHandler</c> that can be attached to an event.</returns>
        [SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates", Justification = "Provides the event raising syntax.")]
        public static implicit operator TEventHandler(DelegateRaiser<TEventHandler> raiser)
        {
            Guard.AgainstNull(raiser, nameof(raiser));

            return raiser.EventHandler;
        }

        object[] IEventRaiserArgumentProvider.GetEventArguments(object fake)
        {
            return this.EventArguments;
        }
    }
}
