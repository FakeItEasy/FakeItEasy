namespace FakeItEasy.Core
{
    using System;
    using System.Diagnostics.CodeAnalysis;
#if FEATURE_NETCORE_REFLECTION
    using System.Reflection;
#endif

    /// <summary>
    /// A class exposing an event handler to attach to a delegate-type event of a faked object
    /// in order to raise that event.
    /// </summary>
    /// <typeparam name="TEventHandler">The type of the event handler.</typeparam>
    internal class DelegateRaiser<TEventHandler> : IEventRaiserArgumentProvider where TEventHandler : Delegate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateRaiser{TEventHandler}"/> class.
        /// </summary>
        /// <param name="arguments">The arguments to be sent to the event handler.</param>
        /// <param name="argumentProviderMap">A map from event handlers to supplied arguments to use when raising.</param>
        public DelegateRaiser(object?[] arguments, EventHandlerArgumentProviderMap argumentProviderMap)
        {
            var delegateMethod = typeof(TEventHandler).GetMethod("Invoke");

            ValueProducerSignatureHelper.AssertThatValuesSatisfyCallSignature(delegateMethod, arguments);

            this.EventHandler = A.Fake<TEventHandler>();
            this.EventArguments = arguments;

            argumentProviderMap.AddArgumentProvider(this.EventHandler, this);
        }

        private TEventHandler EventHandler { get; }

        private object?[] EventArguments { get; }

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

        object?[] IEventRaiserArgumentProvider.GetEventArguments(object fake)
        {
            return this.EventArguments;
        }
    }
}
