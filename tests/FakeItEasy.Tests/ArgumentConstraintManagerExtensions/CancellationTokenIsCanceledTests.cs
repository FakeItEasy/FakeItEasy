namespace FakeItEasy.Tests.ArgumentConstraintManagerExtensions
{
    using System.Threading;
    using Xunit;

    public class CancellationTokenIsCanceledTests : CancellationTokenConstraintTestsBase
    {
        protected override string ExpectedDescription => "canceled cancellation token";

        [Theory]
        [MemberData(nameof(NonCanceledTokens))]
        public override void IsValid_should_return_false_for_invalid_values(object invalidValue)
        {
            base.IsValid_should_return_false_for_invalid_values(invalidValue);
        }

        [Theory]
        [MemberData(nameof(CanceledTokens))]
        public override void IsValid_should_return_true_for_valid_values(object validValue)
        {
            base.IsValid_should_return_true_for_valid_values(validValue);
        }

        protected override void CreateConstraint(INegatableArgumentConstraintManager<CancellationToken> scope)
        {
            scope.IsCanceled();
        }
    }
}
