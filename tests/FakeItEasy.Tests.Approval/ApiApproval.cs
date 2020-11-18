namespace FakeItEasy.Tests.Approval
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using ApprovalTests;
    using ApprovalTests.Core;
    using ApprovalTests.Reporters;
    using ApprovalTests.Writers;
    using PublicApiGenerator;
    using Xunit;

    public class ApiApproval
    {
        [SkippableTheory]
        [InlineData("FakeItEasy", "net45")]
        [InlineData("FakeItEasy", "netstandard2.0")]
        [InlineData("FakeItEasy", "netstandard2.1")]
        [InlineData("FakeItEasy.Extensions.ValueTask", "net45")]
        [InlineData("FakeItEasy.Extensions.ValueTask", "netstandard2.0")]
        [UseReporter(typeof(DiffReporter))]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void ApproveApi(string projectName, string frameworkVersion)
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

            Approvals.Verify(
                WriterFactory.CreateTextWriter(publicApi),
                new ApprovalNamer(projectName, frameworkVersion),
                Approvals.GetReporter());
        }

        private class ApprovalNamer : IApprovalNamer
        {
            public ApprovalNamer(string projectName, string frameworkVersion)
            {
                this.Name = frameworkVersion;
                this.SourcePath = Path.Combine(GetSourceDirectory(), "ApprovedApi", projectName);
            }

            public string SourcePath { get; }

            public string Name { get; }
        }

        private static string GetSourceDirectory([CallerFilePath] string path = "") => Path.GetDirectoryName(path)!;
    }
}
