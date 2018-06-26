#r "packages/Bullseye.1.0.0-rc.4/lib/netstandard2.0/Bullseye.dll"
#r "packages/SimpleExec.2.1.0/lib/netstandard1.3/SimpleExec.dll"

using System.Runtime.CompilerServices;
using Bullseye;
using static Bullseye.Targets;
using static SimpleExec.Command;

var solution = "./FakeItEasy.sln";
var versionInfoFile = "./src/VersionInfo.cs";
var repoUrl = "https://github.com/FakeItEasy/FakeItEasy";

var projectsToPack = new[]
{
    "./src/FakeItEasy/FakeItEasy.csproj",
    "./src/FakeItEasy.Analyzer/FakeItEasy.Analyzer.CSharp.csproj",
    "./src/FakeItEasy.Analyzer/FakeItEasy.Analyzer.VisualBasic.csproj"
};
var analyzerMetaPackageNuspecPath = "./src/FakeItEasy.Analyzer.nuspec";

var pdbs = new []
{
    "src/FakeItEasy/bin/Release/net40/FakeItEasy.pdb",
    "src/FakeItEasy/bin/Release/netstandard1.6/FakeItEasy.pdb",
    "src/FakeItEasy.Analyzer/bin/Release/FakeItEasy.Analyzer.Csharp.pdb",
    "src/FakeItEasy.Analyzer/bin/Release/FakeItEasy.Analyzer.VisualBasic.pdb"
};

var testSuites = new Dictionary<string, string[]>
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

// tool locations

static var toolsPackagesDirectory = Path.Combine(GetCurrentScriptDirectory(), "packages");
var vswhere = $"{toolsPackagesDirectory}/vswhere.1.0.62/tools/vswhere.exe";
var gitversion = $"{toolsPackagesDirectory}/GitVersion.CommandLine.4.0.0-beta0012/tools/GitVersion.exe";
var msBuild = $"{GetVSLocation()}/MSBuild/15.0/Bin/MSBuild.exe";
var nuget = $"{GetCurrentScriptDirectory()}/.nuget/NuGet.exe";
var pdbGit = $"{toolsPackagesDirectory}/pdbGit.3.0.41/tools/PdbGit.exe";
static var xunit = $"{toolsPackagesDirectory}/xunit.runner.console.2.0.0/tools/xunit.console.exe";

// artifact locations
var logsDirectory = "./artifacts/logs";
var outputDirectory = Path.GetFullPath("./artifacts/output");
static var testsDirectory = "./artifacts/tests";

// targets
Targets.Add("default", DependsOn("unit", "integ", "spec", "approve", "pack"));

Targets.Add("outputDirectory", () => Directory.CreateDirectory(outputDirectory));

Targets.Add("logsDirectory", () => Directory.CreateDirectory(logsDirectory));

Targets.Add("testsDirectory", () => Directory.CreateDirectory(testsDirectory));

Targets.Add("build", DependsOn("clean", "restore", "versionInfoFile"), () => RunMsBuild("Build"));

Targets.Add("versionInfoFile", () => Run(gitversion, $"/updateAssemblyInfo {versionInfoFile} /ensureAssemblyInfo"));

Targets.Add("clean", DependsOn("logsDirectory"), () => RunMsBuild("Clean"));

Targets.Add(
    "restore",
    () => Run("dotnet", $"restore"));

Targets.Add(
    "unit",
    DependsOn("build", "testsDirectory"),
    () => RunTests("unit"));

Targets.Add(
    "integ",
    DependsOn("build", "testsDirectory"),
    () => RunTests("integ"));

Targets.Add(
    "spec",
    DependsOn("build", "testsDirectory"),
    () => RunTests("spec"));

Targets.Add(
    "approve",
    DependsOn("build", "testsDirectory"),
    () => RunTests("approve"));

Targets.Add(
    "pack",
    DependsOn("build", "outputDirectory", "pdbgit"),
    () =>
    {
        var version = Read(gitversion, "/showvariable NuGetVersionV2", ".");
        foreach (var project in projectsToPack)
        {
            Run("dotnet", $"pack {project} --configuration Release --no-build --output {outputDirectory} /p:Version={version}");
        }

        Run(nuget, $"pack {analyzerMetaPackageNuspecPath} -Version {version} -OutputDirectory {outputDirectory} -NoPackageAnalysis");
    });

Targets.Add(
    "pdbgit",
    DependsOn("build"),
    () =>
    {
        foreach (var pdb in pdbs)
        {
            Run(pdbGit, $"-u {repoUrl} -s {pdb}");
        }
    });

Targets.Run(Args);

// helpers
public void RunMsBuild(string target)
{
    Run(
        msBuild,
        $"{solution} /target:{target} /p:configuration=Release /maxcpucount /nr:false /verbosity:minimal /nologo /bl:artifacts/logs/{target}.binlog");
}

public void RunTests(string target)
{
    foreach (var directory in testSuites[target])
    {
        RunTestsInDirectory(directory);
    }
}

public void RunTestsInDirectory(string testDirectory)
{
    var xml = Path.GetFullPath(Path.Combine(testsDirectory, Path.GetFileName(testDirectory) + ".TestResults.xml"));
    Run("dotnet", $"xunit -configuration Release -nologo -nobuild -noautoreporters -notrait \"explicit=yes\" -xml {xml}", testDirectory);
}

public string GetVSLocation()
{
    var installationPath = Read($"\"{vswhere}\"", "-nologo -latest -property installationPath -requires Microsoft.Component.MSBuild -version [15,16)", ".");
    if (string.IsNullOrEmpty(installationPath))
    {
        throw new InvalidOperationException("Visual Studio 2017 was not found");
    }

    return installationPath.Trim();
}

public static string GetCurrentScriptDirectory([CallerFilePath] string path = null) => Path.GetDirectoryName(path);
