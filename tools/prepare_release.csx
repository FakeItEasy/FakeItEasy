#r ".\packages\Octokit\lib\net45\Octokit.dll"

using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

using Octokit;
using Octokit.Helpers;

const string repoOwner = "FakeItEasy";
const string repoName = "FakeItEasy";
const string releaseFromBranchName = "develop";
const string targetBranchName = "master";
const string existingReleaseName = "vNext";
const string nextReleaseName = existingReleaseName;

var version = Args.FirstOrDefault();
if (version == null)
{
    throw new Exception("No version supplied");
}

var releaseBranchName = $"release/{version}";

static var gitHubClient = GetAuthenticatedGitHubClient();

var existingMilestone = await GetExistingMilestone();

var issuesInExistingMilestone = await GetIssuesInMilestone(existingMilestone);
var existingReleaseIssue = TakeExistingReleaseIssue(issuesInExistingMilestone);

var existingRelease = await GetExistingRelease();
var issuesReferencedFromRelease = GetIssuesReferencedFromRelease(existingRelease);

if (!CrossReferenceIssues(issuesInExistingMilestone, issuesReferencedFromRelease))
{
    return;
}

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
await UpdateIssue(existingReleaseIssue, existingMilestone, version);
await CreateNextIssue(existingReleaseIssue, nextMilestone);
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
    Console.WriteLine($"Fetching milestone '{existingReleaseName}'...");
    var milestoneRequest = new MilestoneRequest { State = ItemStateFilter.Open };
    var existingMilestone = (await gitHubClient.Issue.Milestone.GetAllForRepository(repoOwner, repoName, milestoneRequest))
        .Single(milestone => milestone.Title == existingReleaseName);
    Console.WriteLine($"Fetched milestone '{existingMilestone.Title}'");
    return existingMilestone;
}

public static async Task<Release> GetExistingRelease()
{
    Console.WriteLine($"Fetching GitHub release '{existingReleaseName}'...");
    var existingRelease = (await gitHubClient.Repository.Release.GetAll(repoOwner, repoName))
        .Single(release => release.Name == existingReleaseName && release.Draft == true);
    Console.WriteLine($"Fetched GitHub release '{existingRelease.Name}'");
    return existingRelease;
}

public static async Task<IList<Issue>> GetIssuesInMilestone(Milestone milestone)
{
    Console.WriteLine($"Fetching issues in milestone '{milestone.Title}'...'");
    var issueRequest = new RepositoryIssueRequest { Milestone = milestone.Number.ToString(), State = ItemStateFilter.All };
    var issues = (await gitHubClient.Issue.GetAllForRepository(repoOwner, repoName, issueRequest)).ToList();
    Console.WriteLine($"Fetched {issues.Count} issues in milestone '{milestone.Title}'");
    return issues;
}

public static IEnumerable<string> GetIssuesReferencedFromRelease(Release release)
{
    var issuesReferencedFromRelease = new HashSet<string>();
    foreach (Match match in Regex.Matches(release.Body, @"\(#(<issueNumber>[0-9]+)\)"))
    {
        issuesReferencedFromRelease.Add(match.Groups["issueNumber"].Value);
    }
    return issuesReferencedFromRelease;
}

public static Issue TakeExistingReleaseIssue(IList<Issue> issues)
{
    var issue = issues.Single(i => i.Title == $"Release {existingReleaseName}");
    Console.WriteLine($"Found release issue #{issue.Number}: '{issue.Title}'");
    issues.Remove(issue);
    return issue;
}

public static bool CrossReferenceIssues(IEnumerable<Issue> issuesInMilestone, IEnumerable<string> issueNumbersReferencedFromRelease)
{
    var issueNumbersInMilestone = issuesInMilestone.Select(i => i.Number.ToString());
    var issueNumbersInReleaseButNotMilestone = issueNumbersReferencedFromRelease.Except(issueNumbersInMilestone).ToList();
    var issuesInMilestoneButNotRelease = issuesInMilestone.Where(i => !issueNumbersReferencedFromRelease.Contains(i.Number.ToString())).ToList();

    if (!issuesInMilestoneButNotRelease.Any() && !issueNumbersInReleaseButNotMilestone.Any())
    {
        Console.WriteLine("The release refers to the same issues included in the milestone. Congratulations.");
        return true;
    }

    Console.WriteLine();

    if (issuesInMilestoneButNotRelease.Any())
    {
        Console.WriteLine("The following issues are linked to the milestone but not referenced in the release:");
        foreach (var issue in issuesInMilestoneButNotRelease)
        {
            Console.WriteLine($"  #{issue.Number}: {issue.Title}");
        }

        Console.WriteLine();
    }

    if (issueNumbersInReleaseButNotMilestone.Any())
    {
        Console.WriteLine("The following issues are referenced in the release but not linked to the milestone:");
        foreach (var issueNumber in issueNumbersInReleaseButNotMilestone)
        {
            Console.WriteLine($"  #{issueNumber}");
        }

        Console.WriteLine();
    }

    Console.WriteLine($"Prepare release anyhow? (y/N)");
    var response = Console.ReadLine().Trim();
    if (string.Equals(response, "y", StringComparison.InvariantCultureIgnoreCase))
    {
        return true;
    }
    else if (string.Equals(response, "n", StringComparison.InvariantCultureIgnoreCase))
    {
        return false;
    }
    else
    {
        Console.WriteLine($"Unknown response '{response}' received. Treating as 'n'.");
        return false;
    }
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
    var newMilestone = new NewMilestone(nextReleaseName);
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

    var newRelease = new NewRelease(nextReleaseName) { Draft = true, Name = nextReleaseName, Body = newReleaseBody.Trim() };
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
    var newIssue = new NewIssue($"Release {nextReleaseName}")
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
