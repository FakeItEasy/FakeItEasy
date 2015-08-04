namespace FakeItEasy.Creation
{
    using System;
    using System.Collections.Generic;
    using System.Reflection.Emit;

    internal class ProxyOptions
    {
        private readonly List<Type> additionalInterfacesToImplement = new List<Type>();
        private readonly List<Action<object>> proxyConfigurationActions = new List<Action<object>>();
        private readonly List<CustomAttributeBuilder> additionalAttributes = new List<CustomAttributeBuilder>();

        public IEnumerable<object> ArgumentsForConstructor { get; set; }

        public IEnumerable<Type> AdditionalInterfacesToImplement
        {
            get { return this.additionalInterfacesToImplement; }
        }

        public IEnumerable<Action<object>> ProxyConfigurationActions
        {
            get { return this.proxyConfigurationActions; }
        }

        public IEnumerable<CustomAttributeBuilder> AdditionalAttributes
        {
            get { return this.additionalAttributes; }
        }

        public void AddInterfaceToImplement(Type interfaceType)
        {
            this.additionalInterfacesToImplement.Add(interfaceType);
        }

        public void AddProxyConfigurationAction(Action<object> action)
        {
            this.proxyConfigurationActions.Add(action);
        }

        public void AddAttribute(CustomAttributeBuilder attribute)
        {
            this.additionalAttributes.Add(attribute);
        }
    }
}