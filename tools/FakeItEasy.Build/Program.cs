using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using SimpleExec;
using static Bullseye.Targets;
using static SimpleExec.Command;

Project[] projectsToPack =
{
    "src/FakeItEasy/FakeItEasy.csproj",
    "src/FakeItEasy.Extensions.ValueTask/FakeItEasy.Extensions.ValueTask.csproj"
};

var testSuites = new Dictionary<string, string[]>
{
    ["unit"] = new[]
    {
        "tests/FakeItEasy.Tests"
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
    ["recipes"] = new[]
    {
        "recipes/FakeItEasy.Recipes.CSharp"
    },
    ["approve"] = new[]
    {
        "tests/FakeItEasy.Tests.Approval"
    }
};

Target("default", DependsOn("unit", "integ", "spec", "recipes", "approve", "pack"));

Target(
    "build",
    () => Run("dotnet", "build FakeItEasy.sln -c Release /maxcpucount /nr:false /verbosity:minimal /nologo /bl:artifacts/logs/build.binlog"));

foreach (var testSuite in testSuites)
{
    Target(
        testSuite.Key,
        DependsOn("build"),
        forEach: testSuite.Value,
        action: testDirectory => Run("dotnet", "test --configuration Release --no-build --nologo -- RunConfiguration.NoAutoReporters=true", testDirectory));
}

Target(
    "pack",
    DependsOn("build"),
    forEach: projectsToPack,
    action: project => Run("dotnet", $"pack {project.Path} --configuration Release --no-build --nologo --output \"{Path.GetFullPath("artifacts/output")}\""));

Target("docs", DependsOn("check-docs-links", "check-readme-links"));

// If mkdocs's site_url configuration is not set, sitemap.xml is full of "None" links.
// Even with a valid site_url, the link check would fail if we added a new page.
// So trust that any non-None links that mkdocs puts in sitemap.xml will be valid.
// The 404.html page is likewise generated, and isn't even the one we use on the live site (mike generates a new one).
// We use a custom User Agent because Github doesn't like the default (LinkChecker/X.Y) and returns a 404.
Target(
    "check-docs-links",
    DependsOn("generate-docs"),
    () => Run(
        "uv",
        "run linkchecker --config=.linkcheckerrc --ignore-url=sitemap.xml --ignore-url=404.html --check-extern --user-agent=FakeItEasyDocs/1.0 -F html/utf-8/../artifacts/docs-link-check.html ../artifacts/docs/index.html",
        "docs"));

Target(
    "generate-docs",
    DependsOn("create-python-version-file"),
    () => Run("uv", "run mkdocs build --clean --site-dir ../artifacts/docs --config-file mkdocs.yml --strict", "docs"));

Target(
    "check-readme-links",
    () => Run(
        "uv",
        "run linkchecker --config=.linkcheckerrc --check-extern --user-agent=FakeItEasyDocs/1.0 -F html/utf-8/../artifacts/readme-link-check.html ../README.md",
        "docs"));

// uv really likes there to be a .python-version file to specify which version of python it should use.
// Rather than maintain a duplicate of the version in pyproject.toml, we'll generate the .python-version file from the latter.
Target(
    "create-python-version-file",
    () =>
    {
        // expect the line to look something like
        // requires-python = "~=x.yz"
        // . We want the x.yz part.
        const string versionSourceFile = "docs/pyproject.toml";
        const string versionPattern = @"requires-python = ""~=(?<version>[^""]+)""";
        var pythonVersion = File.ReadLines(versionSourceFile)
            .Select(line => Regex.Match(line, versionPattern))
            .Where(m => m.Success)
            .Select(m => m.Groups["version"].Value)
            .FirstOrDefault() ?? throw new InvalidOperationException($"Could not find required Python version in {versionSourceFile}");
        File.WriteAllText("docs/.python-version", pythonVersion);
    });

Target(
    "force-approve",
    () =>
    {
        foreach (var received in Directory.EnumerateFiles("tests/FakeItEasy.Tests.Approval/ApprovedApi", "*.received.txt", SearchOption.AllDirectories))
        {
            File.Copy(received, received.Replace(".received.txt", ".verified.txt", StringComparison.OrdinalIgnoreCase), overwrite: true);
        }
    });

Target(
    "initialize-user-properties",
    () =>
    {
        if (!File.Exists("FakeItEasy.user.props"))
        {
            var defaultUserProps = @"
<Project>
<PropertyGroup>
<BuildProfile></BuildProfile>
</PropertyGroup>
</Project>".Trim();
            File.WriteAllText("FakeItEasy.user.props", defaultUserProps, Encoding.UTF8);
        }
    });

foreach (var profile in Directory.EnumerateFiles("profiles", "*.props").Select(Path.GetFileNameWithoutExtension))
{
    Target(
        "use-profile-" + profile,
        DependsOn("initialize-user-properties"),
        () =>
        {
            var xmlDoc = XDocument.Load("FakeItEasy.user.props");

            var buildProfileElement = xmlDoc.Root!.Elements("PropertyGroup").Elements("BuildProfile").FirstOrDefault();
            if (buildProfileElement is null)
            {
                var propertyGroupElement = xmlDoc.Root.Element("PropertyGroup");
                if (propertyGroupElement is null)
                {
                    propertyGroupElement = new XElement("PropertyGroup");
                    xmlDoc.Root.Add(propertyGroupElement);
                }

                buildProfileElement = new XElement("BuildProfile");
                propertyGroupElement.Add(buildProfileElement);
            }

            if (buildProfileElement.Value != profile)
            {
                buildProfileElement.Value = profile!;
                xmlDoc.Save("FakeItEasy.user.props");
            }
        });
}

RunTargetsAndExit(args, messageOnly: ex => ex is NonZeroExitCodeException);

file sealed record Project(string Path)
{
    public static implicit operator Project(string path) => new Project(path);

    public override string ToString() => this.Path.Split('/').Last();
}
