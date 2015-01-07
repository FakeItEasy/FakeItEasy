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
    internal class DelegateRaiser<TEventHandler> : IEventRaiserArguments
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateRaiser{TEventHandler}"/> class.
        /// </summary>
        /// <param name="arguments">The arguments to be sent to the event handler.</param>
        /// <param name="argumentProviderMap">A map from event handlers to supplied arguments to use when raising.</param>
        public DelegateRaiser(object[] arguments, EventHandlerArgumentProviderMap argumentProviderMap)
        {
            var delegateMethod = typeof(TEventHandler).GetMethod("Invoke");

            AssertThatValuesSatisfyCallSignature(delegateMethod, arguments);

            this.EventHandler = A.Fake<TEventHandler>();
            this.EventArguments = arguments;

            argumentProviderMap.AddArgumentProvider(this.EventHandler as Delegate, this);
        }

        private TEventHandler EventHandler { get; set; }

        private object[] EventArguments { get; set; }

        /// <summary>
        /// Converts the <c>DelegateRaiser</c> to a <c>TEventHandler</c>.
        /// </summary>
        /// <param name="raiser">The <c>DelegateRaiser</c> to convert.</param>
        /// <returns>A new <c>TEventHandler</c> that can be attached to an event.</returns>
        [SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates", Justification = "Provides the event raising syntax.")]
        public static implicit operator TEventHandler(DelegateRaiser<TEventHandler> raiser)
        {
            Guard.AgainstNull(raiser, "raiser");

            return raiser.EventHandler;
        }

        object[] IEventRaiserArguments.GetEventArguments(object fake)
        {
            return this.EventArguments;
        }

        private static void AssertThatValuesSatisfyCallSignature(MethodInfo callMethod, object[] values)
        {
            if (IsCallSignatureSatisfiedByValues(callMethod, values))
            {
                return;
            }
            
            var formatter = ServiceLocator.Current.Resolve<ArgumentValueFormatter>();

            var fakeSignature = BuildSignatureDescription(callMethod.GetParameters().Select(p => (object)p.ParameterType), formatter);
            var actionSignature = BuildSignatureDescription(values.Select(v => v == null ? null : (object)v.GetType()), formatter);

            throw new FakeConfigurationException(
                "The event has the signature ({0}), but the provided arguments have types ({1})."
                    .FormatInvariant(fakeSignature, actionSignature, "Raise"));
        }

        private static bool IsCallSignatureSatisfiedByValues(MethodInfo callMethod, object[] values)
        {
            var callMethodParameterTypes = callMethod.GetParameters().Select(p => p.ParameterType).ToList();

            if (callMethodParameterTypes.Count != values.Length)
            {
                return false;
            }

            for (int i = 0; i < callMethodParameterTypes.Count; i++)
            {
                if (values[i] == null)
                {
                    if (callMethodParameterTypes[i].IsValueType)
                    {
                        return false;
                    }
                }
                else if (!callMethodParameterTypes[i].IsInstanceOfType(values[i]))
                {
                    return false;
                }
            }

            return true;
        }

        private static string BuildSignatureDescription(IEnumerable<object> types, ArgumentValueFormatter formatter)
        {
            return types.ToCollectionString(formatter.GetArgumentValueAsString, ", ");
        }
    }
}