namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Provides a set of types that are available.
    /// </summary>
    public interface ITypeAccessor
    {
        /// <summary>
        /// Gets a collection of available types.
        /// </summary>
        /// <returns>The available types.</returns>
        IEnumerable<Type> GetAvailableTypes();
    }
}