namespace FakeItEasy.Creation
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reflection.Emit;

    internal interface IProxyOptions
    {
        IEnumerable<object> ArgumentsForConstructor { get; }

        IEnumerable<Type> AdditionalInterfacesToImplement { get; }

        IEnumerable<Action<object>> ProxyConfigurationActions { get; }

        IEnumerable<CustomAttributeBuilder> AdditionalAttributes { get; }
    }

    internal static class ProxyOptions
    {
        public static readonly IProxyOptions Empty = new EmptyProxyOptions();

        private class EmptyProxyOptions : IProxyOptions
        {
            private readonly IEnumerable<Type> additionalInterfacesToImplement = new Type[0];
            private readonly IEnumerable<Action<object>> proxyConfigurationActions = new Action<object>[0];
            private readonly IEnumerable<CustomAttributeBuilder> customAttributeBuilders = new CustomAttributeBuilder[0];

            public IEnumerable<object> ArgumentsForConstructor
            {
                get { return null; }
            }

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
                get { return this.customAttributeBuilders; }
            }
        }
    }
}