namespace FakeItEasy.Creation
{
    using System;
    using System.Collections.Generic;
    using System.Reflection.Emit;

    internal interface IProxyOptions
    {
        IEnumerable<object> ArgumentsForConstructor { get; }

        IEnumerable<Type> AdditionalInterfacesToImplement { get; }

        IEnumerable<Action<object>> ProxyConfigurationActions { get; }

        IEnumerable<CustomAttributeBuilder> AdditionalAttributes { get; }
    }
}