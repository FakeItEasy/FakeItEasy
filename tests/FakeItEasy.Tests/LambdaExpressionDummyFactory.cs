namespace FakeItEasy.Tests
{
    using System.Linq.Expressions;
    using FakeItEasy.Tests.TestHelpers;

    public class LambdaExpressionDummyFactory
        : DummyFactory<LambdaExpression>
    {
        protected override LambdaExpression Create()
        {
            return ExpressionHelper.CreateExpression<string>(x => object.ReferenceEquals(x, null));
        }
    }
}
