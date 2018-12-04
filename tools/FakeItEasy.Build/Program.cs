namespace FakeItEasy.Build
{
    using System.Collections.Generic;
    using System.IO;
    using static Bullseye.Targets;
    using static SimpleExec.Command;

    public class Program
    {
        private static readonly string[] ProjectsToPack =
        {
            "src/FakeItEasy/FakeItEasy.csproj",
            "src/FakeItEasy.Analyzer.CSharp/FakeItEasy.Analyzer.CSharp.csproj",
            "src/FakeItEasy.Analyzer.VisualBasic/FakeItEasy.Analyzer.VisualBasic.csproj"
        };

        private static readonly string[] Pdbs =
        {
            "src/FakeItEasy/bin/Release/net40/FakeItEasy.pdb",
            "src/FakeItEasy/bin/Release/net45/FakeItEasy.pdb",
            "src/FakeItEasy/bin/Release/netstandard1.6/FakeItEasy.pdb",
            "src/FakeItEasy/bin/Release/netstandard2.0/FakeItEasy.pdb",
            "src/FakeItEasy.Analyzer.CSharp/bin/Release/FakeItEasy.Analyzer.Csharp.pdb",
            "src/FakeItEasy.Analyzer.VisualBasic/bin/Release/FakeItEasy.Analyzer.VisualBasic.pdb"
        };

        private static readonly IReadOnlyDictionary<string, string[]> TestSuites = new Dictionary<string, string[]>
        {
            ["unit"] = new[]
            {
                "tests/FakeItEasy.Tests",
                "tests/FakeItEasy.Analyzer.CSharp.Tests",
                "tests/FakeItEasy.Analyzer.VisualBasic.Tests",
            },
            ["integ"] = new[]
            {
                "tests/FakeItEasy.IntegrationTests",
                "tests/FakeItEasy.IntegrationTests.VB",
            },
            ["spec"] = new[]
            {
                "tests/FakeItEasy.Specs"
            },
            ["approve"] = new[]
            {
                "tests/FakeItEasy.Tests.Approval"
            }
        };

        public static void Main(string[] args)
        {
            Target("default", DependsOn("unit", "integ", "spec", "approve", "pack"));

            Target(
                "build",
                () => Run("dotnet", "build FakeItEasy.sln -c Release /maxcpucount /nr:false /verbosity:minimal /nologo /bl:artifacts/logs/build.binlog"));

            foreach (var testSuite in TestSuites)
            {
                Target(
                    testSuite.Key,
                    DependsOn("build"),
                    forEach: testSuite.Value,
                    action: testDirectory => Run("dotnet", "test --configuration Release --no-build -- RunConfiguration.NoAutoReporters=true", testDirectory));
            }

            Target(
                "pack",
                DependsOn("build", "pdbgit"),
                forEach: ProjectsToPack,
                action: project => Run("dotnet", $"pack {project} --configuration Release --no-build --output {Path.GetFullPath("artifacts/output")}"));

            Target(
                "pdbgit",
                DependsOn("build"),
                forEach: Pdbs,
                action: pdb => Run(ToolPaths.PdbGit, $"-u https://github.com/FakeItEasy/FakeItEasy -s {pdb}"));

            RunTargets(args);
        }
    }
}
