﻿namespace FakeItEasy.Creation
{
    using System;
    using System.Collections.Generic;
    using System.Reflection.Emit;
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

        public ProxyGeneratorResult GenerateProxy(Type typeOfProxy, System.Collections.Generic.IEnumerable<Type> additionalInterfacesToImplement, System.Collections.Generic.IEnumerable<object> argumentsForConstructor)
        {
            var generator = this.SelectProxyGenerator(typeOfProxy);

            return generator.GenerateProxy(typeOfProxy, additionalInterfacesToImplement, argumentsForConstructor);
        }

        public ProxyGeneratorResult GenerateProxy(Type typeOfProxy, System.Collections.Generic.IEnumerable<Type> additionalInterfacesToImplement, System.Collections.Generic.IEnumerable<object> argumentsForConstructor, IEnumerable<CustomAttributeBuilder> customAttributeBuilders)
        {
            var generator = this.SelectProxyGenerator(typeOfProxy);

            return generator.GenerateProxy(typeOfProxy, additionalInterfacesToImplement, argumentsForConstructor, customAttributeBuilders);
        }

        public bool MethodCanBeInterceptedOnInstance(System.Reflection.MethodInfo method, object callTarget, out string failReason)
        {
            var generator = this.SelectProxyGenerator(callTarget.GetType());

            return generator.MethodCanBeInterceptedOnInstance(method, callTarget, out failReason);
        }

        private static bool IsDelegateType(Type typeOfProxy)
        {
            return typeOfProxy.IsSubclassOf(typeof(Delegate));
        }

        private IProxyGenerator SelectProxyGenerator(Type typeOfProxy)
        {
            return IsDelegateType(typeOfProxy) ? this.delegateProxyGenerator : this.defaultProxyGenerator;
        }
    }
}