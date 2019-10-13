namespace FakeItEasy.PrepareRelease
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using FakeItEasy.Tools;
    using Octokit;
    using static FakeItEasy.Tools.ReleaseHelpers;

    public static class Program
    {
        private const string RepoOwner = "FakeItEasy";
        private const string RepoName = "FakeItEasy";
        private const string ExistingReleaseName = "vNext";
        private const string NextReleaseName = ExistingReleaseName;

        public static async Task Main(string[] args)
        {
            var version = args.FirstOrDefault();
            if (version is null)
            {
                throw new Exception("No version supplied");
            }

            var gitHubClient = GetAuthenticatedGitHubClient();

            var existingMilestone = await gitHubClient.GetExistingMilestone();

            var issuesInExistingMilestone = await gitHubClient.GetIssuesInMilestone(existingMilestone);
            var existingReleaseIssue = GetExistingReleaseIssue(issuesInExistingMilestone);

            var allReleases = await gitHubClient.GetAllReleases();
            var existingRelease = allReleases.Single(release => release.Name == ExistingReleaseName && release.Draft);

            var releasesForExistingMilestone = GetReleasesForExistingMilestone(allReleases, existingRelease, version);

            var nonReleaseIssuesInMilestone = ExcludeReleaseIssues(issuesInExistingMilestone, releasesForExistingMilestone);

            var issueNumbersReferencedFromReleases = GetIssueNumbersReferencedFromReleases(releasesForExistingMilestone);

            if (!CrossReferenceIssues(nonReleaseIssuesInMilestone, issueNumbersReferencedFromReleases))
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
                await gitHubClient.RenameMilestone(existingMilestone, version);
                nextMilestone = await gitHubClient.CreateNextMilestone();
            }

            await gitHubClient.UpdateRelease(existingRelease, version);
            await gitHubClient.CreateNextRelease();
            await gitHubClient.UpdateIssue(existingReleaseIssue, existingMilestone, version);
            await gitHubClient.CreateNextIssue(existingReleaseIssue, nextMilestone);
        }

        private static List<Release> GetReleasesForExistingMilestone(IReadOnlyCollection<Release> allReleases, Release existingRelease, string version)
        {
            var releasesForExistingMilestone = new List<Release> { existingRelease };
            var versionRoot = IsPreRelease(version) ? version.Substring(0, version.IndexOf('-')) : version;
            releasesForExistingMilestone.AddRange(allReleases.Where(release => release.Name.StartsWith(versionRoot)));
            return releasesForExistingMilestone;
        }

        private static GitHubClient GetAuthenticatedGitHubClient()
        {
            var token = GitHubTokenSource.GetAccessToken();
            var credentials = new Credentials(token);
            return new GitHubClient(new ProductHeaderValue("FakeItEasy-build-scripts")) { Credentials = credentials };
        }

        private static async Task<Milestone> GetExistingMilestone(this IGitHubClient gitHubClient)
        {
            Console.WriteLine($"Fetching milestone '{ExistingReleaseName}'...");
            var milestoneRequest = new MilestoneRequest { State = ItemStateFilter.Open };
            var existingMilestone = (await gitHubClient.Issue.Milestone.GetAllForRepository(RepoOwner, RepoName, milestoneRequest))
                .Single(milestone => milestone.Title == ExistingReleaseName);
            Console.WriteLine($"Fetched milestone '{existingMilestone.Title}'");
            return existingMilestone;
        }

        private static async Task<IReadOnlyCollection<Release>> GetAllReleases(this IGitHubClient gitHubClient)
        {
            Console.WriteLine("Fetching all GitHub releases...");
            var allReleases = await gitHubClient.Repository.Release.GetAll(RepoOwner, RepoName);
            Console.WriteLine("Fetched all GitHub releases");
            return allReleases;
        }

        private static async Task<IList<Issue>> GetIssuesInMilestone(this IGitHubClient gitHubClient, Milestone milestone)
        {
            Console.WriteLine($"Fetching issues in milestone '{milestone.Title}'...'");
            var issueRequest = new RepositoryIssueRequest { Milestone = milestone.Number.ToString(), State = ItemStateFilter.All };
            var issues = (await gitHubClient.Issue.GetAllForRepository(RepoOwner, RepoName, issueRequest)).ToList();
            Console.WriteLine($"Fetched {issues.Count} issues in milestone '{milestone.Title}'");
            return issues;
        }

        private static Issue GetExistingReleaseIssue(IList<Issue> issues)
        {
            var issue = issues.Single(i => i.Title == $"Release {ExistingReleaseName}");
            Console.WriteLine($"Found release issue #{issue.Number}: '{issue.Title}'");
            return issue;
        }

        private static IList<Issue> ExcludeReleaseIssues(IList<Issue> issues, IEnumerable<Release> releases)
        {
            return issues.Where(issue => releases.All(release => $"Release {release.Name}" != issue.Title)).ToList();
        }

        private static bool CrossReferenceIssues(ICollection<Issue> issuesInMilestone, ICollection<int> issueNumbersReferencedFromRelease)
        {
            var issueNumbersInMilestone = issuesInMilestone.Select(i => i.Number);
            var issueNumbersInReleaseButNotMilestone = issueNumbersReferencedFromRelease.Except(issueNumbersInMilestone).ToList();
            var issuesInMilestoneButNotRelease = issuesInMilestone.Where(i => !issueNumbersReferencedFromRelease.Contains(i.Number)).ToList();

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

            Console.WriteLine("Prepare release anyhow? (y/N)");
            var response = Console.ReadLine().Trim();
            if (string.Equals(response, "y", StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }

            if (string.Equals(response, "n", StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            Console.WriteLine($"Unknown response '{response}' received. Treating as 'n'.");
            return false;
        }

        private static bool IsPreRelease(string version)
        {
            return version.Contains('-');
        }

        private static async Task RenameMilestone(this IGitHubClient gitHubClient, Milestone existingMilestone, string version)
        {
            var milestoneUpdate = new MilestoneUpdate { Title = version };
            Console.WriteLine($"Renaming milestone '{existingMilestone.Title}' to '{milestoneUpdate.Title}'...");
            var updatedMilestone = await gitHubClient.Issue.Milestone.Update(RepoOwner, RepoName, existingMilestone.Number, milestoneUpdate);
            Console.WriteLine($"Renamed milestone '{existingMilestone.Title}' to '{updatedMilestone.Title}'");
        }

        private static async Task<Milestone> CreateNextMilestone(this IGitHubClient gitHubClient)
        {
            var newMilestone = new NewMilestone(NextReleaseName);
            Console.WriteLine($"Creating new milestone '{newMilestone.Title}'...");
            var nextMilestone = await gitHubClient.Issue.Milestone.Create(RepoOwner, RepoName, newMilestone);
            Console.WriteLine($"Created new milestone '{nextMilestone.Title}'");
            return nextMilestone;
        }

        private static async Task UpdateRelease(this IGitHubClient gitHubClient, Release existingRelease, string version)
        {
            var releaseUpdate = new ReleaseUpdate { Name = version, TagName = version, Prerelease = IsPreRelease(version) };
            Console.WriteLine($"Renaming GitHub release '{existingRelease.Name}' to {releaseUpdate.Name}...");
            var updatedRelease = await gitHubClient.Repository.Release.Edit(RepoOwner, RepoName, existingRelease.Id, releaseUpdate);
            Console.WriteLine($"Renamed GitHub release '{existingRelease.Name}' to {updatedRelease.Name}");
        }

        private static async Task CreateNextRelease(this IGitHubClient gitHubClient)
        {
            const string newReleaseBody = @"
### Changed

### New
* Issue Title (#12345)

### Fixed

### Additional Items

### With special thanks for contributions to this release from:
* Real Name - @githubhandle
";

            var newRelease = new NewRelease(NextReleaseName) { Draft = true, Name = NextReleaseName, Body = newReleaseBody.Trim() };
            Console.WriteLine($"Creating new GitHub release '{newRelease.Name}'...");
            var nextRelease = await gitHubClient.Repository.Release.Create(RepoOwner, RepoName, newRelease);
            Console.WriteLine($"Created new GitHub release '{nextRelease.Name}'");
        }

        private static async Task UpdateIssue(this IGitHubClient gitHubClient, Issue existingIssue, Milestone existingMilestone, string version)
        {
            var issueUpdate = new IssueUpdate { Title = $"Release {version}", Milestone = existingMilestone.Number };
            Console.WriteLine($"Renaming release issue '{existingIssue.Title}' to '{issueUpdate.Title}'...");
            var updatedIssue = await gitHubClient.Issue.Update(RepoOwner, RepoName, existingIssue.Number, issueUpdate);
            Console.WriteLine($"Renamed release issue '{existingIssue.Title}' to '{updatedIssue.Title}'");
        }

        private static async Task CreateNextIssue(this IGitHubClient gitHubClient, Issue existingIssue, Milestone nextMilestone)
        {
            var newIssue = new NewIssue($"Release {NextReleaseName}")
            {
                Milestone = nextMilestone.Number,
                Body = existingIssue.Body.Replace("[x]", "[ ]"),
                Labels = { "build", "documentation" }
            };

            Console.WriteLine($"Creating new release issue '{newIssue.Title}'...");
            var nextIssue = await gitHubClient.Issue.Create(RepoOwner, RepoName, newIssue);
            Console.WriteLine($"Created new release issue #{nextIssue.Number}: '{newIssue.Title}'");
        }
    }
}
