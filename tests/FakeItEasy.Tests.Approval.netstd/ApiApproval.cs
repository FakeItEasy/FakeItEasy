namespace FakeItEasy.Tests.Approval
{
    using System;
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
        public void ApproveApiNetStd()
        {
            // Approvals and PublicApiGenerator aren't available for .NET Core, so
            // we'll load the .NET Standard FakeItEasy assembly from disk and
            // examine it.
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string assemblyPath = Uri.UnescapeDataString(uri.Path);
            var containingDirectory = Path.GetDirectoryName(assemblyPath);
            var configurationName = new DirectoryInfo(containingDirectory).Name;
            var assemblyFile = @"..\..\..\..\src\FakeItEasy.netstd\bin\" + configurationName + @"\FakeItEasy.dll";

            var assembly = Assembly.LoadFrom(assemblyFile);
            Approvals.Verify(PublicApiGenerator.GetPublicApi(assembly));
        }
    }
}
