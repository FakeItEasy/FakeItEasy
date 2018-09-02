namespace FakeItEasy.Build
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using static Bullseye.Targets;
    using static SimpleExec.Command;

    public class Program
    {
        private const string TestsDirectory = "artifacts/tests";
        private const string LogsDirectory = "artifacts/logs";
        private const string Solution = "FakeItEasy.sln";
        private const string VersionInfoFile = "src/VersionInfo.cs";
        private const string RepoUrl = "https://github.com/FakeItEasy/FakeItEasy";
        private const string AnalyzerMetaPackageNuspecPath = "src/FakeItEasy.Analyzer.nuspec";

        private static readonly string[] ProjectsToPack =
        {
            "src/FakeItEasy/FakeItEasy.csproj",
            "src/FakeItEasy.Analyzer/FakeItEasy.Analyzer.CSharp.csproj",
            "src/FakeItEasy.Analyzer/FakeItEasy.Analyzer.VisualBasic.csproj"
        };

        private static readonly string[] Pdbs =
        {
            "src/FakeItEasy/bin/Release/net40/FakeItEasy.pdb",
            "src/FakeItEasy/bin/Release/net45/FakeItEasy.pdb",
            "src/FakeItEasy/bin/Release/netstandard1.6/FakeItEasy.pdb",
            "src/FakeItEasy.Analyzer/bin/Release/FakeItEasy.Analyzer.Csharp.pdb",
            "src/FakeItEasy.Analyzer/bin/Release/FakeItEasy.Analyzer.VisualBasic.pdb"
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

        private static string msBuild = null;
        private static string version = null;

        public static void Main(string[] args)
        {
            Target("default", DependsOn("unit", "integ", "spec", "approve", "pack"));

            Target("outputDirectory", () => Directory.CreateDirectory(OutputDirectory));

            Target("logsDirectory", () => Directory.CreateDirectory(LogsDirectory));

            Target("testsDirectory", () => Directory.CreateDirectory(TestsDirectory));

            Target("find-msbuild", () => msBuild = $"{GetVSLocation()}/MSBuild/15.0/Bin/MSBuild.exe");

            Target("build", DependsOn("clean", "restore", "versionInfoFile", "find-msbuild"), () => RunMsBuild("Build"));

            Target("versionInfoFile", () => Run(ToolPaths.GitVersion, $"/updateAssemblyInfo {VersionInfoFile} /ensureAssemblyInfo"));

            Target("clean", DependsOn("logsDirectory", "find-msbuild"), () => RunMsBuild("Clean"));

            Target(
                "restore",
                () => Run("dotnet", $"restore"));

            foreach (var testSuite in TestSuites)
            {
                Target(
                    testSuite.Key,
                    DependsOn("build", "testsDirectory"),
                    forEach: testSuite.Value,
                    action: testDirectory => RunTests(testDirectory));
            }

            Target("get-version", () => version = Read(ToolPaths.GitVersion, "/showvariable NuGetVersionV2"));

            Target(
                "pack-projects",
                DependsOn("build", "outputDirectory", "pdbgit", "get-version"),
                forEach: ProjectsToPack,
                action: project => Run("dotnet", $"pack {project} --configuration Release --no-build --output {OutputDirectory} /p:Version={version}"));

            Target(
                "pack-nuspecs",
                DependsOn("outputDirectory", "get-version"),
                () => Run(ToolPaths.NuGet, $"pack {AnalyzerMetaPackageNuspecPath} -Version {version} -OutputDirectory {OutputDirectory} -NoPackageAnalysis"));

            Target(
                "pack",
                DependsOn("pack-projects", "pack-nuspecs"));

            Target(
                "pdbgit",
                DependsOn("build"),
                forEach: Pdbs,
                action: pdb => Run(ToolPaths.PdbGit, $"-u {RepoUrl} -s {pdb}"));

            RunTargets(args);
        }

        private static void RunMsBuild(string target)
        {
            Run(
                msBuild,
                $"{Solution} /target:{target} /p:configuration=Release /maxcpucount /nr:false /verbosity:minimal /nologo /bl:artifacts/logs/{target}.binlog");
        }

        private static void RunTests(string testDirectory)
        {
            var xml = Path.GetFullPath(Path.Combine(TestsDirectory, Path.GetFileName(testDirectory) + ".TestResults.xml"));
            Run("dotnet", $"xunit -configuration Release -nologo -nobuild -noautoreporters -notrait \"explicit=yes\" -xml {xml}", testDirectory);
        }

        private static string GetVSLocation()
        {
            var installationPath = Read($"\"{ToolPaths.VSWhere}\"", "-nologo -latest -property installationPath -requires Microsoft.Component.MSBuild -version [15,16)");
            if (string.IsNullOrWhiteSpace(installationPath))
            {
                throw new InvalidOperationException("Visual Studio 2017 was not found");
            }

            return installationPath.Trim();
        }
    }
}
