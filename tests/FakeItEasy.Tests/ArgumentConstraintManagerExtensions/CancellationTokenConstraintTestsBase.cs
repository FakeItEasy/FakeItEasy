namespace FakeItEasy.Tests.ArgumentConstraintManagerExtensions
{
    using System.Collections.Generic;
    using System.Threading;

    public abstract class CancellationTokenConstraintTestsBase : ArgumentConstraintTestBase<CancellationToken>
    {
        private static readonly CancellationTokenSource CancelledSource = CreateCancellationTokenSource(true);
        private static readonly CancellationTokenSource NonCancelledSource = CreateCancellationTokenSource(false);

        public static IEnumerable<object[]> NonCancelledTokens()
        {
            return TestCases.FromObject(
                CancellationToken.None,
                default(CancellationToken),
                new CancellationToken(false),
                NonCancelledSource.Token);
        }

        public static IEnumerable<object[]> CancelledTokens()
        {
            return TestCases.FromObject(
                new CancellationToken(true),
                CancelledSource.Token);
        }

        private static CancellationTokenSource CreateCancellationTokenSource(bool cancelled)
        {
            var source = new CancellationTokenSource();
            if (cancelled)
            {
                source.Cancel();
            }

            return source;
        }
    }
}
