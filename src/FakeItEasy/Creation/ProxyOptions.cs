namespace FakeItEasy.Creation
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
#if FEATURE_NETCORE_REFLECTION
    using System.Reflection;
#endif
    using System.Reflection.Emit;

    internal class ProxyOptions : IProxyOptions
    {
        private readonly List<Type> additionalInterfacesToImplement = new List<Type>();
        private readonly List<Action<object>> proxyConfigurationActions = new List<Action<object>>();
        private readonly List<CustomAttributeBuilder> additionalAttributes = new List<CustomAttributeBuilder>();

        public IEnumerable<object> ArgumentsForConstructor { get; set; }

        public IEnumerable<Type> AdditionalInterfacesToImplement => this.additionalInterfacesToImplement;

        public IEnumerable<Action<object>> ProxyConfigurationActions => this.proxyConfigurationActions;

        public IEnumerable<CustomAttributeBuilder> AdditionalAttributes => this.additionalAttributes;

        public void AddInterfaceToImplement(Type interfaceType)
        {
            Guard.AgainstNull(interfaceType, nameof(interfaceType));

            if (!interfaceType.GetTypeInfo().IsInterface)
            {
                throw new ArgumentException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        "The specified type '{0}' is not an interface",
                        interfaceType.FullNameCSharp()));
            }

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
