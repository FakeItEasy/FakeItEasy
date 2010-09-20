namespace FakeItEasy.DynamicProxy
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using Castle.DynamicProxy;
    using FakeItEasy.Core;
    using FakeItEasy.Core.Creation;
    
    public class CastleDynamicProxyGenerator
        : IProxyGenerator
    {
        private static readonly Logger logger = Log.GetLogger<CastleDynamicProxyGenerator>();
        private static readonly ProxyGenerationOptions proxyGenerationOptions = new ProxyGenerationOptions { Hook = new InterceptEverythingHook() };
        private static readonly ProxyGenerator proxyGenerator = new ProxyGenerator();

        public ProxyGeneratorResult GenerateProxy(Type typeOfProxy, IEnumerable<Type> additionalInterfacesToImplement, IEnumerable<object> argumentsForConstructor)
        {
            Guard.AgainstNull(typeOfProxy, "typeOfProxy");
            Guard.AgainstNull(additionalInterfacesToImplement, "additionalInterfacesToImplement");
            GuardAgainstConstructorArgumentsForInterfaceType(typeOfProxy, argumentsForConstructor);

            if (typeOfProxy.IsValueType)
            {
                return GetProxyResultForValueType(typeOfProxy);
            }

            return this.CreateProxyGeneratorResult(typeOfProxy, additionalInterfacesToImplement, argumentsForConstructor);
        }

        private static void GuardAgainstConstructorArgumentsForInterfaceType(Type typeOfProxy, IEnumerable<object> argumentsForConstructor)
        {
            if (typeOfProxy.IsInterface && argumentsForConstructor != null)
            {
                throw new ArgumentException(DynamicProxyResources.ArgumentsForConstructorOnInterfaceTypeMessage);
            }
        }

        private ProxyGeneratorResult CreateProxyGeneratorResult(Type typeOfProxy, IEnumerable<Type> additionalInterfacesToImplement, IEnumerable<object> argumentsForConstructor)
        {
            var interceptor = new ProxyInterceptor();
            
            try
            {
                var proxy = this.DoGenerateProxy(
                    typeOfProxy, 
                    additionalInterfacesToImplement, 
                    argumentsForConstructor,
                    interceptor);

                return new ProxyGeneratorResult(
                    generatedProxy: proxy, 
                    callInterceptedEventRaiser: interceptor);
            }
            catch
            {
                return GetResultForFailedProxyGeneration(typeOfProxy, argumentsForConstructor);
            }
        }

        public bool MemberCanBeIntercepted(MemberInfo member)
        {
            Guard.AgainstNull(member, "member");

            var isNonInterceptableMember =
                IsNonVirtualMethod(member) ||
                IsNonVirtualProperty(member);

            return !isNonInterceptableMember;
        }

        private static ProxyGeneratorResult GetResultForFailedProxyGeneration(Type typeOfProxy, IEnumerable<object> argumentsForConstructor)
        {
            if (argumentsForConstructor != null)
            {
                return new ProxyGeneratorResult(DynamicProxyResources.ArgumentsForConstructorDoesNotMatchAnyConstructorMessage);
            }

            return GetProxyResultForNoDefaultConstructor(typeOfProxy);
        }

        private static bool IsNonVirtualMethod(MemberInfo member)
        {
            var method = member as MethodInfo;
            return method != null && !method.IsVirtual;
        }

        private static bool IsNonVirtualProperty(MemberInfo member)
        {
            var property = member as PropertyInfo;
            return property != null && !PropertyIsVirtual(property);
        }

        private static bool PropertyIsVirtual(PropertyInfo property)
        {
            var getMethod = property.GetGetMethod();
            var setMethod = property.GetSetMethod();

            return 
                (getMethod == null || getMethod.IsVirtual) && 
                (setMethod == null || setMethod.IsVirtual);
        }

        private static ProxyGeneratorResult GetProxyResultForNoDefaultConstructor(Type typeOfProxy)
        {
            return new ProxyGeneratorResult(string.Format(CultureInfo.CurrentCulture, DynamicProxyResources.ProxyTypeWithNoDefaultConstructorMessage, typeOfProxy));
        }

        private static ProxyGeneratorResult GetProxyResultForValueType(Type typeOfProxy)
        {
            return new ProxyGeneratorResult(string.Format(CultureInfo.CurrentCulture, DynamicProxyResources.ProxyIsValueTypeMessage, typeOfProxy));
        }

        private object DoGenerateProxy(Type typeOfProxy, IEnumerable<Type> additionalInterfacesToImplement, IEnumerable<object> argumentsForConstructor, IInterceptor interceptor)
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

            return proxyGenerator.CreateClassProxy(
                typeOfProxy,
                allInterfacesToImplement.ToArray(),
                proxyGenerationOptions,
                argumentsArray,
                interceptor);
        }

        private static object[] GetConstructorArgumentsArray(IEnumerable<object> argumentsForConstructor)
        {
            return argumentsForConstructor != null ? argumentsForConstructor.ToArray() : null;
        }

        private static IEnumerable<Type> GetAllInterfacesToImplement(IEnumerable<Type> additionalInterfacesToImplement)
        {
            return additionalInterfacesToImplement.Concat(new[] { typeof(ITaggable) });
        }

        [Serializable]
        private class InterceptEverythingHook 
            : IProxyGenerationHook
        {
            public void MethodsInspected()
            {
            }

            public void NonProxyableMemberNotification(Type type, MemberInfo memberInfo)
            {
                logger.Debug("Non interceptable member: {0}.{1}.", type, memberInfo.Name);
            }

            public bool ShouldInterceptMethod(Type type, MethodInfo methodInfo)
            {
                logger.Debug("Intercept member: {0}.{1}.", type, methodInfo);
                return true;
            }
        }

        [Serializable]
        private class ProxyInterceptor 
            : IInterceptor, ICallInterceptedEventRaiser
        {
            private static readonly MethodInfo tagGetMethod = typeof(ITaggable).GetProperty("Tag").GetGetMethod();
            private static readonly MethodInfo tagSetMethod = typeof(ITaggable).GetProperty("Tag").GetSetMethod();

            private object tag;

            public void Intercept(IInvocation invocation)
            {
                if (invocation.Method.Equals(tagGetMethod))
                {
                    invocation.ReturnValue = this.tag;
                }
                else if (invocation.Method.Equals(tagSetMethod))
                {
                    this.tag = invocation.Arguments[0];
                }
                else
                {
                    this.RaiseCallWasIntercepted(invocation);
                }
            }

            private void RaiseCallWasIntercepted(IInvocation invocation)
            {
                logger.Debug("Call was intercepted: {0}.", invocation.Method);
                var handler = this.CallWasIntercepted;
                if (handler != null)
                {
                    var call = new CastleInvocationCallAdapter(invocation);
                    handler.Invoke(this, new CallInterceptedEventArgs(call));
                }
            }

            public event EventHandler<CallInterceptedEventArgs> CallWasIntercepted;
        }

    }
}