namespace FakeItEasy.Tests.ArgumentConstraintManagerExtensions
{
    using System.Threading;
    using Xunit;

    public class CancellationTokenIsCancelledTests : CancellationTokenConstraintTestsBase
    {
        protected override string ExpectedDescription => "cancelled cancellation token";

        [Theory]
        [MemberData(nameof(NonCancelledTokens))]
        public override void IsValid_should_return_false_for_invalid_values(object invalidValue)
        {
            base.IsValid_should_return_false_for_invalid_values(invalidValue);
        }

        [Theory]
        [MemberData(nameof(CancelledTokens))]
        public override void IsValid_should_return_true_for_valid_values(object validValue)
        {
            base.IsValid_should_return_true_for_valid_values(validValue);
        }

        protected override void CreateConstraint(IArgumentConstraintManager<CancellationToken> scope)
        {
            scope.IsCancelled();
        }
    }
}
