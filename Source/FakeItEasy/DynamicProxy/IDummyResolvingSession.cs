namespace FakeItEasy.DynamicProxy
{
    using System;
    using FakeItEasy.Core;
    using FakeItEasy.Core.Creation;

    internal interface IDummyResolvingSession
    {
        bool TryGetCachedValue(Type type, out object value);

        void RegisterTriedToResolveType(Type type);

        bool TypeHasFailedToResolve(Type type);

        void AddResolvedValueToCache(Type type, object dummy);

        IDummyValueCreator DummyCreator { get; }

        IConstructorResolver ConstructorResolver { get; }

        IProxyGenerator ProxyGenerator { get; }
    }
}
