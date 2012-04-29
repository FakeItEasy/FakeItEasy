namespace FakeItEasy.Tests.Builders
{
    using System.Linq.Expressions;

    internal class BuilderForExpression : TestDataBuilder<Expression, BuilderForExpression>
    {
        Expression builtExpression;

        public BuilderForExpression()
        {
            this.builtExpression = Expression.Constant(null);
        }

        public BuilderForExpression Constant(object value)
        {
            return this.Do(x => x.builtExpression = Expression.Constant(value));
        }

        protected override Expression Build()
        {
            return this.builtExpression;
        }
    }
}