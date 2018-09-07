namespace FakeItEasy.Creation.DelegateProxies
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Castle.DynamicProxy;
    using FakeItEasy.Configuration;
    using FakeItEasy.Core;

    internal static class DelegateProxyGenerator
    {
        private static readonly ProxyGenerator ProxyGenerator = new ProxyGenerator();

        public static ProxyGeneratorResult GenerateProxy(
            Type typeOfProxy,
            IFakeCallProcessorProvider fakeCallProcessorProvider)
        {
            Guard.AgainstNull(typeOfProxy, nameof(typeOfProxy));

            if (!CanGenerateProxy(typeOfProxy, out string reasonCannotGenerate))
            {
                return new ProxyGeneratorResult(reasonCannotGenerate);
            }

            var invokeMethod = typeOfProxy.GetMethod("Invoke");
            var interceptor = new DelegateCallInterceptor(fakeCallProcessorProvider, invokeMethod, typeOfProxy);

            fakeCallProcessorProvider.EnsureInitialized(interceptor.Instance);
            return new ProxyGeneratorResult(interceptor.Instance);
        }

        public static bool CanGenerateProxy(Type typeOfProxy, out string failReason)
        {
            if (!typeof(Delegate).IsAssignableFrom(typeOfProxy))
            {
                failReason = "The delegate proxy generator can only create proxies for delegate types.";
                return false;
            }

            failReason = null;
            return true;
        }

        private class DelegateCallInterceptor
            : IInterceptor
        {
            private readonly IFakeCallProcessorProvider fakeCallProcessorProvider;
            private readonly MethodInfo method;

            public DelegateCallInterceptor(IFakeCallProcessorProvider fakeCallProcessorProvider, MethodInfo method, Type type)
            {
                this.fakeCallProcessorProvider = fakeCallProcessorProvider;
                this.method = method;
                this.Instance = ProxyGenerator.CreateDelegateProxy(type, this);
            }

            public Delegate Instance { get; }

            public void Intercept(IInvocation invocation)
            {
                var call = new DelegateFakeObjectCall(this.Instance, this.method, invocation.Arguments);
                this.fakeCallProcessorProvider.Fetch(this.Instance).Process(call);
                invocation.ReturnValue = call.ReturnValue;
            }
        }

        private class DelegateFakeObjectCall : IInterceptedFakeObjectCall
        {
            private readonly object[] originalArguments;

            public DelegateFakeObjectCall(Delegate instance, MethodInfo method, object[] arguments)
            {
                this.FakedObject = instance;
                this.originalArguments = arguments.ToArray();
                this.Arguments = new ArgumentCollection(arguments, method);
                this.Method = method;
            }

            public object ReturnValue { get; private set; }

            public MethodInfo Method { get; }

            public ArgumentCollection Arguments { get; }

            public object FakedObject { get; }

            public void SetReturnValue(object value)
            {
                this.ReturnValue = value;
            }

            public void CallBaseMethod()
            {
                throw new NotSupportedException("Can not configure a delegate proxy to call base method.");
            }

            public void SetArgumentValue(int index, object value)
            {
                this.Arguments.GetUnderlyingArgumentsArray()[index] = value;
            }

            public ICompletedFakeObjectCall AsReadOnly()
            {
                return new CompletedFakeObjectCall(
                    this,
                    this.originalArguments,
                    this.ReturnValue);
            }
        }
    }
}
