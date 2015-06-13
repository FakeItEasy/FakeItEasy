namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    /// <summary>
    /// Provides instances from type catalogues.
    /// </summary>
    internal class TypeCatalogueInstanceProvider
    {
        private readonly ITypeCatalogue catalogue;

        public TypeCatalogueInstanceProvider(ITypeCatalogue catalogue)
        {
            this.catalogue = catalogue;
        }

        /// <summary>
        /// Gets an instance per type in the catalogue that is a descendant
        /// of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of instances to get.</typeparam>
        /// <returns>A sequence of instances of the specified type.</returns>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Necessary in this case.")]
        public IEnumerable<T> InstantiateAllOfType<T>()
        {
            var result = new List<T>();

            foreach (var type in this.catalogue.GetAvailableTypes().Where(x => x.CanBeInstantiatedAs(typeof(T))))
            {
                try
                {
                    result.Add((T)Activator.CreateInstance(type));
                }
                catch
                {
                }
            }

            return result;
        }
    }
}