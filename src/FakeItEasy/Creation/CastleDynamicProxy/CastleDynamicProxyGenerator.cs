namespace FakeItEasy.Creation.CastleDynamicProxy
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using Castle.DynamicProxy;
    using FakeItEasy.Core;

    internal class CastleDynamicProxyGenerator
        : IProxyGenerator
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
            IEnumerable<CustomAttributeBuilder> customAttributeBuilders,
            IFakeCallProcessorProvider fakeCallProcessorProvider)
        {
            Guard.AgainstNull(customAttributeBuilders, nameof(customAttributeBuilders));

            var options = CreateProxyGenerationOptions();
            foreach (CustomAttributeBuilder builder in customAttributeBuilders)
            {
                options.AdditionalAttributes.Add(builder);
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
                return new ProxyGeneratorResult(DynamicProxyResources.ProxyIsSealedTypeMessage.FormatInvariant(typeOfProxy));
            }

            GuardAgainstConstructorArgumentsForInterfaceType(typeOfProxy, argumentsForConstructor);

            return CreateProxyGeneratorResult(typeOfProxy, options, additionalInterfacesToImplement, argumentsForConstructor, fakeCallProcessorProvider);
        }

        private static void GuardAgainstConstructorArgumentsForInterfaceType(Type typeOfProxy, IEnumerable<object> argumentsForConstructor)
        {
            if (typeOfProxy.GetTypeInfo().IsInterface && argumentsForConstructor != null)
            {
                throw new ArgumentException(DynamicProxyResources.ArgumentsForConstructorOnInterfaceTypeMessage);
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
                return new ProxyGeneratorResult(DynamicProxyResources.ArgumentsForConstructorDoesNotMatchAnyConstructorMessage, e);
            }

            return GetProxyResultForNoDefaultConstructor(typeOfProxy, e);
        }

        private static ProxyGeneratorResult GetProxyResultForNoDefaultConstructor(Type typeOfProxy, Exception e)
        {
            return new ProxyGeneratorResult(string.Format(CultureInfo.CurrentCulture, DynamicProxyResources.ProxyTypeWithNoDefaultConstructorMessage, typeOfProxy), e);
        }

        private static ProxyGeneratorResult GetProxyResultForValueType(Type typeOfProxy)
        {
            return new ProxyGeneratorResult(string.Format(CultureInfo.CurrentCulture, DynamicProxyResources.ProxyIsValueTypeMessage, typeOfProxy));
        }

        private static object DoGenerateProxy(
            Type typeOfProxy,
            ProxyGenerationOptions options,
            IEnumerable<Type> additionalInterfacesToImplement,
            IEnumerable<object> argumentsForConstructor,
            IInterceptor interceptor)
        {
            var allInterfacesToImplement = GetAllInterfacesToImplement(additionalInterfacesToImplement);

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

        private static IEnumerable<Type> GetAllInterfacesToImplement(IEnumerable<Type> additionalInterfacesToImplement)
        {
            return additionalInterfacesToImplement.Concat(typeof(ITaggable));
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
            private static readonly MethodInfo TagGetMethod = typeof(ITaggable).GetProperty("Tag").GetGetMethod();
            private static readonly MethodInfo TagSetMethod = typeof(ITaggable).GetProperty("Tag").GetSetMethod();

            private readonly IFakeCallProcessorProvider fakeCallProcessorProvider;

            private object tag;

            public ProxyInterceptor(IFakeCallProcessorProvider fakeCallProcessorProvider)
            {
                this.fakeCallProcessorProvider = fakeCallProcessorProvider;
            }

            public void Intercept(IInvocation invocation)
            {
                Guard.AgainstNull(invocation, nameof(invocation));

                if (invocation.Method.Equals(TagGetMethod))
                {
                    invocation.ReturnValue = this.tag;
                }
                else if (invocation.Method.Equals(TagSetMethod))
                {
                    this.tag = invocation.Arguments[0];
                }
                else
                {
                    var call = new CastleInvocationCallAdapter(invocation);
                    this.fakeCallProcessorProvider.Fetch(invocation.Proxy).Process(call);
                }
            }
        }
    }
}
