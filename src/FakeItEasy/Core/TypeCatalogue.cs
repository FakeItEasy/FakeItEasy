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
    internal class TypeCatalogue : ITypeCatalogue
    {
#if !FEATURE_NETCORE_REFLECTION || NET45
        private static readonly Assembly ExecutingAssembly = Assembly.GetExecutingAssembly();
#else
        private static readonly Assembly ExecutingAssembly = typeof(TypeCatalogue).GetTypeInfo().Assembly;
#endif
        private readonly List<Type> availableTypes = new List<Type>();

        /// <summary>
        /// Gets the <c>FakeItEasy</c> assembly.
        /// </summary>
        public static Assembly FakeItEasyAssembly
        {
            get { return ExecutingAssembly; }
        }

        /// <summary>
        /// Loads the available types into the <see cref="TypeCatalogue"/>.
        /// </summary>
        /// <param name="extraAssemblyFiles">
        /// The full paths to assemblies from which to load types,
        /// as well as assemblies loaded into the current <see cref="AppDomain"/>.
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
#if !FEATURE_NETCORE_REFLECTION || NET45
             var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
             var loadedAssembliesReferencingFakeItEasy = loadedAssemblies.Where(assembly => assembly.ReferencesFakeItEasy());
#else
            var coreAssemblyName = typeof(object).GetTypeInfo().Assembly.Name();
            var loadedAssemblies = Microsoft.Extensions.PlatformAbstractions.PlatformServices.Default.LibraryManager.GetReferencingLibraries(coreAssemblyName)
                .SelectMany(info => info.Assemblies)
                .Select(info => Assembly.Load(new AssemblyName(info.Name)))
                .ToArray();
            var loadedAssembliesReferencingFakeItEasy = loadedAssemblies.Where(assembly => assembly.ReferencesFakeItEasy());
#endif

            // Find the paths of already loaded assemblies so we don't double scan them.
            // Exclude the ReflectionOnly assemblies because we want to be able to fully load them if we need to.
            var loadedAssemblyFiles = new HashSet<string>(
                loadedAssemblies
#if !FEATURE_NETCORE_REFLECTION || NET45
                    .Where(a => !a.ReflectionOnly && !a.IsDynamic())
#else
                    .Where(a => a.IsDynamic())
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
#if !FEATURE_NETCORE_REFLECTION || NET45
                    reflectedAssembly = Assembly.ReflectionOnlyLoadFrom(file);
#else
                    reflectedAssembly = CustomLoadContext.Instance.LoadFromFullPath(file);
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
#if FEATURE_NETCORE_REFLECTION && !NET45
    internal class CustomLoadContext : System.Runtime.Loader.AssemblyLoadContext
    {
        public static CustomLoadContext Instance { get; } = new CustomLoadContext();

        private string BasePath { get; set; }
        protected override Assembly Load(AssemblyName assemblyName)
        {
            Console.WriteLine("loading {0}", assemblyName.Name);
            return LoadFromAssemblyPath(System.IO.Path.Combine(BasePath ?? System.IO.Directory.GetCurrentDirectory(), assemblyName.Name));
        }

        public Assembly LoadFromFullPath(string assemblyPath)
        {
            var fileInfo = new System.IO.FileInfo(assemblyPath);
            BasePath = fileInfo.DirectoryName;
            return LoadFromAssemblyPath(assemblyPath);
        }
    }
#endif
}
