namespace FakeItEasy.Creation
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;
#if FEATURE_NETCORE_REFLECTION
    using System.Reflection;
#endif

    internal class ProxyOptions : IProxyOptions
    {
        private readonly List<Type> additionalInterfacesToImplement = new List<Type>();
        private readonly List<Action<object>> proxyConfigurationActions = new List<Action<object>>();
        private readonly List<Expression<Func<Attribute>>> attributes = new List<Expression<Func<Attribute>>>();

        public static IProxyOptions Default { get; } = new DefaultProxyOptions();

        public IEnumerable<object?>? ArgumentsForConstructor { get; set; }

        public ReadOnlyCollection<Type> AdditionalInterfacesToImplement => this.additionalInterfacesToImplement.AsReadOnly();

        public IEnumerable<Action<object>> ProxyConfigurationActions => this.proxyConfigurationActions;

        public IEnumerable<Expression<Func<Attribute>>> Attributes => this.attributes;

        public string? Name { get; set; }

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

        private class DefaultProxyOptions : IProxyOptions
        {
            public IEnumerable<object?>? ArgumentsForConstructor { get; } = null;

            public ReadOnlyCollection<Type> AdditionalInterfacesToImplement { get; } = new ReadOnlyCollection<Type>(new List<Type>());

            public IEnumerable<Action<object>> ProxyConfigurationActions { get; } = Enumerable.Empty<Action<object>>();

            public IEnumerable<Expression<Func<Attribute>>> Attributes { get; } = Enumerable.Empty<Expression<Func<Attribute>>>();

            public string? Name => null;
        }
    }
}
