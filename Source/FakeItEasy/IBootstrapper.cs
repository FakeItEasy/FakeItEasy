namespace FakeItEasy
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// Bootstrapper for FakeItEasy.
    /// </summary>
    public interface IBootstrapper
    {
        /// <summary>
        /// Provides a list of assemblies to scan for extension points.
        /// </summary>
        /// <returns>A list of assemblies to scan for extension points.</returns>
        IEnumerable<string> GetAssemblyFilenamesToScanForExtensions();
    }
}
