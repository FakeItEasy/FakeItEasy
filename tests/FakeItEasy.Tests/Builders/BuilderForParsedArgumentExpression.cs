namespace FakeItEasy.Tests.Builders
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using FakeItEasy.Expressions;

    internal class BuilderForParsedArgumentExpression : TestDataBuilder<ParsedArgumentExpression, BuilderForParsedArgumentExpression>
    {
        private Expression expression;
        private ParameterInfo parameterInfo;

        public BuilderForParsedArgumentExpression()
        {
            this.FromFirstArgumentInMethodCall(() => object.ReferenceEquals(null, this));
        }

        public BuilderForParsedArgumentExpression Expression(Action<BuilderForExpression> expressionBuilder)
        {
            return this.Do(x => x.expression = BuilderForExpression.Build(expressionBuilder));
        }

        public BuilderForParsedArgumentExpression WithExpression(Expression newExpression)
        {
            return this.Do(x => x.expression = newExpression);
        }

        public BuilderForParsedArgumentExpression WithConstantExpression(object value)
        {
            return this.Expression(x => x.Constant(value));
        }

        public BuilderForParsedArgumentExpression FromFirstArgumentInMethodCall(Expression<Action> invocation)
        {
            var methodCall = invocation.Body as MethodCallExpression;

            Debug.Assert(methodCall != null, "The invocation must be a method call.");

            this.expression = methodCall.Arguments.First();

            this.parameterInfo = methodCall.Method.GetParameters().First();

            return this;
        }

        protected override ParsedArgumentExpression Build()
        {
            return new ParsedArgumentExpression(this.expression, this.parameterInfo);
        }
    }
}
