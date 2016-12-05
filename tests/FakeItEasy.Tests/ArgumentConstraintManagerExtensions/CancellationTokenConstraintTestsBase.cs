namespace FakeItEasy.Tests.ArgumentConstraintManagerExtensions
{
    using System.Collections.Generic;
    using System.Threading;

    public abstract class CancellationTokenConstraintTestsBase : ArgumentConstraintTestBase<CancellationToken>
    {
        private static readonly CancellationTokenSource CanceledSource = CreateCancellationTokenSource(true);
        private static readonly CancellationTokenSource NonCanceledSource = CreateCancellationTokenSource(false);

        public static IEnumerable<object[]> NonCanceledTokens()
        {
            return TestCases.FromObject(
                CancellationToken.None,
                default(CancellationToken),
                new CancellationToken(false),
                NonCanceledSource.Token);
        }

        public static IEnumerable<object[]> CanceledTokens()
        {
            return TestCases.FromObject(
                new CancellationToken(true),
                CanceledSource.Token);
        }

        private static CancellationTokenSource CreateCancellationTokenSource(bool canceled)
        {
            var source = new CancellationTokenSource();
            if (canceled)
            {
                source.Cancel();
            }

            return source;
        }
    }
}
