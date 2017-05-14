namespace FakeItEasy.IntegrationTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using FakeItEasy.Tests.TestHelpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    public class ExternalAssemblyGenerator
    {
        public ExternalAssemblyGenerator()
        {
            this.CreateBaseDirectory();
            this.AssemblyOriginalPath = this.PrepareAssemblyPath("Original", AssemblyName);
            this.AssemblyCopyPath = this.PrepareAssemblyPath("Copy", AssemblyName);
            this.AssemblyDependencyPath = this.PrepareAssemblyPath("Dependency", DependencyAssemblyName);
            this.EmitDependencyAssembly();
            this.EmitAssembly();
            this.CopyAssembly();
        }

        /// <summary>
        /// Gets the path to a generated assembly that contains extension points, but is not referenced by the test assembly.
        /// </summary>
        public string AssemblyOriginalPath { get; }

        /// <summary>
        /// Gets the path to a copy of <see cref="AssemblyOriginalPath"/>.
        /// </summary>
        public string AssemblyCopyPath { get; }

        /// <summary>
        /// Gets the path of an assembly on which a type of FakeItEasy.IntegrationTests.External depends.
        /// </summary>
        /// <remarks>
        /// This assembly is referenced by FakeItEasy.ExtensionPoints.External, but won't be available at run time. This is done in order to
        /// cause a type load error when we scan the types in FakeItEasy.ExtensionPoints.External, and ensure the error is handled properly.
        /// </remarks>
        public string AssemblyDependencyPath { get; }

        private const string AssemblyName = "FakeItEasy.ExtensionPoints.External";
        private const string DependencyAssemblyName = "FakeItEasy.ExtensionPoints.ExternalDependency";

        private static IEnumerable<string> GetFrameworkAssemblyLocations()
        {
            var systemAssemblyLocation = typeof(object).GetTypeInformation().Assembly.Location;
            var coreDir = Path.GetDirectoryName(systemAssemblyLocation);
            return new[] { "mscorlib.dll", "System.Runtime.dll" }
                .Select(s => Path.Combine(coreDir, s))
                .Concat(new[]
                {
                    systemAssemblyLocation
                });
        }

        private string baseDirectory;

        private void CreateBaseDirectory()
        {
            this.baseDirectory = Path.Combine(Path.GetTempPath(), AssemblyName);
            if (Directory.Exists(this.baseDirectory))
            {
                Directory.Delete(this.baseDirectory, recursive: true);
            }

            Directory.CreateDirectory(this.baseDirectory);
        }

        private string PrepareAssemblyPath(string subDirectory, string assemblyName)
        {
            string directory = Path.Combine(this.baseDirectory, subDirectory);
            Directory.CreateDirectory(directory);
            return Path.Combine(directory, assemblyName + ".dll");
        }

        private void EmitDependencyAssembly()
        {
            string assemblyContent = @"
namespace FakeItEasy.IntegrationTests.ExternalDependency
{
    public class Foo { }
}";

            var references = GetFrameworkAssemblyLocations()
                .Select(l => MetadataReference.CreateFromFile(l));

            var compilation = CSharpCompilation.Create(
                DependencyAssemblyName,
                syntaxTrees: new[] { CSharpSyntaxTree.ParseText(assemblyContent) },
                references: references,
                options: new CSharpCompilationOptions(outputKind: OutputKind.DynamicallyLinkedLibrary));

            var emitResult = compilation.Emit(this.AssemblyDependencyPath);
            if (!emitResult.Success)
            {
                throw new Exception("Failed to create assembly - " + emitResult.Diagnostics[0]);
            }
        }

        private void EmitAssembly()
        {
            string assemblyContent = @"
namespace FakeItEasy.IntegrationTests.External
{
    using System;
    using FakeItEasy;
    using FakeItEasy.IntegrationTests.ExternalDependency;

    public class GuidValueFormatter : ArgumentValueFormatter<Guid>
    {
        protected override string GetStringValue(Guid argumentValue)
        {
            return argumentValue.ToString(""B"");
        }
    }

    // Will fail to load if the assembly containing Foo cannot be found
    public class FooValueFormatter : ArgumentValueFormatter<Foo>
    {
        protected override string GetStringValue(Foo argumentValue)
        {
            return ""Foo"";
        }
    }
}
";

            var references = GetFrameworkAssemblyLocations()
            .Concat(new[]
            {
                typeof(A).GetTypeInformation().Assembly.Location,
                this.AssemblyDependencyPath
            })
            .Select(l => MetadataReference.CreateFromFile(l));

            var compilation = CSharpCompilation.Create(
                AssemblyName,
                syntaxTrees: new[] { CSharpSyntaxTree.ParseText(assemblyContent) },
                references: references,
                options: new CSharpCompilationOptions(outputKind: OutputKind.DynamicallyLinkedLibrary));

            var emitResult = compilation.Emit(this.AssemblyOriginalPath);
            if (!emitResult.Success)
            {
                throw new Exception("Failed to create assembly - " + emitResult.Diagnostics[0]);
            }
        }

        private void CopyAssembly()
        {
            File.Copy(this.AssemblyOriginalPath, this.AssemblyCopyPath);
        }
    }
}