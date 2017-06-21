namespace FakeItEasy.Creation.CastleDynamicProxy
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.Serialization;
    using Castle.DynamicProxy;
    using FakeItEasy.Core;

    internal class CastleDynamicProxyGenerator
        : FakeItEasy.Creation.IProxyGenerator
    {
        private static readonly IProxyGenerationHook ProxyGenerationHook = new InterceptEverythingHook();
        private static readonly ProxyGenerator ProxyGenerator = new ProxyGenerator();
        private readonly CastleDynamicProxyInterceptionValidator interceptionValidator;

        public CastleDynamicProxyGenerator(CastleDynamicProxyInterceptionValidator interceptionValidator)
        {
            this.interceptionValidator = interceptionValidator;
        }

        public ProxyGeneratorResult GenerateProxy(
            Type typeOfProxy,
            IEnumerable<Type> additionalInterfacesToImplement,
            IEnumerable<object> argumentsForConstructor,
            IEnumerable<Expression<Func<Attribute>>> attributes,
            IFakeCallProcessorProvider fakeCallProcessorProvider)
        {
            Guard.AgainstNull(attributes, nameof(attributes));

            var options = CreateProxyGenerationOptions();
            foreach (var attribute in attributes)
            {
                options.AdditionalAttributes.Add(CustomAttributeInfo.FromExpression(attribute));
            }

            return GenerateProxy(typeOfProxy, options, additionalInterfacesToImplement, argumentsForConstructor, fakeCallProcessorProvider);
        }

        public ProxyGeneratorResult GenerateProxy(
            Type typeOfProxy,
            IEnumerable<Type> additionalInterfacesToImplement,
            IEnumerable<object> argumentsForConstructor,
            IFakeCallProcessorProvider fakeCallProcessorProvider)
        {
            var options = CreateProxyGenerationOptions();
            return GenerateProxy(typeOfProxy, options, additionalInterfacesToImplement, argumentsForConstructor, fakeCallProcessorProvider);
        }

        public bool MethodCanBeInterceptedOnInstance(MethodInfo method, object callTarget, out string failReason)
        {
            return this.interceptionValidator.MethodCanBeInterceptedOnInstance(method, callTarget, out failReason);
        }

        private static ProxyGenerationOptions CreateProxyGenerationOptions()
        {
            return new ProxyGenerationOptions(ProxyGenerationHook);
        }

        private static ProxyGeneratorResult GenerateProxy(
            Type typeOfProxy,
            ProxyGenerationOptions options,
            IEnumerable<Type> additionalInterfacesToImplement,
            IEnumerable<object> argumentsForConstructor,
            IFakeCallProcessorProvider fakeCallProcessorProvider)
        {
            Guard.AgainstNull(typeOfProxy, nameof(typeOfProxy));
            Guard.AgainstNull(additionalInterfacesToImplement, nameof(additionalInterfacesToImplement));
            Guard.AgainstNull(fakeCallProcessorProvider, nameof(fakeCallProcessorProvider));

            if (typeOfProxy.GetTypeInfo().IsValueType)
            {
                return GetProxyResultForValueType(typeOfProxy);
            }

            if (typeOfProxy.GetTypeInfo().IsSealed)
            {
                return new ProxyGeneratorResult(DynamicProxyMessages.ProxyIsSealedType(typeOfProxy));
            }

            GuardAgainstConstructorArgumentsForInterfaceType(typeOfProxy, argumentsForConstructor);

            return CreateProxyGeneratorResult(typeOfProxy, options, additionalInterfacesToImplement, argumentsForConstructor, fakeCallProcessorProvider);
        }

        private static void GuardAgainstConstructorArgumentsForInterfaceType(Type typeOfProxy, IEnumerable<object> argumentsForConstructor)
        {
            if (typeOfProxy.GetTypeInfo().IsInterface && argumentsForConstructor != null)
            {
                throw new ArgumentException(DynamicProxyMessages.ArgumentsForConstructorOnInterfaceType);
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Appropriate since the method tries to create a proxy and returns a result object where success is reported.")]
        private static ProxyGeneratorResult CreateProxyGeneratorResult(
            Type typeOfProxy,
            ProxyGenerationOptions options,
            IEnumerable<Type> additionalInterfacesToImplement,
            IEnumerable<object> argumentsForConstructor,
            IFakeCallProcessorProvider fakeCallProcessorProvider)
        {
            var interceptor = new ProxyInterceptor(fakeCallProcessorProvider);
            object proxy;

            try
            {
                proxy = DoGenerateProxy(
                    typeOfProxy,
                    options,
                    additionalInterfacesToImplement,
                    argumentsForConstructor,
                    interceptor);
            }
            catch (Exception e)
            {
                return GetResultForFailedProxyGeneration(typeOfProxy, argumentsForConstructor, e);
            }

            fakeCallProcessorProvider.EnsureInitialized(proxy);

            return new ProxyGeneratorResult(generatedProxy: proxy);
        }

        private static ProxyGeneratorResult GetResultForFailedProxyGeneration(Type typeOfProxy, IEnumerable<object> argumentsForConstructor, Exception e)
        {
            if (argumentsForConstructor != null)
            {
                return new ProxyGeneratorResult(DynamicProxyMessages.ArgumentsForConstructorDoesNotMatchAnyConstructor, e);
            }

            return GetProxyResultForNoDefaultConstructor(typeOfProxy, e);
        }

        private static ProxyGeneratorResult GetProxyResultForNoDefaultConstructor(Type typeOfProxy, Exception e)
        {
            return new ProxyGeneratorResult(DynamicProxyMessages.ProxyTypeWithNoDefaultConstructor(typeOfProxy), e);
        }

        private static ProxyGeneratorResult GetProxyResultForValueType(Type typeOfProxy)
        {
            return new ProxyGeneratorResult(DynamicProxyMessages.ProxyIsValueType(typeOfProxy));
        }

        private static object DoGenerateProxy(
            Type typeOfProxy,
            ProxyGenerationOptions options,
            IEnumerable<Type> additionalInterfacesToImplement,
            IEnumerable<object> argumentsForConstructor,
            IInterceptor interceptor)
        {
            var allInterfacesToImplement = additionalInterfacesToImplement;

            if (typeOfProxy.GetTypeInfo().IsInterface)
            {
                allInterfacesToImplement = new[] { typeOfProxy }.Concat(allInterfacesToImplement);
                typeOfProxy = typeof(object);
            }

            return GenerateClassProxy(typeOfProxy, options, argumentsForConstructor, interceptor, allInterfacesToImplement);
        }

        private static object GenerateClassProxy(
            Type typeOfProxy,
            ProxyGenerationOptions options,
            IEnumerable<object> argumentsForConstructor,
            IInterceptor interceptor,
            IEnumerable<Type> allInterfacesToImplement)
        {
            var argumentsArray = argumentsForConstructor?.ToArray();

            return ProxyGenerator.CreateClassProxy(
                typeOfProxy,
                allInterfacesToImplement.ToArray(),
                options,
                argumentsArray,
                interceptor);
        }

#if FEATURE_BINARY_SERIALIZATION
        [Serializable]
#endif
        private class InterceptEverythingHook
            : IProxyGenerationHook
        {
            private static readonly int HashCode = typeof(InterceptEverythingHook).GetHashCode();

            public void MethodsInspected()
            {
            }

            public void NonProxyableMemberNotification(Type type, MemberInfo memberInfo)
            {
            }

            public bool ShouldInterceptMethod(Type type, MethodInfo methodInfo)
            {
                return true;
            }

            public override int GetHashCode()
            {
                return HashCode;
            }

            public override bool Equals(object obj)
            {
                return obj is InterceptEverythingHook;
            }
        }

#if FEATURE_BINARY_SERIALIZATION
        [Serializable]
#endif
        private class ProxyInterceptor
            : IInterceptor
        {
            private readonly IFakeCallProcessorProvider fakeCallProcessorProvider;

            public ProxyInterceptor(IFakeCallProcessorProvider fakeCallProcessorProvider)
            {
                this.fakeCallProcessorProvider = fakeCallProcessorProvider;
            }

            public void Intercept(IInvocation invocation)
            {
                Guard.AgainstNull(invocation, nameof(invocation));
                var call = new CastleInvocationCallAdapter(invocation);
                this.fakeCallProcessorProvider.Fetch(invocation.Proxy).Process(call);
            }

#if FEATURE_BINARY_SERIALIZATION
            [OnDeserialized]
            public void OnDeserialized(StreamingContext context)
            {
                this.fakeCallProcessorProvider.EnsureManagerIsRegistered();
            }
#endif
        }
    }
}
