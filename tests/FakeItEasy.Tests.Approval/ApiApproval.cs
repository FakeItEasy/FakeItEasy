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
        [Theory]
        [InlineData("net40")]
        [InlineData("net45")]
        [InlineData("netstandard1.6")]
        [InlineData("netstandard2.0")]
        [InlineData("netstandard2.1")]
        [UseReporter(typeof(DiffReporter))]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void ApproveFakeItEasyApi(string frameworkVersion)
        {
            ApproveApi("FakeItEasy", frameworkVersion);
        }

        [Theory]
        [InlineData("net45")]
        [InlineData("netstandard1.6")]
        [InlineData("netstandard2.0")]
        [UseReporter(typeof(DiffReporter))]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void ApproveExtensionsValueTaskApi(string frameworkVersion)
        {
            ApproveApi("FakeItEasy.Extensions.ValueTask", frameworkVersion);
        }

        private static void ApproveApi(string projectName, string frameworkVersion)
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(new Uri(codeBase));
            string assemblyPath = Uri.UnescapeDataString(uri.Path);
            var containingDirectory = Path.GetDirectoryName(assemblyPath);
            var configurationName = new DirectoryInfo(containingDirectory).Parent.Name;
            var assemblyFile = Path.GetFullPath(
                Path.Combine(
                    GetSourceDirectory(),
                    $@"..\..\src\{projectName}\bin\{configurationName}\{frameworkVersion}\{projectName}.dll"));

            var assembly = Assembly.LoadFile(Path.GetFullPath(assemblyFile));
            var publicApi = ApiGenerator.GeneratePublicApi(assembly);

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

        private static string GetSourceDirectory([CallerFilePath] string path = null) => Path.GetDirectoryName(path);
    }
}
