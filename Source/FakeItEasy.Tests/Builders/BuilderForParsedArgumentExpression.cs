namespace FakeItEasy.Tests.Builders
{
    using System;
    using System.Linq;
    using System.Reflection;
    using FakeItEasy.Expressions;
    using System.Linq.Expressions;
    using System.Diagnostics;

    internal class BuilderForParsedArgumentExpression : TestDataBuilder<ParsedArgumentExpression, BuilderForParsedArgumentExpression>
    {
        Expression expression;
        ParameterInfo parameterInfo;

        public BuilderForParsedArgumentExpression()
        {
            this.FromFirstArgumentInMethodCall(() => this.Equals(null));
        }

        protected override ParsedArgumentExpression Build()
        {
            return new ParsedArgumentExpression(this.expression, this.parameterInfo);
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

        public BuilderForParsedArgumentExpression FromFirstArgumentInMethodCall(Expression<Action> invokation)
        {
            var methodCall = invokation.Body as MethodCallExpression;

            Debug.Assert(methodCall != null);

            this.expression = methodCall.Arguments.First();

            this.parameterInfo = methodCall.Method.GetParameters().First();

            return this;
        }
    }
}