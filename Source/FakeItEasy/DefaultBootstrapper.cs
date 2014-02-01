namespace FakeItEasy
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// The default bootstrapper.    
    /// </summary>
    public class DefaultBootstrapper : IBootstrapper
    {
        /// <summary>
        /// <para>When overridden in a derived class, provides a custom list of assembly file
        /// names to scan for extension points, such as
        /// <see cref="IDummyDefinition"/>s, <see cref="IArgumentValueFormatter"/>s, and 
        /// <see cref="IFakeConfigurator"/>s.</para>
        /// <para>The default implementation returns the absolute paths of all the DLLs in the
        /// <see cref="System.Environment.CurrentDirectory"/>.</para>
        /// </summary>
        /// <returns>A list of absolute paths pointing to assemblies to scan for extension points.</returns>
        public virtual IEnumerable<string> GetAssemblyFileNamesToScanForExtensions()
        {
            return Directory.GetFiles(Environment.CurrentDirectory, "*.dll");
        }
    }
}
