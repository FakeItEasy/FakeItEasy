#load "packages/simple-targets-csx.5.2.0/simple-targets.csx"

#r "System.Net.Http"

using System.Net.Http;
using static SimpleTargets;

var solutionName = "FakeItEasy";

var solution = "./" + solutionName + ".sln";
var packagesDirectory = Path.GetFullPath("packages");
var repoUrl = "https://github.com/FakeItEasy/FakeItEasy";
var coverityProjectUrl = "https://scan.coverity.com/builds?project=FakeItEasy%2FFakeItEasy";
var versionAssembly = "./src/FakeItEasy/bin/Release/net40/FakeItEasy.dll";
var nuspecs = Directory.GetFiles("./src", "*.nuspec");
var pdbs = new []
{
    "src/FakeItEasy/bin/Release/net40/FakeItEasy.pdb",
    "src/FakeItEasy/bin/Release/netstandard1.6/FakeItEasy.pdb",
    "src/FakeItEasy.Analyzer/bin/Release/netstandard1.1/FakeItEasy.Analyzer.Csharp.pdb",
    "src/FakeItEasy.Analyzer/bin/Release/netstandard1.1/FakeItEasy.Analyzer.VisualBasic.pdb"
};

var testSuites = new Dictionary<string, TestSuite[]>
{
    ["unit"] = new TestSuite[]
    {
        new DotnetTestSuite("tests/FakeItEasy.Tests"),
        new DotnetTestSuite("tests/FakeItEasy.Analyzer.CSharp.Tests"),
        new DotnetTestSuite("tests/FakeItEasy.Analyzer.VisualBasic.Tests"),
    },
    ["integ"] = new TestSuite[]
    {
        new DotnetTestSuite("tests/FakeItEasy.IntegrationTests"),
        new ClassicTestSuite("tests/FakeItEasy.IntegrationTests.VB/bin/Release/FakeItEasy.IntegrationTests.VB.dll"),
    },
    ["spec"] = new TestSuite[]
    {
        new DotnetTestSuite("tests/FakeItEasy.Specs")
    },
    ["approve"] = new TestSuite[]
    {
        new DotnetTestSuite("tests/FakeItEasy.Tests.Approval")
    }
};

// tool locations
var vswhere = "./packages/vswhere.1.0.62/tools/vswhere.exe";
var msBuild = $"{GetVSLocation()}/MSBuild/15.0/Bin/MSBuild.exe";
var nuget = "./.nuget/NuGet.exe";
var pdbGit = "./packages/pdbGit.3.0.41/tools/PdbGit.exe";
static var xunit = "./packages/xunit.runner.console.2.0.0/tools/xunit.console.exe";

// artifact locations
var coverityDirectory = "./artifacts/coverity";
var coverityResultsDirectory = "./artifacts/coverity/cov-int";
var logsDirectory = "./artifacts/logs";
var outputDirectory = "./artifacts/output";
static var testsDirectory = "./artifacts/tests";

// targets
var targets = new TargetDictionary();

targets.Add("default", DependsOn("pdbgit", "unit", "integ", "spec", "approve", "pack"));

targets.Add("outputDirectory", () => Directory.CreateDirectory(outputDirectory));

targets.Add("coverityDirectory", () => Directory.CreateDirectory(coverityDirectory));

targets.Add("logsDirectory", () => Directory.CreateDirectory(logsDirectory));

targets.Add("testsDirectory", () => Directory.CreateDirectory(testsDirectory));

targets.Add("build", DependsOn("clean", "outputDirectory", "restore"), () => RunMsBuild("Build"));

