namespace FakeItEasy.Tools
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Octokit;

    public static class ReleaseHelpers
    {
        public static ICollection<int> GetIssueNumbersReferencedFromReleases(IEnumerable<Release> releases)
        {
            var issuesReferencedFromRelease = new HashSet<int>();
            foreach (var release in releases)
            {
                foreach (var capture in Regex.Matches(release.Body, @"\(\s*#(?<issueNumber>[0-9]+)(,\s*#(?<issueNumber>[0-9]+))*\s*\)")
                                             .SelectMany(match => match.Groups["issueNumber"].Captures))
                {
                    issuesReferencedFromRelease.Add(int.Parse(capture.Value, NumberStyles.Integer, NumberFormatInfo.InvariantInfo));
                }
            }

            return issuesReferencedFromRelease;
        }
    }
}
