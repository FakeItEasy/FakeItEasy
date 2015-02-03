namespace FakeItEasy.Tests
{
    using System.Linq.Expressions;
    using TestHelpers;

    public class LambdaExpressionDummyFactory
        : DummyFactory<LambdaExpression>
    {
        protected override LambdaExpression Create()
        {
            return ExpressionHelper.CreateExpression<string>(x => x.Equals(null));
        }
    }
}