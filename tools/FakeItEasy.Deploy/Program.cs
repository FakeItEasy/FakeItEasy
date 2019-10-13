namespace FakeItEasy.Deploy
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using FakeItEasy.Tools;
    using Octokit;
    using static FakeItEasy.Tools.ReleaseHelpers;
    using static SimpleExec.Command;

    public class Program
    {
        public static async Task Main(string[] args)
        {
            var releaseName = GetAppVeyorTagName();
            if (string.IsNullOrEmpty(releaseName))
            {
                Console.WriteLine("No Appveyor tag name supplied. Not deploying.");
                return;
            }

            string nugetServerUrl = GetNuGetServerUrl();
            string nugetApiKey = GetNuGetApiKey();
            if (string.IsNullOrEmpty(nugetServerUrl))
            {
                throw new Exception("NuGet server URL is not set; please set it in the NUGET_SERVER_URL environment variable.");
            }

            if (string.IsNullOrEmpty(nugetApiKey))
            {
                throw new Exception("NuGet API key is not set; please set it in the NUGET_API_KEY environment variable.");
            }

            var (repoOwner, repoName) = GetRepositoryName();
            var gitHubClient = GetAuthenticatedGitHubClient();

            Console.WriteLine($"Deploying {releaseName}");
            Console.WriteLine($"Looking for GitHub release {releaseName}");

            var releases = await gitHubClient.Repository.Release.GetAll(repoOwner, repoName);
            var release = releases.FirstOrDefault(r => r.Name == releaseName)
                ?? throw new Exception($"Can't find release {releaseName}");

            var artifactsFolder = Path.GetFullPath(Path.Combine(GetCurrentScriptDirectory(), "../../artifacts/output/"));
            var artifactsPattern = "*.nupkg";

            var artifacts = Directory.GetFiles(artifactsFolder, artifactsPattern);
            if (!artifacts.Any())
            {
                throw new Exception("Can't find any artifacts to publish");
            }

            Console.WriteLine($"Uploading artifacts to GitHub release {releaseName}");
            foreach (var file in artifacts)
            {
                await UploadArtifactToGitHubReleaseAsync(gitHubClient, release, file);
            }

            Console.WriteLine($"Pushing nupkgs to {nugetServerUrl}");
            foreach (var file in artifacts)
            {
                await UploadPackageToNuGetAsync(file, nugetServerUrl, nugetApiKey);
            }

            var issueNumbersInCurrentRelease = GetIssueNumbersReferencedFromReleases(new[] { release });
            var preReleases = GetPreReleasesContributingToThisRelease(release, releases);
            var issueNumbersInPreReleases = GetIssueNumbersReferencedFromReleases(preReleases);
            var newIssueNumbers = issueNumbersInCurrentRelease.Except(issueNumbersInPreReleases);

            Console.WriteLine($"Adding 'released as part of' notes to {newIssueNumbers.Count()} issues");
            var commentText = $"This change has been released as part of [{repoName} {releaseName}](https://github.com/{repoOwner}/{repoName}/releases/tag/{releaseName}).";
            await Task.WhenAll(newIssueNumbers.Select(n => gitHubClient.Issue.Comment.Create(repoOwner, repoName, n, commentText)));

            Console.WriteLine("Finished deploying");
        }

        private static IEnumerable<Release> GetPreReleasesContributingToThisRelease(Release release, IReadOnlyList<Release> releases)
        {
            if (release.Prerelease)
            {
                return Enumerable.Empty<Release>();
            }

            string baseName = BaseName(release);
            return releases.Where(r => r.Prerelease && BaseName(r) == baseName);

            string BaseName(Release release) => release.Name.Split('-')[0];
        }

        private static async Task UploadArtifactToGitHubReleaseAsync(GitHubClient client, Release release, string path)
        {
            var name = Path.GetFileName(path);
            Console.WriteLine($"Uploading {name}");
            using (var stream = File.OpenRead(path))
            {
                var upload = new ReleaseAssetUpload
                {
                    FileName = name,
                    ContentType = "application/octet-stream",
                    RawData = stream,
                    Timeout = TimeSpan.FromSeconds(100)
                };

                var asset = await client.Repository.Release.UploadAsset(release, upload);
                Console.WriteLine($"Uploaded {asset.Name}");
            }
        }

        private static async Task UploadPackageToNuGetAsync(string path, string nugetServerUrl, string nugetApiKey)
        {
            string name = Path.GetFileName(path);
            Console.WriteLine($"Pushing {name}");
            await RunAsync(ToolPaths.NuGet, $"push \"{path}\" -ApiKey {nugetApiKey} -Source {nugetServerUrl} -NonInteractive -ForceEnglishOutput", noEcho: true);
            Console.WriteLine($"Pushed {name}");
        }

        private static (string repoOwner, string repoName) GetRepositoryName()
        {
            var repoNameWithOwner = Environment.GetEnvironmentVariable("APPVEYOR_REPO_NAME");
            var parts = repoNameWithOwner.Split('/');
            return (parts[0], parts[1]);
        }

        private static GitHubClient GetAuthenticatedGitHubClient()
        {
            var token = GitHubTokenSource.GetAccessToken();
            var credentials = new Credentials(token);
            return new GitHubClient(new ProductHeaderValue("FakeItEasy-build-scripts")) { Credentials = credentials };
        }

        private static string GetAppVeyorTagName() => Environment.GetEnvironmentVariable("APPVEYOR_REPO_TAG_NAME");

        private static string GetNuGetServerUrl() => Environment.GetEnvironmentVariable("NUGET_SERVER_URL");

        private static string GetNuGetApiKey() => Environment.GetEnvironmentVariable("NUGET_API_KEY");

        private static string GetCurrentScriptDirectory([CallerFilePath] string path = null) => Path.GetDirectoryName(path);
    }
}