targets.Add(
    "coverity",
    DependsOn("clean", "coverityDirectory", "restore"),
    () =>
    {
        var packagesDirectoryOption = $"/p:NuGetPackagesDirectory={packagesDirectory}";
        Cmd(
            "cov-build",
            $@"--dir {coverityResultsDirectory} ""{msBuild}"" {solution} /target:Build /p:configuration=Release /nr:false /verbosity:minimal /nologo /fl /flp:LogFile=artifacts/logs/Coverity-Build.log;Verbosity=Detailed;PerformanceSummary {packagesDirectoryOption}");

        var version = GetVersion();
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
    () =>
    {
        Cmd(nuget, $"restore {solution} -PackagesDirectory {packagesDirectory}");
        Cmd("dotnet", $"restore");
    });

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
    DependsOn("build", "outputDirectory"),
    () =>
    {
        var version = GetVersion();
        foreach (var nuspec in nuspecs)
        {
            Cmd(nuget, $"pack {nuspec} -Version {version} -OutputDirectory {outputDirectory} -NoPackageAnalysis");
        }
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

public void RunMsBuild(string target)
{
    var packagesDirectoryOption = string.IsNullOrEmpty(packagesDirectory) ? "" : $"/p:NuGetPackagesDirectory={packagesDirectory}";
    Cmd(
        msBuild,
        $"{solution} /target:{target} /p:configuration=Release /nr:false /verbosity:minimal /nologo /fl /flp:LogFile=artifacts/logs/{target}.log;Verbosity=Detailed;PerformanceSummary {packagesDirectoryOption}");
}

public void RunTests(string target)
{
    foreach (var testSuite in testSuites[target])
    {
        testSuite.Execute();
    }
}

public string GetVersion()
{
    var  versionInfo = FileVersionInfo.GetVersionInfo(versionAssembly);
    var version = versionInfo.ProductVersion;

    var versionSuffix = Environment.GetEnvironmentVariable("VERSION_SUFFIX");

    if (string.IsNullOrEmpty(versionSuffix))
    {
        return version;
    }

    var build = Environment.GetEnvironmentVariable("BUILD");
    var buildSuffix = versionSuffix + "-build" + build.PadLeft(6, '0');
    return version + buildSuffix;
}

public string GetVSLocation()
{
    using (var process = new Process())
    {
        process.StartInfo = new ProcessStartInfo
        {
            FileName = $"\"{vswhere}\"",
            Arguments = "-nologo -latest -property installationPath -requires Microsoft.Component.MSBuild -version [15,16)",
            WorkingDirectory = ".",
            UseShellExecute = false,
            RedirectStandardOutput = true
        };

        process.Start();
        process.WaitForExit();
        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException($"The vswhere command exited with code {process.ExitCode}.");
        }
        else
        {
            string installationPath = process.StandardOutput.ReadToEnd().Trim();
            if (string.IsNullOrEmpty(installationPath))
            {
                throw new InvalidOperationException("Visual Studio 2017 was not found");
            }
            return installationPath;
        }
    }
}

abstract class TestSuite
{
    public abstract void Execute();
}

class DotnetTestSuite : TestSuite
{
    public DotnetTestSuite(string testDirectory)
    {
        this.TestDirectory = testDirectory;
    }

    public string TestDirectory { get; }

    public override void Execute()
    {
        var xml = Path.GetFullPath(Path.Combine(testsDirectory, Path.GetFileName(this.TestDirectory) + ".TestResults.xml"));
        Cmd(this.TestDirectory, "dotnet", $"xunit -configuration Release -nologo -notrait \"explicit=yes\" -xml {xml} -nobuild");
    }
}

class ClassicTestSuite : TestSuite
{
    public ClassicTestSuite(string assemblyPath)
    {
        this.AssemblyPath = assemblyPath;
    }

    public string AssemblyPath { get; }

    public override void Execute()
    {
        var baseFileName = Path.GetFullPath(Path.Combine(testsDirectory, Path.GetFileNameWithoutExtension(this.AssemblyPath))) + ".TestResults";
        var xml = baseFileName + ".xml";
        var html = baseFileName + ".html";
        Cmd(xunit, $"{this.AssemblyPath} -noshadow -nologo -notrait \"explicit=yes\"' -xml {xml} -html {html}");
    }
}