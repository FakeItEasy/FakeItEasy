namespace FakeItEasy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using FakeItEasy.Configuration;
    using FakeItEasy.Core;

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
            var delegateMethod = typeof(TEventHandler).GetMethod("Invoke");

            AssertThatValuesSatisfyCallSignature(delegateMethod, arguments);

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
            var providerMap = ServiceLocator.Current.Resolve<EventHandlerArgumentProviderMap>();
            providerMap.AddArgumentProvider(raiser.EventHandler, fake => raiser.EventArguments);
            return raiser.EventHandler;
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