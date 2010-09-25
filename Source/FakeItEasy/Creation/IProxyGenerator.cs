namespace FakeItEasy.Creation
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public interface IProxyGenerator
    {
        ProxyGeneratorResult GenerateProxy(Type typeOfProxy, IEnumerable<Type> additionalInterfacesToImplement, IEnumerable<object> argumentsForConstructor);
        bool MemberCanBeIntercepted(MemberInfo member);
    }
}