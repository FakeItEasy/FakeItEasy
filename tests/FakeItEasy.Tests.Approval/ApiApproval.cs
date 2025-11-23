namespace FakeItEasy.Tests.Approval
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using PublicApiGenerator;
    using VerifyTests;
    using VerifyTests.DiffPlex;
    using VerifyXunit;
    using Xunit;

    [UsesVerify]
    public class ApiApproval
    {
        static ApiApproval() => VerifyDiffPlex.Initialize(OutputType.Compact);

        [SkippableTheory]
        [InlineData("FakeItEasy", "net462")]
        [InlineData("FakeItEasy", "net8.0")]
        [InlineData("FakeItEasy", "net10.0")]
        [InlineData("FakeItEasy.Extensions.ValueTask", "net462")]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public async Task ApproveApiAsync(string projectName, string frameworkVersion)
        {
            string codeBase = Assembly.GetExecutingAssembly().Location!;
            UriBuilder uri = new UriBuilder(new Uri(codeBase));
            string assemblyPath = Uri.UnescapeDataString(uri.Path);
            var containingDirectory = Path.GetDirectoryName(assemblyPath);
            var configurationName = new DirectoryInfo(containingDirectory!).Parent!.Name;
            var assemblyFile = Path.GetFullPath(
                Path.Combine(
                    GetSourceDirectory(),
                    $"../../src/{projectName}/bin/{configurationName}/{frameworkVersion}/{projectName}.dll"));

            Skip.IfNot(File.Exists(assemblyFile), "Assembly doesn't exist.");

            var assembly = Assembly.LoadFile(Path.GetFullPath(assemblyFile));
            var publicApi = ApiGenerator.GeneratePublicApi(assembly, options: null);

            var settings = new VerifySettings();
            settings.UseDirectory("ApprovedApi/" + projectName);
            settings.UseFileName(frameworkVersion);

            // If this starts failing for non-obvious reasons, or especially if
            // there's differing behavior on the build server and a contributor's
            // own machine, ensure that the same versions of the SDK are used to
            // build, and clear all generated files (e.g. the obj directories)
            // from the entire solution and try again.
            // See https://github.com/dotnet/sdk/issues/10774#issuecomment-973973378
            await Verifier.Verify(publicApi, settings);
        }

        private static string GetSourceDirectory([CallerFilePath] string path = "") => Path.GetDirectoryName(path)!;
    }
}
