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
        /// Otherwise, the first matching type is instantiated and returned, or an
        /// instance of DefaultBootstrapper is used if no other implementation is found.
        /// </summary>
        /// <returns>An instance of the first non-default IBootstrapper implementation found,
        /// or a DefaultBootstrapper.</returns>
        public static IBootstrapper FindBootstrapper()
        {
            var bootstrapperInterface = typeof(IBootstrapper);

            var appDomainAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            var appDomainAssembliesReferencingFakeItEasy = appDomainAssemblies.Where(assembly => assembly.ReferencesFakeItEasy());

            var candidateTypes = appDomainAssembliesReferencingFakeItEasy
                .SelectMany(assembly => assembly.GetExportedTypes())
                .Where(type => type.CanBeInstantiatedAs(bootstrapperInterface));

            var bootstrapperType = candidateTypes.FirstOrDefault() ?? typeof(DefaultBootstrapper);

            return (IBootstrapper)Activator.CreateInstance(bootstrapperType);
        }
    }
}
