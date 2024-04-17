namespace FakeItEasy.Creation.CastleDynamicProxy
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Castle.DynamicProxy;
    using FakeItEasy.Core;

    internal static class CastleDynamicProxyGenerator
    {
        private static readonly IProxyGenerationHook ProxyGenerationHook = new InterceptEverythingHook();
        private static readonly ProxyGenerator ProxyGenerator = new ProxyGenerator();

        [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Appropriate in Try-style methods")]
        public static ProxyGeneratorResult GenerateInterfaceProxy(
            Type typeOfProxy,
            ReadOnlyCollection<Type> additionalInterfacesToImplement,
            IEnumerable<Expression<Func<Attribute>>> attributes,
            IFakeCallProcessorProvider fakeCallProcessorProvider)
        {
            Guard.AgainstNull(typeOfProxy);
            Guard.AgainstNull(additionalInterfacesToImplement);
            Guard.AgainstNull(attributes);
            Guard.AgainstNull(fakeCallProcessorProvider);

            var options = CreateProxyGenerationOptions();
            foreach (var attribute in attributes)
            {
                options.AdditionalAttributes.Add(CustomAttributeInfo.FromExpression(attribute));
            }

            var allInterfacesToImplement = new Type[1 + additionalInterfacesToImplement.Count];
            additionalInterfacesToImplement.CopyTo(allInterfacesToImplement, 1);
            allInterfacesToImplement[0] = typeOfProxy;

            object proxy;
            try
            {
                proxy = ProxyGenerator.CreateClassProxy(
                    typeof(object),
                    allInterfacesToImplement,
                    options,
                    constructorArguments: null,
                    new ProxyInterceptor(fakeCallProcessorProvider));
            }
            catch (Exception e)
            {
                return GetProxyResultForNoDefaultConstructor(typeOfProxy, e);
            }

            fakeCallProcessorProvider.EnsureInitialized(proxy);
            return new ProxyGeneratorResult(generatedProxy: proxy);
        }

        [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Appropriate in Try-style methods")]
        public static ProxyGeneratorResult GenerateClassProxy(
            Type typeOfProxy,
            ReadOnlyCollection<Type> additionalInterfacesToImplement,
            IEnumerable<object?> argumentsForConstructor,
            IEnumerable<Expression<Func<Attribute>>> attributes,
            IFakeCallProcessorProvider fakeCallProcessorProvider)
        {
            Guard.AgainstNull(typeOfProxy);
            Guard.AgainstNull(additionalInterfacesToImplement);
            Guard.AgainstNull(attributes);
            Guard.AgainstNull(argumentsForConstructor);
            Guard.AgainstNull(fakeCallProcessorProvider);

            if (!CanGenerateProxy(typeOfProxy, out string? failReason))
            {
                return new ProxyGeneratorResult(failReason);
            }

            var options = CreateProxyGenerationOptions();
            foreach (var attribute in attributes)
            {
                options.AdditionalAttributes.Add(CustomAttributeInfo.FromExpression(attribute));
            }

            Type[] allInterfacesToImplement;
            if (additionalInterfacesToImplement.Count == 0)
            {
                allInterfacesToImplement = Type.EmptyTypes;
            }
            else
            {
                allInterfacesToImplement = new Type[additionalInterfacesToImplement.Count];
                additionalInterfacesToImplement.CopyTo(allInterfacesToImplement, 0);
            }

            var argumentsArray = argumentsForConstructor.ToArray();

            object proxy;
            try
            {
                proxy = ProxyGenerator.CreateClassProxy(
                    typeOfProxy,
                    allInterfacesToImplement,
                    options,
                    argumentsArray,
                    new ProxyInterceptor(fakeCallProcessorProvider));
            }
            catch (Exception e)
            {
                return GetResultForFailedProxyGeneration(typeOfProxy, argumentsArray, e);
            }

            fakeCallProcessorProvider.EnsureInitialized(proxy);
            return new ProxyGeneratorResult(generatedProxy: proxy);
        }

        public static bool CanGenerateProxy(Type typeOfProxy, [NotNullWhen(false)] out string? failReason)
        {
            if (typeOfProxy.IsSealed)
            {
                failReason = DynamicProxyMessages.ProxyIsSealedType(typeOfProxy);
                return false;
            }

            failReason = null;
            return true;
        }

        private static ProxyGenerationOptions CreateProxyGenerationOptions()
        {
            return new ProxyGenerationOptions(ProxyGenerationHook);
        }

        private static ProxyGeneratorResult GetResultForFailedProxyGeneration(Type typeOfProxy, IEnumerable<object?> argumentsForConstructor, Exception e) =>
            argumentsForConstructor.Any()
                ? new ProxyGeneratorResult(e)
                : GetProxyResultForNoDefaultConstructor(typeOfProxy, e);

        private static ProxyGeneratorResult GetProxyResultForNoDefaultConstructor(Type typeOfProxy, Exception e)
        {
            return new ProxyGeneratorResult(DynamicProxyMessages.ProxyTypeWithNoDefaultConstructor(typeOfProxy), e);
        }

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

            public override bool Equals(object? obj)
            {
                return obj is InterceptEverythingHook;
            }
        }
    }
}
