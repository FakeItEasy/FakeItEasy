#r ".\packages\Octokit.0.26.0\lib\net45\Octokit.dll"

using System.Linq;

using Octokit;
using Octokit.Helpers;
using System.Runtime.CompilerServices;

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

static var gitHubClient = GetAuthenticatedGitHubClient();

var existingMilestone = await GetExistingMilestone();
var existingRelease = await GetExistingRelease();
var existingIssue = await GetExistingIssue(existingMilestone);
Milestone nextMilestone;
if (IsPreRelease(version))
{
    nextMilestone = existingMilestone;
}
else
{
    await RenameMilestone(existingMilestone, version);
    nextMilestone = await CreateNextMilestone();
}

await UpdateRelease(existingRelease, version);
await CreateNextRelease();
await UpdateIssue(existingIssue, existingMilestone, version);
await CreateNextIssue(existingIssue, nextMilestone);
var releaseFromBranch = await GetReleaseFromBranch();
await CreateReleaseBranch(releaseBranchName, releaseFromBranch);
await CreatePullRequest(version, releaseBranchName);

public static GitHubClient GetAuthenticatedGitHubClient()
{
    var token = ReadGitHubToken();
    var credentials = new Credentials(token);
    return new GitHubClient(new ProductHeaderValue("FakeItEasy-build-scripts")) { Credentials = credentials };
}

public static string ReadGitHubToken()
{
    return File.ReadAllText(Path.Combine(GetCurrentScriptDirectory(), ".githubtoken")).Trim();
}

public static async Task<Milestone> GetExistingMilestone()
{
    Console.WriteLine("Fetching milestone 'vNext'...");
    var milestoneRequest = new MilestoneRequest { State = ItemStateFilter.Open };
    var existingMilestone = (await gitHubClient.Issue.Milestone.GetAllForRepository(repoOwner, repoName, milestoneRequest))
        .Single(milestone => milestone.Title == "vNext");
    Console.WriteLine($"Fetched milestone '{existingMilestone.Title}'");
    return existingMilestone;
}

public static async Task<Release> GetExistingRelease()
{
    Console.WriteLine("Fetching GitHub release 'vNext'...");
    var existingRelease = (await gitHubClient.Repository.Release.GetAll(repoOwner, repoName))
        .Single(release => release.Name == "vNext" && release.Draft == true);
    Console.WriteLine($"Fetched GitHub release '{existingRelease.Name}'");
    return existingRelease;
}

public static async Task<Issue> GetExistingIssue(Milestone existingMilestone)
{
    Console.WriteLine("Fetching release issue 'Release vNext'...");
    var issueRequest = new RepositoryIssueRequest { Milestone = existingMilestone.Number.ToString(), State = ItemStateFilter.Open };
    var existingIssue = (await gitHubClient.Issue.GetAllForRepository(repoOwner, repoName, issueRequest))
        .Single(issue => issue.Title == "Release vNext");
    Console.WriteLine($"Fetched release issue #{existingIssue.Number}: '{existingIssue.Title}'");
    return existingIssue;
}

public static bool IsPreRelease(string version)
{
    return version.Contains('-');
}

public static async Task RenameMilestone(Milestone existingMilestone, string version)
{
    var milestoneUpdate = new MilestoneUpdate { Title = version };
    Console.WriteLine($"Renaming milestone '{existingMilestone.Title}' to '{milestoneUpdate.Title}'...");
    var updatedMilestone = await gitHubClient.Issue.Milestone.Update(repoOwner, repoName, existingMilestone.Number, milestoneUpdate);
    Console.WriteLine($"Renamed milestone '{existingMilestone.Title}' to '{updatedMilestone.Title}'");
}

public static async Task<Milestone> CreateNextMilestone()
{
    var newMilestone = new NewMilestone("vNext");
    Console.WriteLine($"Creating new milestone '{newMilestone.Title}'...");
    var nextMilestone = (await gitHubClient.Issue.Milestone.Create(repoOwner, repoName, newMilestone));
    Console.WriteLine($"Created new milestone '{nextMilestone.Title}'");
    return nextMilestone;
}

