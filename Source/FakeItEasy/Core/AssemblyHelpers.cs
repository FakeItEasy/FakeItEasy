namespace FakeItEasy.Core
{
    using System.Linq;
    using System.Reflection;

    internal static class AssemblyHelpers
    {
        private static readonly Assembly FakeItEasyAssembly = Assembly.GetExecutingAssembly();

        /// <summary>
        /// Determines whether an assembly references FakeItEasy.
        /// </summary>
        /// <param name="inspectedAssembly">The assembly to check.</param>
        /// <returns>Whether or not the assembly references FakeItEasy.</returns>
        public static bool ReferencesFakeItEasy(Assembly inspectedAssembly)
        {
            return inspectedAssembly.GetReferencedAssemblies().Any(r => r.FullName == FakeItEasyAssembly.FullName);
        }
    }
}
