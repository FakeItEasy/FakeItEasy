namespace FakeItEasy.Core
{
    using System;
    using System.Dynamic;
#if FEATURE_NETCORE_REFLECTION
    using System.Reflection;
#endif
    using FakeItEasy.Sdk;

    internal class DynamicRaiser : DynamicObject, IEventRaiserArgumentProvider
    {
        private readonly object?[] arguments;
        private readonly EventHandlerArgumentProviderMap argumentProviderMap;

        public DynamicRaiser(object?[] arguments, EventHandlerArgumentProviderMap argumentProviderMap)
        {
            this.arguments = arguments;
            this.argumentProviderMap = argumentProviderMap;
        }

        object?[] IEventRaiserArgumentProvider.GetEventArguments(object fake)
        {
            return this.arguments;
        }

        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            var targetType = binder.Type;
            if (!targetType.GetTypeInfo().IsSubclassOf(typeof(Delegate)))
            {
                throw new InvalidCastException($"Unable to cast object of type '{this.GetType()}' to type '{targetType}'.");
            }

            var method = targetType.GetMethod("Invoke");
            ValueProducerSignatureHelper.AssertThatValuesSatisfyCallSignature(method, this.arguments);
            var @delegate = (Delegate)Create.Fake(targetType);
            this.argumentProviderMap.AddArgumentProvider(@delegate, this);
            result = @delegate;
            return true;
        }
    }
}
