namespace FakeItEasy
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The default bootstrapper, used to initialize FakeItEasy unless another 
    /// implementation of <see cref="IBootstrapper"/> is present in the assemblies
    /// loaded in the app domain.
    /// </summary>
    public class DefaultBootstrapper : IBootstrapper
    {
        /// <summary>
        /// When overridden in a derived class, provides a custom list of assembly file
        /// names to scan for extension points, such as
        /// <see cref="IDummyFactory"/>s, <see cref="IArgumentValueFormatter"/>s, and 
        /// <see cref="IFakeConfigurator"/>s.
        /// </summary>
        /// <returns>
        /// An empty list, but may be overridden to provide a list of absolute paths
        /// to assemblies to scan for extension points.
        /// </returns>
        public virtual IEnumerable<string> GetAssemblyFileNamesToScanForExtensions()
        {
            return Enumerable.Empty<string>();
        }
    }
}
