namespace FakeItEasy.Creation.DelegateProxies
{
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Castle.DynamicProxy;
    using FakeItEasy.Core;

    internal static class DelegateProxyGenerator
    {
        private static readonly ProxyGenerator ProxyGenerator = new ProxyGenerator();
        private static readonly ConcurrentDictionary<Type, bool> AccessibleToDynamicProxyCache = new ConcurrentDictionary<Type, bool>();

        [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Appropriate in Try-style methods")]
        public static ProxyGeneratorResult GenerateProxy(
            Type typeOfProxy,
            IFakeCallProcessorProvider fakeCallProcessorProvider)
        {
            Guard.AgainstNull(typeOfProxy);

            if (!IsAccessibleToDynamicProxy(typeOfProxy))
            {
                try
                {
                    // This is the only way to get the proper error message.
                    // The need for this will go away when we start really using DynamicProxy to generate delegate proxies.
                    ProxyGenerator.CreateClassProxy(typeOfProxy);
                }
                catch (Exception ex)
                {
                    return new ProxyGeneratorResult(ex.Message);
                }
            }

            // You'll need distinct `options` per delegate type. Specify additional options as needed:
            var options = new ProxyGenerationOptions();
            options.AddDelegateTypeMixin(typeOfProxy);

            // Then use it to generate a proxy. That proxy will have an `Invoke` method with the specified
            // delegate type's signature. Proxy type, base class, interceptors etc. are chosen as usual:
            var proxy = ProxyGenerator.CreateClassProxy(typeof(object), options, new ProxyInterceptor(fakeCallProcessorProvider));

            // Either call `Delegate.CreateDelegate` for the generated `Invoke` method yourself, or use a
            // helper method provided by DynamicProxy:
            var delegateProxy = ProxyUtil.CreateDelegateToMixin(proxy, typeOfProxy);

            fakeCallProcessorProvider.EnsureInitialized(proxy);

            var a = ServiceLocator.Resolve<IFakeManagerAccessor>();
            a.SetFakeManager(delegateProxy, a.GetFakeManager(proxy));
            return new ProxyGeneratorResult(delegateProxy);
        }

        private static bool IsAccessibleToDynamicProxy(Type type)
        {
            return AccessibleToDynamicProxyCache.GetOrAdd(type, IsAccessibleImpl);

            bool IsAccessibleImpl(Type t)
            {
                if (!ProxyUtil.IsAccessible(t))
                {
                    return false;
                }

                if (type.IsGenericType && !type.IsGenericTypeDefinition)
                {
                    return t.GetGenericArguments().All(IsAccessibleToDynamicProxy);
                }

                return true;
            }
        }
    }
}
