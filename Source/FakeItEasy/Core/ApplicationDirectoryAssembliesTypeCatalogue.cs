namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Castle.Core.Internal;

    /// <summary>
    /// Access all types in all assemblies in the same directory as the FakeItEasy assembly.
    /// </summary>
    public class ApplicationDirectoryAssembliesTypeCatalogue : ITypeCatalogue
    {
        private static readonly Assembly FakeItEasyAssembly = Assembly.GetExecutingAssembly();
        private readonly List<Type> availableTypes = new List<Type>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationDirectoryAssembliesTypeCatalogue"/> class.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Defensive and performed on best effort basis.")]
        public ApplicationDirectoryAssembliesTypeCatalogue()
        {
            foreach (var assembly in GetAllAvailableAssemblies())
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
            Assembly[] appDomainAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            IEnumerable<Assembly> appDomainAssembliesReferencingFakeItEasy = appDomainAssemblies.Where(ReferencesFakeItEasy);

            // Find the paths of already loaded assemblies so we don't double scan them.
            // Checking Assembly.IsDynamic would be preferable to the business with the Namespace, but the former isn't available in .NET 3.5.
            // Exclude the ReflectionOnly assemblies because we want to be able to fully load them if we need to.
            var loadedAssemblyPaths = new HashSet<string>(
                appDomainAssemblies
                    .Where(a => !a.ReflectionOnly && a.ManifestModule.GetType().Namespace != "System.Reflection.Emit")
                    .Select(a => a.Location),
                StringComparer.OrdinalIgnoreCase);

            var folderAssembliesReferencingFakeItEasy = new List<Assembly>();

            IEnumerable<string> filesToScan;
            if (appDomainAssemblies.Any(a => a.HasAttribute<DisableFakeItEasyExtensionDirectoryScanAttribute>()))
            {
                filesToScan = Enumerable.Empty<string>();
            }
            else
            {
                filesToScan = Directory.GetFiles(Environment.CurrentDirectory, "*.dll");
            }

            // Skip assemblies already in the application domain.
            // This is an optimization that can be fooled by test runners that make shadow copies of the assemblies, but it's a start.
            foreach (var assemblyFile in filesToScan.Except(loadedAssemblyPaths))
            {
                Assembly assembly = null;
                try
                {
                    assembly = Assembly.ReflectionOnlyLoadFrom(assemblyFile);
                }
                catch (BadImageFormatException)
                {
                    // The assembly may not be managed, so move on.
                    continue;
                }
                catch (Exception e)
                {
                    WarnFailedToLoadAssembly(assemblyFile, e);
                    continue;
                }

                if (!ReferencesFakeItEasy(assembly))
                {
                    continue;
                }

                try
                {
                    // A reflection-only loaded assembly can't be scanned for types, so fully load it before saving it.
                    folderAssembliesReferencingFakeItEasy.Add(Assembly.Load(assembly.GetName()));
                }
                catch
                {
                }
            }

            return folderAssembliesReferencingFakeItEasy
                .Concat(appDomainAssembliesReferencingFakeItEasy)
                .Concat(new[] { FakeItEasyAssembly })
                .Distinct();
        }

        private static bool ReferencesFakeItEasy(Assembly inspectedAssembly)
        {
            return inspectedAssembly.GetReferencedAssemblies().Any(r => r.FullName == FakeItEasyAssembly.FullName);
        }

        private static void WarnFailedToLoadAssembly(string assemblyFile, Exception e)
        {
            var outputWriter = new DefaultOutputWriter(Console.Write);
            outputWriter.Write(
                "Warning: FakeItEasy failed to load assembly '{0}' while scanning for extension points. Any IArgumentValueFormatters, IDummyDefinitions, and IFakeConfigurators in that assembly will not be available.",
                assemblyFile);
            outputWriter.WriteLine();
            using (outputWriter.Indent())
            {
                outputWriter.Write(e.Message);
                outputWriter.WriteLine();
            }
        }
    }
}
