namespace FakeItEasy.Core;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

/// <summary>
/// Provides access to all types in:
/// <list type="bullet">
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
    public static Assembly FakeItEasyAssembly { get; } = Assembly.GetExecutingAssembly();

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
                foreach (var type in ex.Types.Where(t => t is not null))
                {
                    this.availableTypes.Add(type!);
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

    private static HashSet<Assembly> GetAllAssemblies(IEnumerable<string> extraAssemblyFiles)
    {
        var assembliesToScan = new HashSet<Assembly>();
        var loadedAssemblyFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (!assembly.ReferencesFakeItEasy())
            {
                continue;
            }

            assembliesToScan.Add(assembly);
            if (!assembly.IsDynamic)
            {
                loadedAssemblyFiles.Add(assembly.Location);
            }

            foreach (var referencedAssemblyName in assembly.GetReferencedAssemblies())
            {
                if (TryLoadAssemblyIfItReferencesFakeItEasy(referencedAssemblyName, out var referencedAssembly) &&
                    assembliesToScan.Add(referencedAssembly))
                {
                    if (!referencedAssembly.IsDynamic)
                    {
                        loadedAssemblyFiles.Add(referencedAssembly.Location);
                    }
                }
            }
        }

        // Skip assemblies that have already been loaded.
        // This optimization can be fooled by test runners that make shadow copies of the assemblies but it's a start.
        var extraAssemblies = GetAssemblies(extraAssemblyFiles.Except(loadedAssemblyFiles));
        foreach (var assembly in extraAssemblies)
        {
            if (!assembly.ReferencesFakeItEasy())
            {
                continue;
            }

            assembliesToScan.Add(assembly);
        }

        return assembliesToScan;
    }

    private static IEnumerable<Assembly> GetAssemblies(IEnumerable<string> files)
    {
        foreach (var file in files)
        {
            if (TryLoadAssemblyIfItReferencesFakeItEasy(file, out var assembly))
            {
                yield return assembly;
            }
        }
    }

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Appropriate in try methods.")]
    private static bool TryLoadAssemblyIfItReferencesFakeItEasy(string file, [NotNullWhen(true)] out Assembly? assembly)
    {
        assembly = null;
        try
        {
            assembly = Assembly.LoadFrom(file);
        }
        catch (Exception ex)
        {
            WarnFailedToLoadAssembly(file, ex);
            return false;
        }

        return assembly.ReferencesFakeItEasy();
    }

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Appropriate in try methods.")]
    private static bool TryLoadAssemblyIfItReferencesFakeItEasy(AssemblyName assemblyName, [NotNullWhen(true)] out Assembly? assembly)
    {
        assembly = null;
        try
        {
            assembly = Assembly.Load(assemblyName);
        }
        catch (Exception ex)
        {
            WarnFailedToLoadAssembly(assemblyName.ToString(), ex);
            return false;
        }

        return assembly.ReferencesFakeItEasy();
    }

    private static void WarnFailedToLoadAssembly(string pathOrName, Exception ex)
    {
        Write(
            ex,
            $"Warning: FakeItEasy failed to load assembly '{pathOrName}' while scanning for extension points. Any IArgumentValueFormatters, IDummyFactories, and IFakeOptionsBuilders in that assembly will not be available.");
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
            foreach (var loaderException in ex.LoaderExceptions.Where(ex => ex is not null))
            {
                writer.Write(" - ");
                writer.Write(loaderException!.GetType());
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

    private static void Write(DefaultOutputWriter writer, Exception ex, string message)
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

    private static DefaultOutputWriter CreateConsoleWriter()
    {
        // We can pass null as the ArgumentValueFormatter, because we never call
        // WriteArgumentValue on this writer.
        return new DefaultOutputWriter(Console.Write, null!);
    }
}
