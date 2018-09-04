namespace FakeItEasy.PrepareRelease
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using FakeItEasy.Tools;
    using Octokit;
    using Octokit.Helpers;

    public static class Program
    {
        private const string RepoOwner = "FakeItEasy";
        private const string RepoName = "FakeItEasy";
        private const string ReleaseFromBranchName = "develop";
        private const string TargetBranchName = "master";
        private const string ExistingReleaseName = "vNext";
        private const string NextReleaseName = ExistingReleaseName;

        public static async Task Main(string[] args)
        {
            var version = args.FirstOrDefault();
            if (version == null)
            {
                throw new Exception("No version supplied");
            }

            var releaseBranchName = $"release/{version}";

            var gitHubClient = GetAuthenticatedGitHubClient();

            var existingMilestone = await gitHubClient.GetExistingMilestone();

            var issuesInExistingMilestone = await gitHubClient.GetIssuesInMilestone(existingMilestone);
            var existingReleaseIssue = TakeExistingReleaseIssue(issuesInExistingMilestone);

            var existingRelease = await gitHubClient.GetExistingRelease();
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
                await gitHubClient.RenameMilestone(existingMilestone, version);
                nextMilestone = await gitHubClient.CreateNextMilestone();
            }

            await gitHubClient.UpdateRelease(existingRelease, version);
            await gitHubClient.CreateNextRelease();
            await gitHubClient.UpdateIssue(existingReleaseIssue, existingMilestone, version);
            await gitHubClient.CreateNextIssue(existingReleaseIssue, nextMilestone);
            var releaseFromBranch = await gitHubClient.GetReleaseFromBranch();
            await gitHubClient.CreateReleaseBranch(releaseBranchName, releaseFromBranch);
            await gitHubClient.CreatePullRequest(version, releaseBranchName);
        }

        public static GitHubClient GetAuthenticatedGitHubClient()
        {
            var token = GitHubTokenSource.GetAccessToken();
            var credentials = new Credentials(token);
            return new GitHubClient(new ProductHeaderValue("FakeItEasy-build-scripts")) { Credentials = credentials };
        }

        public static async Task<Milestone> GetExistingMilestone(this IGitHubClient gitHubClient)
        {
            Console.WriteLine($"Fetching milestone '{ExistingReleaseName}'...");
            var milestoneRequest = new MilestoneRequest { State = ItemStateFilter.Open };
            var existingMilestone = (await gitHubClient.Issue.Milestone.GetAllForRepository(RepoOwner, RepoName, milestoneRequest))
                .Single(milestone => milestone.Title == ExistingReleaseName);
            Console.WriteLine($"Fetched milestone '{existingMilestone.Title}'");
            return existingMilestone;
        }

        public static async Task<Release> GetExistingRelease(this IGitHubClient gitHubClient)
        {
            Console.WriteLine($"Fetching GitHub release '{ExistingReleaseName}'...");
            var existingRelease = (await gitHubClient.Repository.Release.GetAll(RepoOwner, RepoName))
                .Single(release => release.Name == ExistingReleaseName && release.Draft == true);
            Console.WriteLine($"Fetched GitHub release '{existingRelease.Name}'");
            return existingRelease;
        }

        public static async Task<IList<Issue>> GetIssuesInMilestone(this IGitHubClient gitHubClient, Milestone milestone)
        {
            Console.WriteLine($"Fetching issues in milestone '{milestone.Title}'...'");
            var issueRequest = new RepositoryIssueRequest { Milestone = milestone.Number.ToString(), State = ItemStateFilter.All };
            var issues = (await gitHubClient.Issue.GetAllForRepository(RepoOwner, RepoName, issueRequest)).ToList();
            Console.WriteLine($"Fetched {issues.Count} issues in milestone '{milestone.Title}'");
            return issues;
        }

        public static IEnumerable<string> GetIssuesReferencedFromRelease(Release release)
        {
            // Release bodies should contain references to fixed issues in the form 
            // (#1234), or (#1234, #1235, #1236) if multiple issues apply to a topic.
            // It's hard (impossible?) to harvest values from a repeated capture group,
            // so grab everything between the ()s and split manually.
            var issuesReferencedFromRelease = new HashSet<string>();
            foreach (Match match in Regex.Matches(release.Body, @"\((?<issueNumbers>#[0-9]+((, )#[0-9]+)*)\)"))
            {
                var issueNumbers = match.Groups["issueNumbers"].Value;
                foreach (var issueNumber in issueNumbers.Split(new char[] { '#', ' ', ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    issuesReferencedFromRelease.Add(issueNumber);
                }
            }

            return issuesReferencedFromRelease;
        }

        public static Issue TakeExistingReleaseIssue(IList<Issue> issues)
        {
            var issue = issues.Single(i => i.Title == $"Release {ExistingReleaseName}");
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

        public static async Task RenameMilestone(this IGitHubClient gitHubClient, Milestone existingMilestone, string version)
        {
            var milestoneUpdate = new MilestoneUpdate { Title = version };
            Console.WriteLine($"Renaming milestone '{existingMilestone.Title}' to '{milestoneUpdate.Title}'...");
            var updatedMilestone = await gitHubClient.Issue.Milestone.Update(RepoOwner, RepoName, existingMilestone.Number, milestoneUpdate);
            Console.WriteLine($"Renamed milestone '{existingMilestone.Title}' to '{updatedMilestone.Title}'");
        }

        public static async Task<Milestone> CreateNextMilestone(this IGitHubClient gitHubClient)
        {
            var newMilestone = new NewMilestone(NextReleaseName);
            Console.WriteLine($"Creating new milestone '{newMilestone.Title}'...");
            var nextMilestone = await gitHubClient.Issue.Milestone.Create(RepoOwner, RepoName, newMilestone);
            Console.WriteLine($"Created new milestone '{nextMilestone.Title}'");
            return nextMilestone;
        }

        public static async Task UpdateRelease(this IGitHubClient gitHubClient, Release existingRelease, string version)
        {
            var releaseUpdate = new ReleaseUpdate { Name = version };
            Console.WriteLine($"Renaming GitHub release '{existingRelease.Name}' to {releaseUpdate.Name}...");
            var updatedRelease = await gitHubClient.Repository.Release.Edit(RepoOwner, RepoName, existingRelease.Id, releaseUpdate);
            Console.WriteLine($"Renamed GitHub release '{existingRelease.Name}' to {updatedRelease.Name}");
        }

        public static async Task CreateNextRelease(this IGitHubClient gitHubClient)
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

        public static async Task UpdateIssue(this IGitHubClient gitHubClient, Issue existingIssue, Milestone existingMilestone, string version)
        {
            var issueUpdate = new IssueUpdate { Title = $"Release {version}", Milestone = existingMilestone.Number };
            Console.WriteLine($"Renaming release issue '{existingIssue.Title}' to '{issueUpdate.Title}'...");
            var updatedIssue = await gitHubClient.Issue.Update(RepoOwner, RepoName, existingIssue.Number, issueUpdate);
            Console.WriteLine($"Renamed release issue '{existingIssue.Title}' to '{updatedIssue.Title}'");
        }

        public static async Task CreateNextIssue(this IGitHubClient gitHubClient, Issue existingIssue, Milestone nextMilestone)
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

        public static async Task<Reference> GetReleaseFromBranch(this IGitHubClient gitHubClient)
        {
            Console.WriteLine($"Fetching reference to branch {ReleaseFromBranchName}...");
            var releaseFromBranch = await gitHubClient.Git.Reference.Get(RepoOwner, RepoName, $"heads/{ReleaseFromBranchName}");
            Console.WriteLine($"Fetched reference to branch {releaseFromBranch.Url}");
            return releaseFromBranch;
        }

        public static async Task CreateReleaseBranch(this IGitHubClient gitHubClient, string releaseBranchName, Reference releaseFromBranch)
        {
            Console.WriteLine($"Creating branch {releaseBranchName} from {ReleaseFromBranchName}...");
            var releaseBranch = await gitHubClient.Git.Reference.CreateBranch(RepoOwner, RepoName, releaseBranchName, releaseFromBranch);
            Console.WriteLine($"Created branch at {releaseBranch.Url}");
        }

        public static async Task CreatePullRequest(this IGitHubClient gitHubClient, string version, string releaseBranchName)
        {
            Console.WriteLine($"Creating pull request to merge {releaseBranchName} into {TargetBranchName}...");
            var pr = await gitHubClient.PullRequest.Create(
                RepoOwner,
                RepoName,
                new NewPullRequest($"Release {version}", releaseBranchName, TargetBranchName));
            Console.WriteLine($"Created pull request '{pr.Title}' at {pr.HtmlUrl}");
        }
    }
}
