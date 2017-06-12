namespace FakeItEasy.Core
{
    using System;
    using System.Linq;
#if !FEATURE_REFLECTION_GETASSEMBLIES
    using System.Reflection;
    using Microsoft.Extensions.DependencyModel;
#endif

    /// <summary>
    /// Locate an <see cref="IBootstrapper"/> implementation.
    /// </summary>
    /// <remarks>
    /// Will search loaded assemblies for a concrete implementation,
    /// and if it can't find one will use the <see cref="DefaultBootstrapper"/>.
    /// </remarks>
    internal static class BootstrapperLocator
    {
        /// <summary>
        /// Scans loaded assemblies looking for a concrete implementation of
        /// <see cref="IBootstrapper"/> that is not the <see cref="DefaultBootstrapper"/>.
        /// The first matching type is instantiated and returned, or an
        /// instance of DefaultBootstrapper is used if no other implementation is found.
        /// </summary>
        /// <returns>An instance of the first non-default IBootstrapper implementation found,
        /// or a DefaultBootstrapper.</returns>
        public static IBootstrapper FindBootstrapper()
        {
            var bootstrapperInterface = typeof(IBootstrapper);

#if FEATURE_REFLECTION_GETASSEMBLIES
            var appDomainAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            var loadedAssembliesReferencingFakeItEasy = appDomainAssemblies
                .Where(assembly => !assembly.IsDynamic)
                .Where(assembly => assembly.ReferencesFakeItEasy());
#else
            var fakeItEasyLibraryName = TypeCatalogue.FakeItEasyAssembly.GetName().Name;

            var context = DependencyContext.Default;
            var candidateLibraries = context.RuntimeLibraries.Where(
                library => library.Dependencies.Any(
                    dependency => string.Equals(dependency.Name, fakeItEasyLibraryName, StringComparison.Ordinal)));

            var loadedAssembliesReferencingFakeItEasy = candidateLibraries
                .SelectMany(library => library.GetDefaultAssemblyNames(context))
                .Select(Assembly.Load)
                .Where(a => !a.IsDynamic);
#endif

            var candidateTypes = loadedAssembliesReferencingFakeItEasy
                .SelectMany(assembly => assembly.GetExportedTypes())
                .Where(type => type.CanBeInstantiatedAs(bootstrapperInterface));

            var bootstrapperType = candidateTypes.FirstOrDefault() ?? typeof(DefaultBootstrapper);

            return (IBootstrapper)Activator.CreateInstance(bootstrapperType);
        }
    }
}
