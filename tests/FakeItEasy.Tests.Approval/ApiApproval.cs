namespace FakeItEasy.Tests.Approval
{
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
        public void ApproveApi()
        {
            Approvals.Verify(PublicApiGenerator.GetPublicApi(typeof(A).Assembly));
        }
    }
}
