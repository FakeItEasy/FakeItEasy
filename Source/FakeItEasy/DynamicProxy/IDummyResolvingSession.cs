namespace FakeItEasy.DynamicProxy
{
    using System;

    internal interface IDummyResolvingSession
    {
        bool TryGetCachedValue(Type type, out object value);

        void RegisterTriedToResolveType(Type type);

        bool TypeHasFailedToResolve(Type type);

        void AddResolvedValueToCache(Type type, object dummy);
    }
}
