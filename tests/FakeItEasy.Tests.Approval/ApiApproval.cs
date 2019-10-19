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
        [InlineData("net40", "ApproveApi40")]
        [InlineData("net45", "ApproveApi45")]
        [InlineData("netstandard1.6", "ApproveApiNetStd16")]
        [InlineData("netstandard2.0", "ApproveApiNetStd20")]
        [InlineData("netstandard2.1", "ApproveApiNetStd21")]
        [UseReporter(typeof(DiffReporter))]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void ApproveApi(string frameworkVersion, string baseName)
        {
            ApproveApi("FakeItEasy", frameworkVersion, baseName);
        }

        [Theory]
        [InlineData("net45", "ApproveExtensionsValueTaskApi45")]
        [InlineData("netstandard1.6", "ApproveExtensionsValueTaskApiNetStd16")]
        [InlineData("netstandard2.0", "ApproveExtensionsValueTaskApiNetStd20")]
        [UseReporter(typeof(DiffReporter))]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void ApproveExtensionsValueTaskApi(string frameworkVersion, string baseName)
        {
            ApproveApi("FakeItEasy.Extensions.ValueTask", frameworkVersion, baseName);
        }

        private static void ApproveApi(string projectName, string frameworkVersion, string baseName)
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(new Uri(codeBase));
            string assemblyPath = Uri.UnescapeDataString(uri.Path);
            var containingDirectory = Path.GetDirectoryName(assemblyPath);
            var configurationName = new DirectoryInfo(containingDirectory).Parent.Name;
            var assemblyFile = $@"..\..\..\..\..\src\{projectName}\bin\{configurationName}\{frameworkVersion}\{projectName}.dll";

            var assembly = Assembly.LoadFile(Path.GetFullPath(assemblyFile));
            var publicApi = ApiGenerator.GeneratePublicApi(assembly);

            Approvals.Verify(
                WriterFactory.CreateTextWriter(publicApi),
                new ApprovalNamer($"{nameof(ApiApproval)}.{baseName}"),
                Approvals.GetReporter());
        }

        private class ApprovalNamer : IApprovalNamer
        {
            public ApprovalNamer(string name, [CallerFilePath] string sourcePath = null)
            {
                this.Name = name;
                this.SourcePath = Path.GetDirectoryName(sourcePath);
            }

            public string SourcePath { get; }

            public string Name { get; }
        }
    }
}
