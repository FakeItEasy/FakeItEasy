namespace FakeItEasy
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Bootstrapper for FakeItEasy.
    /// </summary>
    public interface IBootstrapper
    {
        /// <summary>
        /// Provides a list of assembly file names to scan for extension points, such as
        /// <see cref="IDummyDefinition"/>s, <see cref="IArgumentValueFormatter"/>s, and 
        /// <see cref="IFakeConfigurator"/>s.
        /// </summary>
        /// <returns>A list of absolute paths pointing to assemblies to scan for extension points.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "A property would not be appropriate here since the operation might perform significant work.")]
        IEnumerable<string> GetAssemblyFileNamesToScanForExtensions();
    }
}
