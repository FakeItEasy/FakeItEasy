namespace FakeItEasy.Core
{
    using System;
    using System.Linq;

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

            var appDomainAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            var loadedAssembliesReferencingFakeItEasy = appDomainAssemblies
                .Where(assembly => !assembly.IsDynamic)
                .Where(assembly => assembly.ReferencesFakeItEasy());
            var candidateTypes = loadedAssembliesReferencingFakeItEasy
                .SelectMany(assembly => assembly.GetExportedTypes())
                .Where(type => type.CanBeInstantiatedAs(bootstrapperInterface));

            var bootstrapperType = candidateTypes.FirstOrDefault() ?? typeof(DefaultBootstrapper);

            return (IBootstrapper)Activator.CreateInstance(bootstrapperType)!;
        }
    }
}
