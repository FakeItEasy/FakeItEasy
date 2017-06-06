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
            ApproveApi("net45");
        }

        [Fact]
        [UseReporter(typeof(DiffReporter))]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void ApproveApi40()
        {
            ApproveApi("net40");
        }

        [Fact]
        [UseReporter(typeof(DiffReporter))]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void ApproveApiNetStd()
        {
            ApproveApi("netstandard1.6");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void ApproveApi(string frameworkVersion)
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(new Uri(codeBase));
            string assemblyPath = Uri.UnescapeDataString(uri.Path);
            var containingDirectory = Path.GetDirectoryName(assemblyPath);
            var configurationName = new DirectoryInfo(containingDirectory).Parent.Name;
            var assemblyFile = $@"..\..\..\..\..\src\FakeItEasy\bin\{configurationName}\{frameworkVersion}\FakeItEasy.dll";

            var assembly = Assembly.LoadFile(Path.GetFullPath(assemblyFile));
            Approvals.Verify(PublicApiGenerator.GetPublicApi(assembly));
        }
    }
}
