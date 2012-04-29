namespace FakeItEasy.Tests.Builders
{
    using System;
    using System.Reflection;
    using FakeItEasy.Expressions;
    using System.Linq.Expressions;

    internal class BuilderForParsedArgumentExpression : TestDataBuilder<ParsedArgumentExpression, BuilderForParsedArgumentExpression>
    {
        Expression expression;

        protected override ParsedArgumentExpression Build()
        {
            return new ParsedArgumentExpression(this.expression, A.Dummy<ParameterInfo>());
        }

        public BuilderForParsedArgumentExpression Expression(Action<BuilderForExpression> expressionBuilder)
        {
            return this.Do(x => x.expression = BuilderForExpression.Build(expressionBuilder));
        }

        public BuilderForParsedArgumentExpression WithExpression(Expression expression)
        {
            return this.Do(x => x.expression = expression);
        }

        public BuilderForParsedArgumentExpression WithConstantExpression(object value)
        {
            return this.Expression(x => x.Constant(value));
        }
    }
}