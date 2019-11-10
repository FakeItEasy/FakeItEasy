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
    ///   <item>currently loaded assemblies that reference FakeItEasy and</item>
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
        /// as well as currently loaded assemblies.
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
                catch (ReflectionTypeLoadException ex)
                {
                    // In this case, some types failed to load.
                    // Just keep the types that were successfully loaded, and warn about the rest.
                    foreach (var type in ex.Types.Where(t => t is object))
                    {
                        this.availableTypes.Add(type);
                    }

                    WarnFailedToGetSomeTypes(assembly, ex);
                }
                catch (Exception ex)
                {
                    WarnFailedToGetTypes(assembly, ex);
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
            var context = DependencyContext.Default;
            var loadedAssemblies = context.RuntimeLibraries
                .SelectMany(library => library.GetDefaultAssemblyNames(context))
                .Distinct()
                .Select(Assembly.Load)
                .ToArray();
#endif
            var loadedAssembliesReferencingFakeItEasy = loadedAssemblies.Where(assembly => assembly.ReferencesFakeItEasy());

            // Find the paths of already loaded assemblies so we don't double scan them.
            var loadedAssemblyFiles = new HashSet<string>(
                loadedAssemblies
#if FEATURE_REFLECTIONONLYLOAD
                    // Exclude the ReflectionOnly assemblies because we may fully load them later.
                    .Where(a => !a.ReflectionOnly)
#endif
                    .Where(a => !a.IsDynamic)
                    .Select(a => a.Location),
                StringComparer.OrdinalIgnoreCase);

            // Skip assemblies that have already been loaded.
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
                Assembly assembly;
                try
                {
#if FEATURE_REFLECTIONONLYLOAD
                    assembly = Assembly.ReflectionOnlyLoadFrom(file);
#elif USE_RUNTIMELOADER
                    assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(file);
#else
                    assembly = Assembly.LoadFrom(file);
#endif
                }
                catch (Exception ex)
                {
                    WarnFailedToLoadAssembly(file, ex);
                    continue;
                }

                if (!assembly.ReferencesFakeItEasy())
                {
                    continue;
                }

#if FEATURE_REFLECTIONONLYLOAD
                // A reflection-only loaded assembly can't be scanned for types, so fully load it before returning it.
                try
                {
                    assembly = Assembly.Load(assembly.GetName());
                }
                catch (Exception ex)
                {
                    WarnFailedToLoadAssembly(file, ex);
                    continue;
                }
#endif

                yield return assembly;
            }
        }

        private static void WarnFailedToLoadAssembly(string path, Exception ex)
        {
            Write(
                ex,
                $"Warning: FakeItEasy failed to load assembly '{path}' while scanning for extension points. Any IArgumentValueFormatters, IDummyFactories, and IFakeOptionsBuilders in that assembly will not be available.");
        }

        private static void WarnFailedToGetTypes(Assembly assembly, Exception ex)
        {
            Write(
                ex,
                $"Warning: FakeItEasy failed to get types from assembly '{assembly}' while scanning for extension points. Any IArgumentValueFormatters, IDummyFactories, and IFakeOptionsBuilders in that assembly will not be available.");
        }

        private static void WarnFailedToGetSomeTypes(Assembly assembly, ReflectionTypeLoadException ex)
        {
            var writer = CreateConsoleWriter();
            Write(
                writer,
                ex,
                $"Warning: FakeItEasy failed to get some types from assembly '{assembly}' while scanning for extension points. Some IArgumentValueFormatters, IDummyFactories, and IFakeOptionsBuilders in that assembly might not be available.");

            using (writer.Indent())
            {
                int notLoadedCount = ex.Types.Count(t => t is null);
                writer.Write($"{notLoadedCount} type(s) were not loaded for the following reasons:");
                writer.WriteLine();
                foreach (var loaderException in ex.LoaderExceptions)
                {
                    writer.Write(" - ");
                    writer.Write(loaderException.GetType());
                    writer.Write(": ");
                    writer.Write(loaderException.Message);
                    writer.WriteLine();
                }
            }
        }

        private static void Write(Exception ex, string message)
        {
            var writer = CreateConsoleWriter();
            Write(writer, ex, message);
        }

        private static void Write(IOutputWriter writer, Exception ex, string message)
        {
            writer.Write(message);
            writer.WriteLine();
            using (writer.Indent())
            {
                writer.Write(ex.GetType());
                writer.Write(": ");
                writer.Write(ex.Message);
                writer.WriteLine();
            }
        }

        private static IOutputWriter CreateConsoleWriter()
        {
            // We can pass null as the ArgumentValueFormatter, because we never call
            // WriteArgumentValue on this writer.
            return new DefaultOutputWriter(Console.Write, null!);
        }
    }
}
