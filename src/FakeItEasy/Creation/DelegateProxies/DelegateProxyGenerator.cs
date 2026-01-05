namespace FakeItEasy.Creation.DelegateProxies;

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

        var options = new ProxyGenerationOptions();
        options.AddDelegateTypeMixin(typeOfProxy);

        var delegateProxyInterceptor = new DelegateProxyInterceptor(fakeCallProcessorProvider);
        var proxy = ProxyGenerator.CreateClassProxy<object>(options, delegateProxyInterceptor);

        var delegateProxy = ProxyUtil.CreateDelegateToMixin(proxy, typeOfProxy);
        delegateProxyInterceptor.SetDelegate(delegateProxy);

        fakeCallProcessorProvider.EnsureInitialized(delegateProxy);

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

    private class DelegateProxyInterceptor(IFakeCallProcessorProvider fakeCallProcessorProvider) : IInterceptor
    {
        private Delegate? theDelegate;

        public void SetDelegate(Delegate newDelegate)
        {
            this.theDelegate = newDelegate;
        }

        public void Intercept(IInvocation invocation)
        {
            Guard.AgainstNull(invocation);
            var call = new CastleInvocationDelegateCallAdapter(invocation, this.theDelegate!);
            fakeCallProcessorProvider.Fetch(invocation.Proxy).Process(call);
        }
    }
}
