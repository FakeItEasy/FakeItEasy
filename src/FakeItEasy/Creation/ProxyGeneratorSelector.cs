namespace FakeItEasy.Creation
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;
    using FakeItEasy.Core;
    using FakeItEasy.Creation.DelegateProxies;

    internal class ProxyGeneratorSelector
        : IProxyGenerator
    {
        private readonly DelegateProxyGenerator delegateProxyGenerator;
        private readonly IProxyGenerator defaultProxyGenerator;

        public ProxyGeneratorSelector(DelegateProxyGenerator delegateProxyGenerator, IProxyGenerator defaultProxyGenerator)
        {
            this.delegateProxyGenerator = delegateProxyGenerator;
            this.defaultProxyGenerator = defaultProxyGenerator;
        }

        public ProxyGeneratorResult GenerateProxy(Type typeOfProxy, IEnumerable<Type> additionalInterfacesToImplement, IEnumerable<object> argumentsForConstructor, IFakeCallProcessorProvider fakeCallProcessorProvider)
        {
            var generator = this.SelectProxyGenerator(typeOfProxy);

            return generator.GenerateProxy(typeOfProxy, additionalInterfacesToImplement, argumentsForConstructor, fakeCallProcessorProvider);
        }

        public ProxyGeneratorResult GenerateProxy(Type typeOfProxy, IEnumerable<Type> additionalInterfacesToImplement, IEnumerable<object> argumentsForConstructor, IEnumerable<Expression<Func<Attribute>>> attributes, IFakeCallProcessorProvider fakeCallProcessorProvider)
        {
            var generator = this.SelectProxyGenerator(typeOfProxy);

            return generator.GenerateProxy(typeOfProxy, additionalInterfacesToImplement, argumentsForConstructor, attributes, fakeCallProcessorProvider);
        }

        public bool MethodCanBeInterceptedOnInstance(MethodInfo method, object callTarget, out string failReason)
        {
            var generator = this.SelectProxyGenerator(callTarget == null ? null : callTarget.GetType());

            return generator.MethodCanBeInterceptedOnInstance(method, callTarget, out failReason);
        }

        private static bool IsDelegateType(Type typeOfProxy)
        {
            return typeOfProxy != null && typeOfProxy.GetTypeInfo().IsSubclassOf(typeof(Delegate));
        }

        private IProxyGenerator SelectProxyGenerator(Type typeOfProxy)
        {
            return IsDelegateType(typeOfProxy) ? this.delegateProxyGenerator : this.defaultProxyGenerator;
        }
    }
}
