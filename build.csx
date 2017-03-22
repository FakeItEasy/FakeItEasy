#load "packages/simple-targets-csx.5.2.0/simple-targets.csx"

#r "System.Net.Http"

using System.Net.Http;
using static SimpleTargets;

var solutionName = "FakeItEasy";

var solution = "./" + solutionName + ".sln";
var packagesDirectory = Path.GetFullPath("packages");
var repoUrl = "https://github.com/FakeItEasy/FakeItEasy";
var coverityProjectUrl = "https://scan.coverity.com/builds?project=FakeItEasy%2FFakeItEasy";
var versionAssembly = "./src/FakeItEasy/bin/Release/FakeItEasy.dll";
var nuspecs = Directory.GetFiles("./src", "*.nuspec");
var gitlinks = Directory.GetDirectories("./src").Where(p => !p.EndsWith(".Shared")).Select(Path.GetFileName);

var unitTests = new []
{
    "tests/FakeItEasy.Tests/bin/Release/FakeItEasy.Tests.dll",
    "tests/FakeItEasy.Analyzer.CSharp.Tests/bin/Release/FakeItEasy.Analyzer.CSharp.Tests.dll",
    "tests/FakeItEasy.Analyzer.VisualBasic.Tests/bin/Release/FakeItEasy.Analyzer.VisualBasic.Tests.dll"
};

var netstdUnitTestDirectories = new []
{
    "tests/FakeItEasy.Tests.netstd"
};

var integrationTests = new []
{
    "tests/FakeItEasy.IntegrationTests/bin/Release/FakeItEasy.IntegrationTests.dll",
    "tests/FakeItEasy.IntegrationTests.VB/bin/Release/FakeItEasy.IntegrationTests.VB.dll"
};

var netstdIntegrationTestDirectories = new []
{
    "tests/FakeItEasy.IntegrationTests.netstd"
};

var specs = new []
{
    "tests/FakeItEasy.Specs/bin/Release/FakeItEasy.Specs.dll"
};

var netstdSpecDirectories = new []
{
    "tests/FakeItEasy.Specs.netstd"
};

var approvalTests = new []
{
    "tests/FakeItEasy.Tests.Approval/bin/Release/FakeItEasy.Tests.Approval.dll",
    "tests/FakeItEasy.Tests.Approval.netstd/bin/Release/FakeItEasy.Tests.Approval.netstd.dll"
};

// tool locations
var msBuild = $"{Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86)}/MSBuild/14.0/Bin/msbuild.exe";
var nuget = "./.nuget/NuGet.exe";
var gitlink = "./packages/gitlink.2.3.0/lib/net45/GitLink.exe";
var xunit = "./packages/xunit.runner.console.2.0.0/tools/xunit.console.exe";

// artifact locations
var coverityDirectory = "./artifacts/coverity";
var coverityResultsDirectory = "./artifacts/coverity/cov-int";
var logsDirectory = "./artifacts/logs";
var outputDirectory = "./artifacts/output";
var testsDirectory = "./artifacts/tests";

// targets
var targets = new TargetDictionary();

targets.Add("default", DependsOn("gitlink", "unit", "integ", "spec", "approve", "pack"));

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
        foreach (var projectDir in netstdUnitTestDirectories.Concat(netstdIntegrationTestDirectories).Concat(netstdSpecDirectories))
        {
            Cmd(projectDir, "dotnet", "restore");
        }
    });

targets.Add(
    "unit",
    DependsOn("build", "testsDirectory"),
    () =>
    {
        RunTests(unitTests);
        RunDotNetTests(netstdUnitTestDirectories);
    });

targets.Add(
    "integ",
    DependsOn("build", "testsDirectory"),
    () =>
    {
        RunTests(integrationTests);
        RunDotNetTests(netstdIntegrationTestDirectories);
    });

targets.Add(
    "spec",
    DependsOn("build", "testsDirectory"),
    () =>
    {
        RunTests(specs);
        RunDotNetTests(netstdSpecDirectories);
    });

targets.Add(
    "approve",
    DependsOn("build", "testsDirectory"),
    () => RunTests(approvalTests));

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
    "gitlink",
    DependsOn("build"), () => Cmd(gitlink, $". -f {solution} -u {repoUrl} -include " + string.Join(",", gitlinks)));

Run(Args, targets);

// helpers
public void Cmd(string fileName, string args)
{
    Cmd(".", fileName, args);
}

public void Cmd(string workingDirectory, string fileName, string args)
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

public void RunTests(IEnumerable<string> testDlls)
{
    foreach (var testDll in testDlls)
    {
        var baseFileName = Path.GetFullPath(Path.Combine(testsDirectory, Path.GetFileNameWithoutExtension(testDll))) + ".TestResults";
        var xml = baseFileName + ".xml";
        var html = baseFileName + ".html";
        Cmd(xunit, $"{testDll} -noshadow -nologo -notrait \"explicit=yes\"' -xml {xml} -html {html}");
    }
}

public void RunDotNetTests(IEnumerable<string> testDirectories)
{
    foreach (var testDirectory in testDirectories)
    {
        var xml = Path.GetFullPath(Path.Combine(testsDirectory, Path.GetFileName(testDirectory) + ".TestResults.xml"));
        Cmd(testDirectory, "dotnet", $"test -c Release -nologo -notrait \"explicit=yes\" -xml {xml}");
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
