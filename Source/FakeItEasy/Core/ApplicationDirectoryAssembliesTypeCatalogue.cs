namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Access all types in all assemblies in the same directory as the FakeItEasy dll.
    /// </summary>
    public class ApplicationDirectoryAssembliesTypeCatalogue
        : ITypeCatalogue
    {
        private List<Type> availableTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationDirectoryAssembliesTypeCatalogue"/> class.
        /// </summary>
        public ApplicationDirectoryAssembliesTypeCatalogue()
        {
            this.availableTypes = new List<Type>();

            this.InitializeAvailableTypes();
        }

        /// <summary>
        /// Gets a collection of available types.
        /// </summary>
        /// <returns>The available types.</returns>
        public IEnumerable<Type> GetAvailableTypes()
        {
            return this.availableTypes;
        }

        private void InitializeAvailableTypes()
        {
            foreach (var assembly in GetAllAvailableAssemblies())
            {
                this.LoadAllTypesFromAssembly(assembly);
            }
        }

        private static IEnumerable<Assembly> GetAllAvailableAssemblies()
        {
            return GetAllAssembliesInApplicationDirectory().Concat(GetAllAsembliesInAppDomain()).Distinct();
        }


        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Appropriate in try methods.")]
        private static IEnumerable<Assembly> GetAllAssembliesInApplicationDirectory()
        {
            var applicationDirectory = Environment.CurrentDirectory;
            var result = new LinkedList<Assembly>();

            foreach (var assemblyFile in Directory.GetFiles(applicationDirectory, "*.dll"))
            {
                try
                {
                    result.AddLast(Assembly.LoadFile(assemblyFile));
                }
                catch { }
            }

            return result;
        }

        private static IEnumerable<Assembly> GetAllAsembliesInAppDomain()
        {
            return AppDomain.CurrentDomain.GetAssemblies();
        }

        private void LoadAllTypesFromAssembly(Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
            {
                this.availableTypes.Add(type);
            }
        }
    }
}
