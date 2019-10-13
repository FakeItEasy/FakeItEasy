namespace FakeItEasy.Tools
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using Octokit;

    public static class ReleaseHelpers
    {
        public static ICollection<string> GetIssuesReferencedFromReleases(IEnumerable<Release> releases)
        {
            // Release bodies should contain references to fixed issues in the form
            // (#1234), or (#1234, #1235, #1236) if multiple issues apply to a topic.
            // It's hard (impossible?) to harvest values from a repeated capture group,
            // so grab everything between the ()s and split manually.
            var issuesReferencedFromRelease = new HashSet<string>();
            foreach (var release in releases)
            {
                foreach (Match match in Regex.Matches(release.Body, @"\((?<issueNumbers>#[0-9]+((, )#[0-9]+)*)\)"))
                {
                    var issueNumbers = match.Groups["issueNumbers"].Value;
                    foreach (var issueNumber in issueNumbers.Split(new[] { '#', ' ', ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        issuesReferencedFromRelease.Add(issueNumber);
                    }
                }
            }

            return issuesReferencedFromRelease;
        }
    }
}
