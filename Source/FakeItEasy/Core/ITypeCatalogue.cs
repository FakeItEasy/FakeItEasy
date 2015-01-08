namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Provides a set of types that are available.
    /// </summary>
    internal interface ITypeCatalogue
    {
        /// <summary>
        /// Gets a collection of available types.
        /// </summary>
        /// <returns>The available types.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "A property would not be appropriate here since the operation might perform significant work.")]
        IEnumerable<Type> GetAvailableTypes();
    }
}