namespace FakeItEasy.Core.Creation
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    
    /// <summary>
    /// Represents a fake creation session.
    /// </summary>
    public interface IFakeCreationSession
    {
        /// <summary>
        /// Tries to get a cached dummy value.
        /// </summary>
        /// <param name="type">The type of dummy value to get.</param>
        /// <param name="value">An output parameter for the cached dummy value.</param>
        /// <returns>True if a cached value exists.</returns>
        [SuppressMessage("Microsoft.Design", "CA1007:UseGenericsWhereAppropriate", Justification = "Used by the framework, generics would provide no benefit.")]
        bool TryGetCachedDummyValue(Type type, out object value);

        /// <summary>
        /// Registers that an attempt to resolve the specified type has happened in the session.
        /// </summary>
        /// <param name="type">The type of value that has been attempted.</param>
        void RegisterTriedToResolveType(Type type);

        /// <summary>
        /// Gets a value indicating if a failed attempt to resolve the specified type has
        /// happened in the session.
        /// </summary>
        /// <param name="type">The type in question.</param>
        /// <returns>True if a failed attempt has happened in the session.</returns>
        bool TypeHasFailedToResolve(Type type);

        /// <summary>
        /// Adds a resolved dummy value to the dummy value cache.
        /// </summary>
        /// <param name="type">The type of the dummy value.</param>
        /// <param name="dummy">The dummy value to put in the cache.</param>
        void AddResolvedDummyValueToCache(Type type, object dummy);

        /// <summary>
        /// Gets the dummy creator that participates in the session.
        /// </summary>
        IDummyValueCreator DummyCreator { get; }

        /// <summary>
        /// Gets the constructor resolver that participates in the session.
        /// </summary>
        IConstructorResolver ConstructorResolver { get; }

        /// <summary>
        /// Gets the proxy generator that participates in the session.
        /// </summary>
        IProxyGenerator ProxyGenerator { get; }
    }
}
