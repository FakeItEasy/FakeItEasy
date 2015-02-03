namespace FakeItEasy.Tests
{
    using System.Linq.Expressions;
    using TestHelpers;

    public class LambdaExpressionDummyFactory
        : DummyFactory<LambdaExpression>
    {
        protected override LambdaExpression CreateDummy()
        {
            return ExpressionHelper.CreateExpression<string>(x => x.Equals(null));
        }
    }
}