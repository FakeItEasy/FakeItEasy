#r ".\packages\Octokit.0.26.0\lib\net45\Octokit.dll"
#r ".\packages\system.runtime.interopservices.runtimeinformation\4.0.0\lib\net45\System.Runtime.InteropServices.RuntimeInformation.dll"

using Octokit;
using Octokit.Helpers;

const string repoOwner = "FakeItEasy";
const string repoName = "FakeItEasy";
const string releaseFromBranchName = "develop";
const string targetBranchName = "master";

var version = Args.FirstOrDefault();
if (version == null)
{
    throw new Exception("No version supplied");
}

var releaseBranchName = $"release/{version}";

var token = ReadGitHubToken();
var credentials = new Credentials(token);

var gitHubClient = new GitHubClient(new ProductHeaderValue("FakeItEasy-build-scripts")) { Credentials = credentials };
var referenceClient = gitHubClient.Git.Reference;

Console.WriteLine($"Fetching reference to branch {releaseFromBranchName}...");
var releaseFromBranch = await referenceClient.Get(repoOwner, repoName, $"heads/{releaseFromBranchName}");
Console.WriteLine($"Fetched reference to branch {releaseFromBranch.Url}");

Console.WriteLine($"Creating branch {releaseBranchName} from {releaseFromBranchName}...");
var releaseBranch = await referenceClient.CreateBranch(repoOwner, repoName, releaseBranchName, releaseFromBranch);
Console.WriteLine($"Created branch at {releaseBranch.Url}");

Console.WriteLine($"Creating pull request to merge {releaseBranchName} into {targetBranchName}...");
var pr = await gitHubClient.PullRequest.Create(
    repoOwner,
    repoName,
    new NewPullRequest($"Release {version}", releaseBranchName, targetBranchName));
Console.WriteLine($"Created pull request '{pr.Title}' at {pr.HtmlUrl}");

public static string ReadGitHubToken()
{
    return File.ReadAllText(".githubtoken").Trim();
}