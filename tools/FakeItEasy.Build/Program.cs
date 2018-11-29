namespace FakeItEasy.Build
{
    using System.Collections.Generic;
    using System.IO;
    using static Bullseye.Targets;
    using static SimpleExec.Command;

    public class Program
    {
        private const string LogsDirectory = "artifacts/logs";
        private const string Solution = "FakeItEasy.sln";
        private const string VersionInfoFile = "src/VersionInfo.cs";
        private const string RepoUrl = "https://github.com/FakeItEasy/FakeItEasy";

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

        private static readonly string OutputDirectory = Path.GetFullPath("artifacts/output");

        private static string version = null;

        public static void Main(string[] args)
        {
            Target("default", DependsOn("unit", "integ", "spec", "approve", "pack"));

            Target("outputDirectory", () => Directory.CreateDirectory(OutputDirectory));

            Target("logsDirectory", () => Directory.CreateDirectory(LogsDirectory));

            Target(
                "build",
                DependsOn("versionInfoFile"),
                () => Run("dotnet", $"build {Solution} -c Release /maxcpucount /nr:false /verbosity:minimal /nologo /bl:artifacts/logs/build.binlog"));

            Target("versionInfoFile", () => Run(ToolPaths.GitVersion, $"/updateAssemblyInfo {VersionInfoFile} /ensureAssemblyInfo"));

            foreach (var testSuite in TestSuites)
            {
                Target(
                    testSuite.Key,
                    DependsOn("build"),
                    forEach: testSuite.Value,
                    action: testDirectory => RunTests(testDirectory));
            }

            Target("get-version", () => version = Read(ToolPaths.GitVersion, "/showvariable NuGetVersionV2"));

            Target(
                "pack",
                DependsOn("build", "outputDirectory", "pdbgit", "get-version"),
                forEach: ProjectsToPack,
                action: project => Run("dotnet", $"pack {project} --configuration Release --no-build --output {OutputDirectory} /p:Version={version}"));

            Target(
                "pdbgit",
                DependsOn("build"),
                forEach: Pdbs,
                action: pdb => Run(ToolPaths.PdbGit, $"-u {RepoUrl} -s {pdb}"));

            RunTargets(args);
        }

        private static void RunTests(string testDirectory) =>
            Run("dotnet", "test --configuration Release --no-build -- RunConfiguration.NoAutoReporters=true", testDirectory);
    }
}
