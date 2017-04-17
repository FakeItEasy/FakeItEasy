namespace FakeItEasy.Creation
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
#if FEATURE_NETCORE_REFLECTION
    using System.Reflection;
#endif

    internal class ProxyOptions : IProxyOptions
    {
        private readonly List<Type> additionalInterfacesToImplement = new List<Type>();
        private readonly List<Action<object>> proxyConfigurationActions = new List<Action<object>>();
        private readonly List<Expression<Func<Attribute>>> attributes = new List<Expression<Func<Attribute>>>();

        public IEnumerable<object> ArgumentsForConstructor { get; set; }

        public IEnumerable<Type> AdditionalInterfacesToImplement => this.additionalInterfacesToImplement;

        public IEnumerable<Action<object>> ProxyConfigurationActions => this.proxyConfigurationActions;

        public IEnumerable<Expression<Func<Attribute>>> Attributes => this.attributes;

        public void AddInterfaceToImplement(Type interfaceType)
        {
            Guard.AgainstNull(interfaceType, nameof(interfaceType));

            if (!interfaceType.GetTypeInfo().IsInterface)
            {
                throw new ArgumentException($"The specified type {interfaceType} is not an interface");
            }

            this.additionalInterfacesToImplement.Add(interfaceType);
        }

        public void AddProxyConfigurationAction(Action<object> action)
        {
            this.proxyConfigurationActions.Add(action);
        }

        public void AddAttribute(Expression<Func<Attribute>> attribute)
        {
            this.attributes.Add(attribute);
        }
    }
}
