namespace FakeItEasy.Tests
{
    using System;
    using System.Linq.Expressions;
    using Xunit;

    public class CapturedTests
    {
        [Fact]
        public void FrozenBy_should_be_null_guarded()
        {
            var captured = A.Captured<string>();
#pragma warning disable CA1806 // Do not ignore method results
            Expression<Action> call = () => captured.FrozenBy(s => s);
#pragma warning restore CA1806 // Do not ignore method results
            call.Should().BeNullGuarded();
        }
    }
}
