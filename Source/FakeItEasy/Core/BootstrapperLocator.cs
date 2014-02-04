namespace FakeItEasy.Core
{
    using System;
    using System.Linq;

    /// <summary>
    /// Locate an <see cref="IBootstrapper"/> implementation.
    /// </summary>
    /// <remarks>
    /// Will search the app domain for a concrete implementation,
    /// and if it can't find one will use the <see cref="DefaultBootstrapper"/>.
    /// </remarks>
    internal static class BootstrapperLocator
    {
        /// <summary>
        /// Scans app domain assemblies looking for a concrete implementation of
        /// <see cref="IBootstrapper"/> that is not the <see cref="DefaultBootstrapper"/>.
        /// If none is found, the latter is used. Otherwise, the first matching type
        /// is instantiated and returned.
        /// </summary>
        /// <returns>An instance of the first non-default IBootstrapper implementation found,
        /// or a DefaultBootstrapper.</returns>
        public static IBootstrapper FindBootstrapper()
        {
            var bootstrapperInterface = typeof(IBootstrapper);
            var defaultBootstrapperType = typeof(DefaultBootstrapper);

            var appDomainAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            var appDomainAssembliesReferencingFakeItEasy = appDomainAssemblies.Where(assembly => assembly.ReferencesFakeItEasy());

            var candidateTypes = appDomainAssembliesReferencingFakeItEasy
                .SelectMany(assembly => assembly.GetExportedTypes())
                .Where(type => bootstrapperInterface.IsAssignableFrom(type) && type != defaultBootstrapperType);

            var bootstrapperType = candidateTypes.FirstOrDefault() ?? defaultBootstrapperType;

            return (IBootstrapper)Activator.CreateInstance(bootstrapperType);
        }
    }
}
