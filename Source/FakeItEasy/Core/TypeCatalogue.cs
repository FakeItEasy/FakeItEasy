namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Provides access to all types in:
    /// <list type="bullet">
    ///   <item>FakeItEasy,</item>
    ///   <item>assemblies loaded into the current <see cref="AppDomain"/> that reference FakeItEasy and</item>
    ///   <item>assemblies whose paths are supplied to the constructor, that also reference FakeItEasy.</item>
    /// </list>
    /// </summary>
    public class TypeCatalogue : ITypeCatalogue
    {
        private static readonly Assembly FakeItEasyAssembly = Assembly.GetExecutingAssembly();
        private readonly List<Type> availableTypes = new List<Type>();

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeCatalogue"/> class.
        /// </summary>
        /// <param name="extraAssemblyFiles">
        /// The full paths to assemblies from which to load types,
        /// as well as assemblies loaded into the current <see cref="AppDomain"/>.
        /// </param>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Defensive and performed on best effort basis.")]
        public TypeCatalogue(IEnumerable<string> extraAssemblyFiles)
        {
            foreach (var assembly in GetAllAssemblies(extraAssemblyFiles))
            {
                try
                {
                    foreach (var type in assembly.GetTypes())
                    {
                        this.availableTypes.Add(type);
                    }
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Gets a collection of available types.
        /// </summary>
        /// <returns>The available types.</returns>
        public IEnumerable<Type> GetAvailableTypes()
        {
            return this.availableTypes;
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Appropriate in try methods.")]
        private static IEnumerable<Assembly> GetAllAssemblies(IEnumerable<string> extraAssemblyFiles)
        {
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            var loadedAssembliesReferencingFakeItEasy = loadedAssemblies.Where(assembly => assembly.ReferencesFakeItEasy());

            // Find the paths of already loaded assemblies so we don't double scan them.
            // Checking Assembly.IsDynamic would be preferable to the business with the Namespace
            // but the former isn't available in .NET 3.5.
            // Exclude the ReflectionOnly assemblies because we want to be able to fully load them if we need to.
            var loadedAssemblyFiles = new HashSet<string>(
                loadedAssemblies
                    .Where(a => !a.ReflectionOnly && a.ManifestModule.GetType().Namespace != "System.Reflection.Emit")
                    .Select(a => a.Location),
                StringComparer.OrdinalIgnoreCase);

            // Skip assemblies already in the application domain.
            // This optimization can be fooled by test runners that make shadow copies of the assemblies but it's a start.
            return GetAssemblies(extraAssemblyFiles.Except(loadedAssemblyFiles))
                .Concat(loadedAssembliesReferencingFakeItEasy)
                .Concat(FakeItEasyAssembly)
                .Distinct();
        }

        private static IEnumerable<Assembly> GetAssemblies(IEnumerable<string> files)
        {
            foreach (var file in files)
            {
                Assembly reflectedAssembly = null;
                try
                {
                    reflectedAssembly = Assembly.ReflectionOnlyLoadFrom(file);
                }
                catch (BadImageFormatException)
                {
                    // The assembly may not be managed, so move on.
                    continue;
                }
                catch (Exception e)
                {
                    WarnFailedToLoadAssembly(file, e);
                    continue;
                }

                if (!reflectedAssembly.ReferencesFakeItEasy())
                {
                    continue;
                }

                // A reflection-only loaded assembly can't be scanned for types, so fully load it before saving it.
                Assembly loadedAssembly = null;
                try
                {
                    loadedAssembly = Assembly.Load(reflectedAssembly.GetName());
                }
                catch
                {
                    continue;
                }

                yield return loadedAssembly;
            }
        }

        private static void WarnFailedToLoadAssembly(string file, Exception ex)
        {
            var writer = new DefaultOutputWriter(Console.Write);
            writer.Write(
                "Warning: FakeItEasy failed to load assembly '{0}' while scanning for extension points. Any IArgumentValueFormatters, IDummyDefinitions, and IFakeConfigurators in that assembly will not be available.",
                file);
            
            writer.WriteLine();
            using (writer.Indent())
            {
                writer.Write(ex.Message);
                writer.WriteLine();
            }
        }
    }
}
