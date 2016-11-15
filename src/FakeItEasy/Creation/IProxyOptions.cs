namespace FakeItEasy.Creation
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    internal interface IProxyOptions
    {
        IEnumerable<object> ArgumentsForConstructor { get; }

        IEnumerable<Type> AdditionalInterfacesToImplement { get; }

        IEnumerable<Action<object>> ProxyConfigurationActions { get; }

        IEnumerable<Expression<Func<Attribute>>> Attributes { get; }
    }
}
