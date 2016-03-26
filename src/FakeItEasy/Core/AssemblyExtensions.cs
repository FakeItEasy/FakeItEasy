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
            var referencingLibraries = Microsoft.Extensions.PlatformAbstractions.PlatformServices.Default.LibraryManager.GetReferencingLibraries(fakeItEasyLibraryName);
            return referencingLibraries
                .SelectMany(info => info.Assemblies)
                .Select(info => Assembly.Load(info))
                .Any(r => r.FullName == assembly.FullName);
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

        /// <summary>
        /// Gets a value indicating whether a given assembly was generated dynamically by using reflection emit.
        /// </summary>
        /// <remarks>This extension works in .NET 3.5, where <c>Type.IsDynamic</c> does not exist.</remarks>
        /// <param name="assembly">The assembly to check.</param>
        /// <returns>Whether or not the assembly is dynamically generated.</returns>
        public static bool IsDynamic(this Assembly assembly)
        {
#if NET40_OR_LATER
            return assembly.IsDynamic;
#else
            return assembly.ManifestModule.GetType().Namespace == "System.Reflection.Emit";
#endif
        }
    }
}
