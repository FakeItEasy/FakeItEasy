#load "packages/simple-targets-csx.6.0.0/contentFiles/csx/any/simple-targets.csx"

#r "System.Net.Http"
#r "System.Xml.Linq"

using System.Net.Http;
using System.Xml.Linq;
using System.Runtime.CompilerServices;
using static SimpleTargets;

var solutionName = "FakeItEasy";

var solution = "./" + solutionName + ".sln";
var versionInfoFile = "./src/VersionInfo.cs";
var repoUrl = "https://github.com/FakeItEasy/FakeItEasy";
var coverityProjectUrl = "https://scan.coverity.com/builds?project=FakeItEasy%2FFakeItEasy";

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
var coverityDirectory = "./artifacts/coverity";
var coverityResultsDirectory = "./artifacts/coverity/cov-int";
var logsDirectory = "./artifacts/logs";
var outputDirectory = Path.GetFullPath("./artifacts/output");
static var testsDirectory = "./artifacts/tests";

// targets
var targets = new TargetDictionary();

targets.Add("default", DependsOn("unit", "integ", "spec", "approve", "pack"));

targets.Add("outputDirectory", () => Directory.CreateDirectory(outputDirectory));

targets.Add("coverityDirectory", () => Directory.CreateDirectory(coverityDirectory));

targets.Add("logsDirectory", () => Directory.CreateDirectory(logsDirectory));

targets.Add("testsDirectory", () => Directory.CreateDirectory(testsDirectory));

targets.Add("build", DependsOn("clean", "restore", "versionInfoFile"), () => RunMsBuild("Build"));

targets.Add("versionInfoFile", () => Cmd(gitversion, $"/updateAssemblyInfo {versionInfoFile} /ensureAssemblyInfo"));

targets.Add(
    "coverity",
    DependsOn("clean", "coverityDirectory", "restore"),
    () =>
    {
        Cmd(
            "cov-build",
            $@"--dir {coverityResultsDirectory} ""{msBuild}"" {solution} /target:Build /p:configuration=Release /nr:false /verbosity:minimal /nologo /fl /flp:LogFile=artifacts/logs/Coverity-Build.log;Verbosity=Detailed;PerformanceSummary");

        var version = ReadCmdOutput(".", gitversion, "/showvariable SemVer");
        var coverityToken = Environment.GetEnvironmentVariable("COVERITY_TOKEN");
        var coverityEmail = Environment.GetEnvironmentVariable("COVERITY_EMAIL");
        var repoCommitId = Environment.GetEnvironmentVariable("APPVEYOR_REPO_COMMIT");

        var coverityZipFile = coverityDirectory + "/coverity.zip";
        Cmd("7z", $"a -r {coverityZipFile} {coverityResultsDirectory}");

        using (var client = new HttpClient())
        {
            client.Timeout = TimeSpan.FromMinutes(20);

            var form = new MultipartFormDataContent();
            form.Add(new StringContent(coverityToken), @"""token""");
            form.Add(new StringContent(coverityEmail), @"""email""");
            form.Add(new StringContent(version), @"""version""");
            form.Add(new StringContent($"Build {version} ({repoCommitId})"), @"""description""");

            using (var fileStream = new FileStream(coverityZipFile, FileMode.Open, FileAccess.Read))
            {
                var formFileField = new StreamContent(fileStream);

                form.Add(formFileField, @"""file""", "coverity.zip");

                Console.WriteLine("Uploading coverity scan...");
                var postTask = client.PostAsync(coverityProjectUrl, form);
                try
                {
                    postTask.Wait();
                }
                catch (AggregateException e)
                {
                    throw e.InnerException;
                }
            }
        }
    });

targets.Add("clean", DependsOn("logsDirectory"), () => RunMsBuild("Clean"));

targets.Add(
    "restore",
    () => Cmd("dotnet", $"restore"));

targets.Add(
    "unit",
    DependsOn("build", "testsDirectory"),
    () => RunTests("unit"));

targets.Add(
    "integ",
    DependsOn("build", "testsDirectory"),
    () => RunTests("integ"));

targets.Add(
    "spec",
    DependsOn("build", "testsDirectory"),
    () => RunTests("spec"));

targets.Add(
    "approve",
    DependsOn("build", "testsDirectory"),
    () => RunTests("approve"));

targets.Add(
    "pack",
    DependsOn("build", "outputDirectory", "pdbgit"),
    () =>
    {
        var version = ReadCmdOutput(".", gitversion, "/showvariable NuGetVersionV2");
        foreach (var project in projectsToPack)
        {
            Cmd("dotnet", $"pack {project} --configuration Release --no-build --output {outputDirectory} /p:Version={version}");
        }

        Cmd(nuget, $"pack {analyzerMetaPackageNuspecPath} -Version {version} -OutputDirectory {outputDirectory} -NoPackageAnalysis");
    });

targets.Add(
    "pdbgit",
    DependsOn("build"),
    () =>
    {
        foreach (var pdb in pdbs)
        {
            Cmd(pdbGit, $"-u {repoUrl} -s {pdb}");
        }
    });

Run(Args, targets);

// helpers
public static void Cmd(string fileName, string args)
{
    Cmd(".", fileName, args);
}

public static void Cmd(string workingDirectory, string fileName, string args)
{
    using (var process = new Process())
    {
        process.StartInfo = new ProcessStartInfo
        {
            FileName = $"\"{fileName}\"",
            Arguments = args,
            WorkingDirectory = workingDirectory,
            UseShellExecute = false,
        };

        var workingDirectoryMessage = workingDirectory == "." ? "" : $" in '{process.StartInfo.WorkingDirectory}'";
        Console.WriteLine($"Running '{process.StartInfo.FileName} {process.StartInfo.Arguments}'{workingDirectoryMessage}...");
        process.Start();
        process.WaitForExit();
        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException($"The command exited with code {process.ExitCode}.");
        }
    }
}

public string ReadCmdOutput(string workingDirectory, string fileName, string args)
{
    using (var process = new Process())
    {
        process.StartInfo = new ProcessStartInfo
        {
            FileName = fileName,
            Arguments = args,
            WorkingDirectory = workingDirectory,
            UseShellExecute = false,
            RedirectStandardOutput = true
        };

        process.Start();
        process.WaitForExit();
        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException($"The {fileName} command exited with code {process.ExitCode}.");
        }

        return process.StandardOutput.ReadToEnd().Trim();
    }
}

public void RunMsBuild(string target)
{
    Cmd(
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
    Cmd(testDirectory, "dotnet", $"xunit -configuration Release -nologo -nobuild -noautoreporters -notrait \"explicit=yes\" -xml {xml}");
}

public string GetVSLocation()
{
    var installationPath = ReadCmdOutput(".", $"\"{vswhere}\"", "-nologo -latest -property installationPath -requires Microsoft.Component.MSBuild -version [15,16)");
    if (string.IsNullOrEmpty(installationPath))
    {
        throw new InvalidOperationException("Visual Studio 2017 was not found");
    }

    return installationPath;
}

public static string GetCurrentScriptDirectory([CallerFilePath] string path = null) => Path.GetDirectoryName(path);
