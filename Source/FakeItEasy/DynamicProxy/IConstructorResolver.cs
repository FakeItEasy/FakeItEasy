namespace FakeItEasy.DynamicProxy
{
    using System;
    using System.Collections.Generic;

    internal interface IConstructorResolver
    {
        IEnumerable<ConstructorAndArgumentsInfo> ListAllConstructors(Type type);
    }
}
