namespace FakeItEasy.DynamicProxy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Castle.Core.Interceptor;
    using Castle.DynamicProxy;
    using FakeItEasy.Core;
    using FakeItEasy.Core.Creation;
    using IInterceptor = Castle.Core.Interceptor.IInterceptor;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;

    /// <summary>
    /// An implementation of the IProxyGenerator interface that uses DynamicProxy2 to
    /// generate proxies.
    /// </summary>
    internal class CastleDynamicProxyGenerator
        : DynamicProxyGeneratorBase
    {
        private static ProxyGenerator proxyGenerator = new ProxyGenerator();
        private static HashSet<RuntimeMethodHandle> objectMethods = new HashSet<RuntimeMethodHandle>() 
        {
            typeof(object).GetMethod("ToString", new Type[] {}).MethodHandle,
            typeof(object).GetMethod("GetHashCode", new Type[] {}).MethodHandle,
            typeof(object).GetMethod("Equals", new[] { typeof(object) }).MethodHandle
        };

        public CastleDynamicProxyGenerator(IFakeCreationSession session)
            : base(session)
        {
        }

        protected override IFakedProxy GenerateInterfaceProxy(Type typeToProxy, IEnumerable<Type> additionalInterfacesToImplement, FakeManager fakeManager, IInterceptionCallback interceptionCallback)
        {
            var interceptor = new FakeObjectInterceptor() { FakeManager = fakeManager, InterceptionCallback = interceptionCallback };
            var proxy = (IFakedProxy)proxyGenerator.CreateInterfaceProxyWithoutTarget(typeToProxy, additionalInterfacesToImplement.ToArray(), interceptor);
            return proxy;
        }


        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Appropriate in try-methods.")]
        protected override bool TryGenerateClassProxy(Type typeToProxy, IEnumerable<Type> additionalInterfacesToImplement, FakeManager fakeManager, IEnumerable<object> argumentsForConstructor, IInterceptionCallback interceptionCallback, out IFakedProxy proxy)
        {
            var interceptor = new FakeObjectInterceptor { FakeManager = fakeManager, InterceptionCallback = interceptionCallback };

            try
            {
                proxy = (IFakedProxy)proxyGenerator.CreateClassProxy(
                    typeToProxy,
                    additionalInterfacesToImplement.ToArray(),
                    ProxyGenerationOptions.Default,
                    argumentsForConstructor.ToArray(),
                    interceptor);
            }
            catch (Exception)
            {
                proxy = null;
                return false;
            }

            return true;
        }

        public override bool MemberCanBeIntercepted(MemberInfo member)
        {
            if (IsNonInterceptableObjectMethod(member))
            {
                return false;
            }

            return base.MemberCanBeIntercepted(member);
        }

        private static bool IsNonInterceptableObjectMethod(MemberInfo member)
        {
            var method = member as MethodInfo;
            return method != null && objectMethods.Contains(method.MethodHandle);
        }

        [Serializable]
        private class FakeObjectInterceptor
            : IInterceptor
        {
            private static readonly MethodInfo getFakeManagerMethod = typeof(IFakedProxy).GetProperty("FakeManager").GetGetMethod();

            public FakeManager FakeManager { get; set; }

            public IInterceptionCallback InterceptionCallback { get; set; }

            public void Intercept(IInvocation invocation)
            {
                if (invocation.Method.Equals(getFakeManagerMethod))
                {
                    invocation.ReturnValue = this.FakeManager;
                }
                else
                {
                    this.InterceptionCallback.Invoke(new CastleInvocationCallAdapter(invocation));
                }
            }
        }
    }

    public class CastleDynamicProxyGeneratorNew
        : IProxyGenerator2
    {
        private static ProxyGenerator proxyGenerator = new ProxyGenerator();
        private static HashSet<RuntimeMethodHandle> objectMethods = new HashSet<RuntimeMethodHandle>() 
        {
            typeof(object).GetMethod("ToString", new Type[] {}).MethodHandle,
            typeof(object).GetMethod("GetHashCode", new Type[] {}).MethodHandle,
            typeof(object).GetMethod("Equals", new[] { typeof(object) }).MethodHandle
        };

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
                IsNonInterceptableObjectMethod(member) || 
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

        private static bool IsNonInterceptableObjectMethod(MemberInfo member)
        {
            var method = member as MethodInfo;
            return method != null && objectMethods.Contains(method.MethodHandle);
        }

        private static ProxyGeneratorResult GetProxyResultForNoDefaultConstructor(Type typeOfProxy)
        {
            return new ProxyGeneratorResult(string.Format(CultureInfo.CurrentCulture, DynamicProxyResources.ProxyTypeWithNoDefaultConstructorMessage, typeOfProxy));
        }

        private static ProxyGeneratorResult GetProxyResultForValueType(Type typeOfProxy)
        {
            return new ProxyGeneratorResult(string.Format(CultureInfo.CurrentCulture, DynamicProxyResources.ProxyIsValueTypeMessage, typeOfProxy));
        }

        private IProxy DoGenerateProxy(Type typeOfProxy, IEnumerable<Type> additionalInterfacesToImplement, IEnumerable<object> argumentsForConstructor, IInterceptor interceptor)
        {
            var allInterfacesToImplement = GetAllInterfacesToImplement(additionalInterfacesToImplement);

            if (typeOfProxy.IsInterface)
            {
                return (IProxy)proxyGenerator.CreateInterfaceProxyWithoutTarget(
                    typeOfProxy, 
                    allInterfacesToImplement,
                    interceptor);
            }
            else
            {
                return GenerateClassProxy(
                    typeOfProxy, 
                    argumentsForConstructor, 
                    interceptor, 
                    allInterfacesToImplement);
            }
        }

        private static IProxy GenerateClassProxy(Type typeOfProxy, IEnumerable<object> argumentsForConstructor, IInterceptor interceptor, Type[] allInterfacesToImplement)
        {
            var argumentsArray = GetConstructorArgumentsArray(argumentsForConstructor);

            return (IProxy)proxyGenerator.CreateClassProxy(
                typeOfProxy,
                allInterfacesToImplement,
                ProxyGenerationOptions.Default,
                argumentsArray,
                interceptor);
        }

        private static object[] GetConstructorArgumentsArray(IEnumerable<object> argumentsForConstructor)
        {
            return argumentsForConstructor != null ? argumentsForConstructor.ToArray() : null;
        }

        private static Type[] GetAllInterfacesToImplement(IEnumerable<Type> additionalInterfacesToImplement)
        {
            return additionalInterfacesToImplement.Concat(new[] { typeof(IProxy) }).ToArray();
        }

        private class ProxyInterceptor 
            : IInterceptor, ICallInterceptedEventRaiser
        {
            private static readonly MethodInfo tagGetMethod = typeof(IProxy).GetProperty("Tag").GetGetMethod();
            private static readonly MethodInfo tagSetMethod = typeof(IProxy).GetProperty("Tag").GetSetMethod();

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