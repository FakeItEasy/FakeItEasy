namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Access all types in all assemblies in the same directory as the FakeItEasy assembly.
    /// </summary>
    public class ApplicationDirectoryAssembliesTypeCatalogue
        : ITypeCatalogue
    {
        private static readonly Assembly FakeItEasyAssembly = Assembly.GetExecutingAssembly();
        private readonly List<Type> availableTypes;

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

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Appropriate in try methods.")]
        private static IEnumerable<Assembly> GetAllAvailableAssemblies()
        {
            var appDomainAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            var assembliesInAppDomainWithFakeItEasyReferences = appDomainAssemblies.Where(ReferencesFakeItEasy);

            // Find the paths of already loaded assemblies so we don't double scan them. Checking Assembly.IsDynamic would be preferable
            // to the business with the Namespace, but the former isn't available in .NET 3.5. Exclude the ReflectionOnly assemblies because
            // we want to be able to fully load them if we need to.
            var loadedAssemblyPaths = new HashSet<string>(
                appDomainAssemblies
                    .Where(a => !a.ReflectionOnly && a.ManifestModule.GetType().Namespace != "System.Reflection.Emit")
                    .Select(a => a.Location),
                StringComparer.OrdinalIgnoreCase);

            var assembliesFromDirectoryFilesWithFakeItEasyReferences = new List<Assembly>();
            var applicationDirectory = Environment.CurrentDirectory;

            foreach (var assemblyFile in Directory.GetFiles(applicationDirectory, "*.dll"))
            {
                // Skip assemblies already in the application domain. This is an optimization that can be fooled by test runners that
                // make shadow copies of the assemblies, but it's a start.
                if (loadedAssemblyPaths.Contains(assemblyFile))
                {
                    continue;
                }

                Assembly inspectedAssembly = null;
                try
                {
                    inspectedAssembly = Assembly.ReflectionOnlyLoadFrom(assemblyFile);
                }
                catch (BadImageFormatException)
                {
                    // The assembly may not be managed, so move on.
                }

                if (inspectedAssembly != null && ReferencesFakeItEasy(inspectedAssembly))
                {
                    try
                    {
                        // A reflection-only loaded assembly can't be scanned for types, so fully load it before saving it.
                        assembliesFromDirectoryFilesWithFakeItEasyReferences.Add(Assembly.Load(inspectedAssembly.GetName()));
                    }
                    catch
                    {
                    }
                }
            }

            return assembliesFromDirectoryFilesWithFakeItEasyReferences
                .Concat(assembliesInAppDomainWithFakeItEasyReferences)
                .Concat(new[] { FakeItEasyAssembly })
                .Distinct();
        }

        private static bool ReferencesFakeItEasy(Assembly inspectedAssembly)
        {
            return inspectedAssembly.GetReferencedAssemblies().Any(r => r.FullName == FakeItEasyAssembly.FullName);
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Appropriate in try methods.")]
        private void LoadAllTypesFromAssembly(Assembly assembly)
        {
            try
            {
                foreach (var type in assembly.GetTypes())
                {
                    this.availableTypes.Add(type);
                }
            }
            catch
            {
            }
        }

        private void InitializeAvailableTypes()
        {
            foreach (var assembly in GetAllAvailableAssemblies())
            {
                this.LoadAllTypesFromAssembly(assembly);
            }
        }
    }
}