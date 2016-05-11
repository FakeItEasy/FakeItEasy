namespace FakeItEasy.Core
{
    using System.Linq;
    using System.Reflection;

    internal static class AssemblyExtensions
    {
        /// <summary>
        /// Determines whether an assembly references FakeItEasy.
        /// </summary>
        /// <param name="assembly">The assembly to check.</param>
        /// <returns>Whether or not the assembly references FakeItEasy.</returns>
        public static bool ReferencesFakeItEasy(this Assembly assembly)
        {
            Guard.AgainstNull(assembly, "assembly");

#if FEATURE_REFLECTION_GETASSEMBLIES
            return assembly.GetReferencedAssemblies().Any(r => r.FullName == TypeCatalogue.FakeItEasyAssembly.FullName);
#else
            var fakeItEasyLibraryName = TypeCatalogue.FakeItEasyAssembly.GetName().Name;
            var context = Microsoft.Extensions.DependencyModel.DependencyContext.Default;

            return context.RuntimeLibraries.Any(
                library => library.Dependencies.Any(
                    dependency => string.Equals(dependency.Name, fakeItEasyLibraryName, System.StringComparison.Ordinal)));
#endif
        }

        /// <summary>
        /// Gets the simple name of the assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>The simple name of the assembly.</returns>
        public static string Name(this Assembly assembly)
        {
            return new AssemblyName(assembly.FullName).Name;
        }
    }
}
