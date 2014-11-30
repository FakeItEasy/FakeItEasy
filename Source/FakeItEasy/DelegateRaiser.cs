namespace FakeItEasy
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using FakeItEasy.Configuration;
    using FakeItEasy.Core;

    /// <summary>
    /// A class exposing an event handler to attach to a delegate-type event of a faked object
    /// in order to raise that event.
    /// </summary>
    /// <typeparam name="TEventHandler">The type of the event handler. Should be a <see cref="Delegate"/></typeparam>
    internal class DelegateRaiser<TEventHandler> : IArgumentProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateRaiser{TEventHandler}"/> class.
        /// </summary>
        /// <param name="arguments">The arguments to be sent to the event handler.</param>
        /// <param name="argumentProviderMap">A map from event handlers to supplied arguments to use when raising.</param>
        public DelegateRaiser(object[] arguments, EventHandlerArgumentProviders argumentProviderMap)
        {
            var method = typeof(TEventHandler).GetMethod("Invoke");

            Validate(method, arguments);

            this.EventHandler = A.Fake<TEventHandler>();
            this.EventArguments = arguments;

            argumentProviderMap[this.EventHandler as Delegate] = this;
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

        object[] IArgumentProvider.GetArguments(object fake)
        {
            return this.EventArguments;
        }

        private static void Validate(MethodInfo method, IList<object> arguments)
        {
            var parameterTypes = method.GetParameters().Select(p => p.ParameterType).ToList();

            if (parameterTypes.Count != arguments.Count)
            {
                Mismatch(method, arguments);
            }

            for (var i = 0; i < parameterTypes.Count; ++i)
            {
                if (arguments[i] == null)
                {
                    if (parameterTypes[i].IsValueType)
                    {
                        Mismatch(method, arguments);
                    }
                }
                else if (!parameterTypes[i].IsInstanceOfType(arguments[i]))
                {
                    Mismatch(method, arguments);
                }
            }
        }

        private static void Mismatch(MethodInfo method, IEnumerable<object> arguments)
        {
            var formatter = ServiceLocator.Current.Resolve<ArgumentValueFormatter>();

            var eventSignature = method.GetParameters()
                .Select(parameter => (object)parameter.ParameterType)
                .ToCollectionString(formatter.GetArgumentValueAsString, ", ");

            var argumentsSignature = arguments.Select(argument => argument == null ? null : (object)argument.GetType())
                .ToCollectionString(formatter.GetArgumentValueAsString, ", ");

            throw new FakeConfigurationException(
                "The event has the signature ({0}), but the provided arguments have types ({1})."
                    .FormatInvariant(eventSignature, argumentsSignature, "Raise"));
        }
    }
}