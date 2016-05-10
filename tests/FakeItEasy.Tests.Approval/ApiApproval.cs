namespace FakeItEasy.Tests.Approval
{
    using System.Runtime.CompilerServices;
    using ApprovalTests;
    using ApprovalTests.Reporters;
    using NUnit.Framework;
    using PublicApiGenerator;

    public class ApiApproval
    {
        [Test]
        [UseReporter(typeof(DiffReporter))]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void ApproveApi()
        {
            Approvals.Verify(PublicApiGenerator.GetPublicApi(typeof(A).Assembly));
        }
    }
}
