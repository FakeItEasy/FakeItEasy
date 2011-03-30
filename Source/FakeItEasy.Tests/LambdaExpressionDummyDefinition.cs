namespace FakeItEasy.Tests
{
    using System.Linq.Expressions;
    using TestHelpers;

    public class LambdaExpressionDummyDefinition
        : DummyDefinition<LambdaExpression>
    {
        protected override LambdaExpression CreateDummy()
        {
            return ExpressionHelper.CreateExpression<string>(x => x.Equals(null));
        }
    }
}