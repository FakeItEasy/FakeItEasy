namespace FakeItEasy
{
    using System;
    using System.Collections.Generic;
    using System.IO;

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
        /// <see cref="IDummyDefinition"/>s, <see cref="IArgumentValueFormatter"/>s, and 
        /// <see cref="IFakeConfigurator"/>s.
        /// </summary>
        /// <remark>This implementation returns the absolute paths of all the DLLs in the
        /// <see cref="System.Environment.CurrentDirectory"/>.
        /// </remark>
        /// <returns>A list of absolute paths of to assemblies to scan for extension points.</returns>
        public virtual IEnumerable<string> GetAssemblyFileNamesToScanForExtensions()
        {
            return Directory.GetFiles(Environment.CurrentDirectory, "*.dll");
        }
    }
}
