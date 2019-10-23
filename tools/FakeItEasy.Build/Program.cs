namespace FakeItEasy.Build
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using SimpleExec;
    using static Bullseye.Targets;
    using static SimpleExec.Command;

    public class Program
    {
        private static readonly Project[] ProjectsToPack =
        {
            "src/FakeItEasy/FakeItEasy.csproj",
            "src/FakeItEasy.Extensions.ValueTask/FakeItEasy.Extensions.ValueTask.csproj",
            "src/FakeItEasy.Analyzer.CSharp/FakeItEasy.Analyzer.CSharp.csproj",
            "src/FakeItEasy.Analyzer.VisualBasic/FakeItEasy.Analyzer.VisualBasic.csproj"
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
                DependsOn("build"),
                forEach: ProjectsToPack,
                action: project => Run("dotnet", $"pack {project.Path} --configuration Release --no-build --output {Path.GetFullPath("artifacts/output")}"));

            RunTargetsAndExit(args, messageOnly: ex => ex is NonZeroExitCodeException);
        }

        private class Project
        {
            public string Path { get; set; }

            public static implicit operator Project(string path) => new Project { Path = path };

            public override string ToString() => this.Path.Split('/').Last();
        }
    }
}
