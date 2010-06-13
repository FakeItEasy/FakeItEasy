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

    /// <summary>
    /// An implementation of the IProxyGenerator interface that uses DynamicProxy2 to
    /// generate proxies.
    /// </summary>
    internal class CastleDynamicProxyGenerator
        : DynamicProxyGeneratorBase
    {
        private static ProxyGenerator proxyGenerator = new ProxyGenerator();

        public CastleDynamicProxyGenerator(IFakeCreationSession session)
            : base(session)
        {
        }

        protected override IEnumerable<Type> InterfacesThatAllProxiesShouldImplement
        {
            get
            {
                return base.InterfacesThatAllProxiesShouldImplement.Concat(new[] { typeof(ICanInterceptObjectMembers) });
            }
        }

        protected override IFakedProxy GenerateInterfaceProxy(Type typeToProxy, IEnumerable<Type> additionalInterfacesToImplement, FakeManager fakeManager, IInterceptionCallback interceptionCallback)
        {
            var interceptor = new FakeObjectInterceptor() { FakeManager = fakeManager, InterceptionCallback = interceptionCallback };
            var proxy = (IFakedProxy)proxyGenerator.CreateInterfaceProxyWithoutTarget(typeToProxy, additionalInterfacesToImplement.ToArray(), interceptor);
            return proxy;
        }


        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification="Appropriate in try-methods.")]
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
}