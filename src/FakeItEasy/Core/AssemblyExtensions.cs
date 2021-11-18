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
            Guard.AgainstNull(assembly);

            return assembly.GetReferencedAssemblies().Any(r => r.FullName == TypeCatalogue.FakeItEasyAssembly.FullName);
        }
    }
}
