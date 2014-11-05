namespace FakeItEasy.Creation.CastleDynamicProxy
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Security.Permissions;
    using Castle.DynamicProxy;
    using Castle.DynamicProxy.Generators;
    using Core;

    internal class CastleDynamicProxyGenerator
        : IProxyGenerator
    {
        private static readonly ProxyGenerationOptions ProxyGenerationOptions = new ProxyGenerationOptions { Hook = new InterceptEverythingHook() };
        private static readonly ProxyGenerator ProxyGenerator = new ProxyGenerator();
        private readonly CastleDynamicProxyInterceptionValidator interceptionValidator;

        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "No field initialization.")]
        static CastleDynamicProxyGenerator()
        {
#pragma warning disable 618
            AttributesToAvoidReplicating.Add(typeof(SecurityPermissionAttribute));
#pragma warning restore 618
        }

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
            Guard.AgainstNull(customAttributeBuilders, "customAttributeBuilders");
            Guard.AgainstNull(fakeCallProcessorProvider, "fakeCallProcessorProvider");

            ProxyGenerationOptions.AdditionalAttributes.Clear();
            foreach (CustomAttributeBuilder builder in customAttributeBuilders)
            {
                ProxyGenerationOptions.AdditionalAttributes.Add(builder);
            }

            return this.GenerateProxy(typeOfProxy, additionalInterfacesToImplement, argumentsForConstructor, fakeCallProcessorProvider);
        }

        public ProxyGeneratorResult GenerateProxy(Type typeOfProxy, IEnumerable<Type> additionalInterfacesToImplement, IEnumerable<object> argumentsForConstructor, IFakeCallProcessorProvider fakeCallProcessorProvider)
        {
            Guard.AgainstNull(typeOfProxy, "typeOfProxy");
            Guard.AgainstNull(additionalInterfacesToImplement, "additionalInterfacesToImplement");
            Guard.AgainstNull(fakeCallProcessorProvider, "fakeCallProcessorProvider");

            if (typeOfProxy.IsValueType)
            {
                return GetProxyResultForValueType(typeOfProxy);
            }

            if (typeOfProxy.IsSealed)
            {
                return new ProxyGeneratorResult(DynamicProxyResources.ProxyIsSealedTypeMessage.FormatInvariant(typeOfProxy));
            }

            GuardAgainstConstructorArgumentsForInterfaceType(typeOfProxy, argumentsForConstructor);

            return CreateProxyGeneratorResult(typeOfProxy, additionalInterfacesToImplement, argumentsForConstructor, fakeCallProcessorProvider);
        }

        public bool MethodCanBeInterceptedOnInstance(MethodInfo method, object callTarget, out string failReason)
        {
            return this.interceptionValidator.MethodCanBeInterceptedOnInstance(method, callTarget, out failReason);
        }

        private static void GuardAgainstConstructorArgumentsForInterfaceType(Type typeOfProxy, IEnumerable<object> argumentsForConstructor)
        {
            if (typeOfProxy.IsInterface && argumentsForConstructor != null)
            {
                throw new ArgumentException(DynamicProxyResources.ArgumentsForConstructorOnInterfaceTypeMessage);
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Appropriate since the method tries to create a proxy and returns a result object where success is reported.")]
        private static ProxyGeneratorResult CreateProxyGeneratorResult(Type typeOfProxy, IEnumerable<Type> additionalInterfacesToImplement, IEnumerable<object> argumentsForConstructor, IFakeCallProcessorProvider fakeCallProcessorProvider)
        {
            var interceptor = new ProxyInterceptor(fakeCallProcessorProvider);
            object proxy;

            try
            {
                proxy = DoGenerateProxy(
                    typeOfProxy,
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

        private static object DoGenerateProxy(Type typeOfProxy, IEnumerable<Type> additionalInterfacesToImplement, IEnumerable<object> argumentsForConstructor, IInterceptor interceptor)
        {
            var allInterfacesToImplement = GetAllInterfacesToImplement(additionalInterfacesToImplement);

            if (typeOfProxy.IsInterface)
            {
                allInterfacesToImplement = new[] { typeOfProxy }.Concat(allInterfacesToImplement);
                typeOfProxy = typeof(object);
            }

            return GenerateClassProxy(typeOfProxy, argumentsForConstructor, interceptor, allInterfacesToImplement);
        }

        private static object GenerateClassProxy(Type typeOfProxy, IEnumerable<object> argumentsForConstructor, IInterceptor interceptor, IEnumerable<Type> allInterfacesToImplement)
        {
            var argumentsArray = GetConstructorArgumentsArray(argumentsForConstructor);

            return ProxyGenerator.CreateClassProxy(
                typeOfProxy,
                allInterfacesToImplement.ToArray(),
                ProxyGenerationOptions,
                argumentsArray,
                interceptor);
        }

        private static object[] GetConstructorArgumentsArray(IEnumerable<object> argumentsForConstructor)
        {
            return argumentsForConstructor != null ? argumentsForConstructor.ToArray() : null;
        }

        private static IEnumerable<Type> GetAllInterfacesToImplement(IEnumerable<Type> additionalInterfacesToImplement)
        {
            return additionalInterfacesToImplement.Concat(typeof(ITaggable));
        }

        [Serializable]
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
                var other = obj as InterceptEverythingHook;
                return other != null;
            }
        }

        [Serializable]
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
                Guard.AgainstNull(invocation, "invocation");

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