namespace FakeItEasy.Tools
{
    using System;
    using System.IO;

    using static FakeItEasy.Tools.ToolHelpers;

    public static class GitHubTokenSource
    {
        public static string GetAccessToken()
        {
            var token = Environment.GetEnvironmentVariable("GITHUB_TOKEN");
            if (string.IsNullOrEmpty(token))
            {
                var tokenFilePath = Path.Combine(GetCurrentScriptDirectory(), ".githubtoken");
                if (File.Exists(tokenFilePath))
                {
                    token = File.ReadAllText(tokenFilePath)?.Trim();
                }
            }

            if (string.IsNullOrEmpty(token))
            {
                throw new Exception("GitHub access token is missing; please put it in tools/.githubtoken, or in the GITHUB_TOKEN environment variable.");
            }

            return token;
        }
    }
}
