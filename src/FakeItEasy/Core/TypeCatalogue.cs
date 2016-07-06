namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
#if !FEATURE_REFLECTION_GETASSEMBLIES
    using System.Runtime.Loader;
    using Microsoft.Extensions.DependencyModel;
#endif

    /// <summary>
    /// Provides access to all types in:
    /// <list type="bullet">
    ///   <item>FakeItEasy,</item>
    ///   <item>assemblies currently loaded that reference FakeItEasy and</item>
    ///   <item>assemblies whose paths are supplied to the constructor, that also reference FakeItEasy.</item>
    /// </list>
    /// </summary>
    internal class TypeCatalogue : ITypeCatalogue
    {
        private readonly List<Type> availableTypes = new List<Type>();

        /// <summary>
        /// Gets the <c>FakeItEasy</c> assembly.
        /// </summary>
#if FEATURE_REFLECTION_GETASSEMBLIES
        public static Assembly FakeItEasyAssembly { get; } = Assembly.GetExecutingAssembly();
#else
        public static Assembly FakeItEasyAssembly { get; } = typeof(TypeCatalogue).GetTypeInfo().Assembly;
#endif

        /// <summary>
        /// Loads the available types into the <see cref="TypeCatalogue"/>.
        /// </summary>
        /// <param name="extraAssemblyFiles">
        /// The full paths to assemblies from which to load types,
        /// as well as assemblies loaded currently loaded.
        /// </param>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Defensive and performed on best effort basis.")]
        public void Load(IEnumerable<string> extraAssemblyFiles)
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
                catch (Exception ex)
                {
                    WarnFailedToGetTypes(assembly, ex);
                    continue;
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
#if FEATURE_REFLECTION_GETASSEMBLIES
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
#else
            var fakeItEasyLibraryName = TypeCatalogue.FakeItEasyAssembly.GetName().Name;

            var context = DependencyContext.Default;
            var loadedAssemblies = context.RuntimeLibraries
                .SelectMany(library => library.GetDefaultAssemblyNames(context))
                .Distinct()
                .Select(Assembly.Load)
                .ToArray();
#endif
            var loadedAssembliesReferencingFakeItEasy = loadedAssemblies.Where(assembly => assembly.ReferencesFakeItEasy());

            // Find the paths of already loaded assemblies so we don't double scan them.
            // Exclude the ReflectionOnly assemblies because we want to be able to fully load them if we need to.
            var loadedAssemblyFiles = new HashSet<string>(
                loadedAssemblies
#if FEATURE_REFLECTION_GETASSEMBLIES
                    .Where(a => !a.ReflectionOnly && !a.IsDynamic)
#else
                    .Where(a => !a.IsDynamic)
#endif
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
#if FEATURE_REFLECTION_GETASSEMBLIES
                    reflectedAssembly = Assembly.ReflectionOnlyLoadFrom(file);
#else
                    reflectedAssembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(file);
#endif
                }
                catch (Exception ex)
                {
                    WarnFailedToLoadAssembly(file, ex);
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
                catch (Exception ex)
                {
                    WarnFailedToLoadAssembly(file, ex);
                    continue;
                }

                yield return loadedAssembly;
            }
        }

        private static void WarnFailedToLoadAssembly(string path, Exception ex)
        {
            Write(
                ex,
                "Warning: FakeItEasy failed to load assembly '{0}' while scanning for extension points. Any IArgumentValueFormatters, IDummyFactories, and IFakeOptionsBuilders in that assembly will not be available.",
                path);
        }

        private static void WarnFailedToGetTypes(Assembly assembly, Exception ex)
        {
            Write(
                ex,
                "Warning: FakeItEasy failed to get types from assembly '{0}' while scanning for extension points. Any IArgumentValueFormatters, IDummyFactories, and IFakeOptionsBuilders in that assembly will not be available.",
                assembly);
        }

        private static void Write(Exception ex, string messageFormat, params object[] messageArgs)
        {
            var writer = new DefaultOutputWriter(Console.Write);
            writer.Write(messageFormat, messageArgs);
            writer.WriteLine();
            using (writer.Indent())
            {
                writer.Write(ex.Message);
                writer.WriteLine();
            }
        }
    }
}
