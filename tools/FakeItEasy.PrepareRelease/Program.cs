namespace FakeItEasy.PrepareRelease
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using FakeItEasy.Tools;
    using Octokit;
    using static FakeItEasy.Tools.ReleaseHelpers;

    public static class Program
    {
        private const string RepoOwner = "FakeItEasy";
        private const string RepoName = "FakeItEasy";

        public static async Task Main(string[] args)
        {
            if (args.Length != 3 || (args[0] != "next" && args[0] != "fork"))
            {
                Console.WriteLine("Illegal arguments. Must be one of the following:");
                Console.WriteLine("  next <new release> <existing release>");
                Console.WriteLine("  fork <new release> <existing release>");
                return;
            }

            var action = args[0];
            var version = args[1];
            var existingReleaseName = args[2];

            var gitHubClient = GetAuthenticatedGitHubClient();
            var existingMilestone = await gitHubClient.GetExistingMilestone(existingReleaseName);
            var issuesInExistingMilestone = await gitHubClient.GetIssuesInMilestone(existingMilestone);
            var existingReleaseIssue = GetExistingReleaseIssue(issuesInExistingMilestone, existingReleaseName);

            if (action == "next")
            {
                var nextReleaseName = existingReleaseName;

                var allReleases = await gitHubClient.GetAllReleases();
                var existingRelease = allReleases.Single(release => release.Name == existingReleaseName && release.Draft);

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
                    nextMilestone = await gitHubClient.CreateNextMilestone(nextReleaseName);
                }

                await gitHubClient.UpdateRelease(existingRelease, version);
                await gitHubClient.CreateNextRelease(nextReleaseName);
                await gitHubClient.UpdateIssue(existingReleaseIssue, existingMilestone, version);
                await gitHubClient.CreateNextIssue(existingReleaseIssue, nextMilestone, nextReleaseName);
            }
            else
            {
                var nextReleaseName = version;

                var nextMilestone = await gitHubClient.CreateNextMilestone(nextReleaseName);
                await gitHubClient.CreateNextRelease(nextReleaseName);
                await gitHubClient.CreateNextIssue(existingReleaseIssue, nextMilestone, nextReleaseName);
            }
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

        private static async Task<Milestone> GetExistingMilestone(this IGitHubClient gitHubClient, string existingMilestoneTitle)
        {
            Console.WriteLine($"Fetching milestone '{existingMilestoneTitle}'...");
            var milestoneRequest = new MilestoneRequest { State = ItemStateFilter.Open };
            var existingMilestone = (await gitHubClient.Issue.Milestone.GetAllForRepository(RepoOwner, RepoName, milestoneRequest))
                .Single(milestone => milestone.Title == existingMilestoneTitle);
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

        private static Issue GetExistingReleaseIssue(IList<Issue> issues, string existingReleaseName)
        {
            var issue = issues.Single(i => i.Title == $"Release {existingReleaseName}");
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

        private static async Task<Milestone> CreateNextMilestone(this IGitHubClient gitHubClient, string nextReleaseName)
        {
            var newMilestone = new NewMilestone(nextReleaseName);
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

        private static async Task CreateNextRelease(this IGitHubClient gitHubClient, string nextReleaseName)
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

            var newRelease = new NewRelease(nextReleaseName) { Draft = true, Name = nextReleaseName, Body = newReleaseBody.Trim() };
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

        private static async Task CreateNextIssue(this IGitHubClient gitHubClient, Issue existingIssue, Milestone nextMilestone, string nextReleaseName)
        {
            var existingReleaseName = existingIssue.Title.Replace("Release ", string.Empty);
            var newIssue = new NewIssue($"Release {nextReleaseName}")
            {
                Milestone = nextMilestone.Number,
                Body = Regex.Replace(existingIssue.Body, $@"\b{existingReleaseName}\b", nextReleaseName).Replace("[x]", "[ ]"),
                Labels = { "build", "documentation" }
            };

            Console.WriteLine($"Creating new release issue '{newIssue.Title}'...");
            var nextIssue = await gitHubClient.Issue.Create(RepoOwner, RepoName, newIssue);
            Console.WriteLine($"Created new release issue #{nextIssue.Number}: '{newIssue.Title}'");
        }
    }
}
