namespace FakeItEasy.Tests.Approval
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using ApprovalTests;
    using ApprovalTests.Reporters;
    using PublicApiGenerator;
    using Xunit;

    public class ApiApproval
    {
        [Fact]
        [UseReporter(typeof(DiffReporter))]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void ApproveApi45()
        {
            ApproveApi("FakeItEasy", "net45");
        }

        [Fact]
        [UseReporter(typeof(DiffReporter))]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void ApproveApi40()
        {
            ApproveApi("FakeItEasy", "net40");
        }

        [Fact]
        [UseReporter(typeof(DiffReporter))]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void ApproveApiNetStd16()
        {
            ApproveApi("FakeItEasy", "netstandard1.6");
        }

        [Fact]
        [UseReporter(typeof(DiffReporter))]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void ApproveApiNetStd20()
        {
            ApproveApi("FakeItEasy", "netstandard2.0");
        }

        [Fact]
        [UseReporter(typeof(DiffReporter))]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void ApproveApiNetStd21()
        {
            ApproveApi("FakeItEasy", "netstandard2.1");
        }

        [Fact]
        [UseReporter(typeof(DiffReporter))]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void ApproveExtensionsValueTaskApi45()
        {
            ApproveApi("FakeItEasy.Extensions.ValueTask", "net45");
        }

        [Fact]
        [UseReporter(typeof(DiffReporter))]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void ApproveExtensionsValueTaskApiNetStd16()
        {
            ApproveApi("FakeItEasy.Extensions.ValueTask", "netstandard1.6");
        }

        [Fact]
        [UseReporter(typeof(DiffReporter))]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void ApproveExtensionsValueTaskApiNetStd20()
        {
            ApproveApi("FakeItEasy.Extensions.ValueTask", "netstandard2.0");
        }

        private static void ApproveApi(string projectName, string frameworkVersion)
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(new Uri(codeBase));
            string assemblyPath = Uri.UnescapeDataString(uri.Path);
            var containingDirectory = Path.GetDirectoryName(assemblyPath);
            var configurationName = new DirectoryInfo(containingDirectory).Parent.Name;
            var assemblyFile = $@"..\..\..\..\..\src\{projectName}\bin\{configurationName}\{frameworkVersion}\{projectName}.dll";

            var assembly = Assembly.LoadFile(Path.GetFullPath(assemblyFile));
            Approvals.Verify(ApiGenerator.GeneratePublicApi(assembly));
        }
    }
}
