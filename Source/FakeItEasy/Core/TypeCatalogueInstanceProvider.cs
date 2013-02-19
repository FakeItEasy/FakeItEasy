﻿namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    /// <summary>
    /// Providesinstances from type catalogues.
    /// </summary>
    internal class TypeCatalogueInstanceProvider
    {
        private static readonly Logger Logger = Log.GetLogger<TypeCatalogueInstanceProvider>();
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
            Logger.Debug("Getting instances of type {0}.", typeof(T));

            var result = new List<T>();

            foreach (var type in this.catalogue.GetAvailableTypes().Where(x => typeof(T).IsAssignableFrom(x)))
            {
                try
                {
                    result.Add((T)Activator.CreateInstance(type));
                }
                catch
                {
                    Logger.Debug("Failed to create instance of type {0}.", type);
                }
            }

            return result;
        }
    }
}