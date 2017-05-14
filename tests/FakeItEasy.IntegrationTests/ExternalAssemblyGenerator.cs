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
            this.AssemblyOriginalPath = this.PrepareAssemblyPath("Original");
            this.AssemblyCopyPath = this.PrepareAssemblyPath("Copy");
            this.EmitAssembly();
            this.CopyAssembly();
        }

        /// <summary>
        /// Gets the path to a generated assembly that contains extension points, but is not referenced by the test assembly.
        /// </summary>
        public string AssemblyOriginalPath { get; }

        /// <summary>
        /// Gets the path to a copy of <see cref="AssemblyOriginalPath"/>.
        ///
        public string AssemblyCopyPath { get; }

        private const string AssemblyName = "FakeItEasy.ExtensionPoints.External";

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

        private string PrepareAssemblyPath(string subDirectory)
        {
            string directory = Path.Combine(this.baseDirectory, subDirectory);
            Directory.CreateDirectory(directory);
            return Path.Combine(directory, AssemblyName + ".dll");
        }

        private void EmitAssembly()
        {
            string assemblyContent = @"
namespace FakeItEasy.IntegrationTests.External
{
    using System;
    using FakeItEasy;

    public class GuidValueFormatter : ArgumentValueFormatter<Guid>
    {
        protected override string GetStringValue(Guid argumentValue)
        {
            return argumentValue.ToString(""B"");
        }
    }
}
";

            var references = GetFrameworkAssemblyLocations()
            .Concat(new[]
            {
                typeof(A).GetTypeInformation().Assembly.Location
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