public static async Task UpdateRelease(Release existingRelease, string version)
{
    var releaseUpdate = new ReleaseUpdate { Name = version };
    Console.WriteLine($"Renaming GitHub release '{existingRelease.Name}' to {releaseUpdate.Name}...");
    var updatedRelease = await gitHubClient.Repository.Release.Edit(repoOwner, repoName, existingRelease.Id, releaseUpdate);
    Console.WriteLine($"Renamed GitHub release '{existingRelease.Name}' to {updatedRelease.Name}");
}

public static async Task CreateNextRelease()
{
    const string newReleaseBody = @"
### Changed

### New
* Issue Title (#12345)

### Fixed

### Additional Items

### With special thanks for contributions to this release from:
* Real Name - @githubhandle";

    var newRelease = new NewRelease("vNext") { Draft = true, Name = "vNext", Body = newReleaseBody.Trim() };
    Console.WriteLine($"Creating new GitHub release '{newRelease.Name}'...");
    var nextRelease = await gitHubClient.Repository.Release.Create(repoOwner, repoName, newRelease);
    Console.WriteLine($"Created new GitHub release '{nextRelease.Name}'");
}

public static async Task UpdateIssue(Issue existingIssue, Milestone existingMilestone, string version)
{
    var issueUpdate = new IssueUpdate { Title = $"Release {version}", Milestone = existingMilestone.Number };
    Console.WriteLine($"Renaming release issue '{existingIssue.Title}' to '{issueUpdate.Title}'...");
    var updatedIssue = await gitHubClient.Issue.Update(repoOwner, repoName, existingIssue.Number, issueUpdate);
    Console.WriteLine($"Renamed release issue '{existingIssue.Title}' to '{updatedIssue.Title}'");
}

public static async Task CreateNextIssue(Issue existingIssue, Milestone nextMilestone)
{
    var newIssue = new NewIssue("Release vNext")
    {
        Milestone = nextMilestone.Number,
        Body = existingIssue.Body.Replace("[x]", "[ ]"),
        Labels = { "build", "documentation" }
    };

    Console.WriteLine($"Creating new release issue '{newIssue.Title}'...");
    var nextIssue = await gitHubClient.Issue.Create(repoOwner, repoName, newIssue);
    Console.WriteLine($"Created new release issue #{nextIssue.Number}: '{newIssue.Title}'");
}

public static async Task<Reference> GetReleaseFromBranch()
{
    Console.WriteLine($"Fetching reference to branch {releaseFromBranchName}...");
    var releaseFromBranch = await gitHubClient.Git.Reference.Get(repoOwner, repoName, $"heads/{releaseFromBranchName}");
    Console.WriteLine($"Fetched reference to branch {releaseFromBranch.Url}");
    return releaseFromBranch;
}

public static async Task CreateReleaseBranch(string releaseBranchName, Reference releaseFromBranch)
{
    Console.WriteLine($"Creating branch {releaseBranchName} from {releaseFromBranchName}...");
    var releaseBranch = await gitHubClient.Git.Reference.CreateBranch(repoOwner, repoName, releaseBranchName, releaseFromBranch);
    Console.WriteLine($"Created branch at {releaseBranch.Url}");
}

public static async Task CreatePullRequest(string version, string releaseBranchName)
{
    Console.WriteLine($"Creating pull request to merge {releaseBranchName} into {targetBranchName}...");
    var pr = await gitHubClient.PullRequest.Create(
        repoOwner,
        repoName,
        new NewPullRequest($"Release {version}", releaseBranchName, targetBranchName));
    Console.WriteLine($"Created pull request '{pr.Title}' at {pr.HtmlUrl}");
}

public static string GetCurrentScriptDirectory([CallerFilePath] string path = null) => Path.GetDirectoryName(path);
