namespace FakeItEasy
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Bootstrapper for FakeItEasy.
    /// </summary>
    /// <remarks>
    /// <para>When FakeItEasy is initialized, it scans the executing app domain for implementations
    /// of this interface. If any are found, one will be instantiated and used to bootstrap
    /// FakeItEasy. If no implementations are found, then a <see cref="DefaultBootstrapper"/>
    /// will be used.</para>
    /// <para>The recommended way to implement IBootstrapper is to extend DefaultBootstrapper 
    /// and override selected methods.</para>
    /// </remarks>
    public interface IBootstrapper
    {
        /// <summary>
        /// Provides a list of assembly file names to scan for extension points, such as
        /// <see cref="IDummyFactory"/>s, <see cref="IArgumentValueFormatter"/>s, and 
        /// <see cref="IFakeConfigurator"/>s.
        /// </summary>
        /// <returns>A list of absolute paths pointing to assemblies to scan for extension points.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "A property would not be appropriate here since the operation might perform significant work.")]
        IEnumerable<string> GetAssemblyFileNamesToScanForExtensions();
    }
}
