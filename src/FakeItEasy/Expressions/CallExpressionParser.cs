namespace FakeItEasy.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq.Expressions;
    using System.Reflection;
    using FakeItEasy.Compatibility;

    internal class CallExpressionParser : ICallExpressionParser
    {
        public ParsedCallExpression Parse(LambdaExpression callExpression)
        {
            return callExpression.Body switch
            {
                MethodCallExpression methodExpression => ParseMethodCallExpression(methodExpression),
                MemberExpression propertyExpression => ParsePropertyCallExpression(propertyExpression),
                _ => ParseInvocationExpression((InvocationExpression)callExpression.Body),
            };
        }

        public ParsedCallExpression Parse(LambdaExpression callExpression, object fake)
        {
            // Unnatural fakes use an expression with a parameter (the fake).
            // Transform it into an expression with no parameters and with
            // references to the parameter replaced with the fake itself,
            // so that it can be parsed the same way as for natural fakes.
            var rewrittenCallExpression = ReplaceParameterWithFake(callExpression, fake);
            return this.Parse(rewrittenCallExpression);
        }

        private static ParsedCallExpression ParseInvocationExpression(InvocationExpression expression)
        {
            var expressionType = expression.Expression.Type;
            var method = expressionType.GetMethod("Invoke");

            var argumentsExpressions = CreateParsedArgumentExpressions(expression.Arguments, method.GetParameters());

            return new ParsedCallExpression(
                calledMethod: method,
                callTargetExpression: expression.Expression,
                argumentsExpressions: argumentsExpressions);
        }

        private static ParsedCallExpression ParseMethodCallExpression(MethodCallExpression expression)
        {
            var argumentsExpressions = CreateParsedArgumentExpressions(expression.Arguments, expression.Method.GetParameters());
            return new ParsedCallExpression(
                calledMethod: expression.Method,
                callTargetExpression: expression.Object,
                argumentsExpressions: argumentsExpressions);
        }

        private static ParsedCallExpression ParsePropertyCallExpression(MemberExpression expression)
        {
            var property = expression.Member as PropertyInfo;

            if (property is null)
            {
                throw new ArgumentException("The specified expression is not a method call or property getter.");
            }

            return new ParsedCallExpression(
                calledMethod: property.GetGetMethod(true),
                callTargetExpression: expression.Expression,
                argumentsExpressions: ArrayHelper.Empty<ParsedArgumentExpression>());
        }

        private static ParsedArgumentExpression[] CreateParsedArgumentExpressions(IList<Expression> expressionArguments, ParameterInfo[] parameters)
        {
            var argumentsExpressions = new ParsedArgumentExpression[expressionArguments.Count];
            for (int i = 0; i < argumentsExpressions.Length; i++)
            {
                argumentsExpressions[i] = new ParsedArgumentExpression(expressionArguments[i], parameters[i]);
            }

            return argumentsExpressions;
        }

        private static LambdaExpression ReplaceParameterWithFake(LambdaExpression callExpression, object fake)
        {
            var visitor = new ParameterValueReplacementVisitor(fake);
            return Expression.Lambda(visitor.Visit(callExpression.Body));
        }

        private class ParameterValueReplacementVisitor : ExpressionVisitor
        {
            private readonly object fake;

            public ParameterValueReplacementVisitor(object fake)
            {
                this.fake = fake;
            }

            [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "It's not public, and will never be called with a null value")]
            protected override Expression VisitParameter(ParameterExpression node)
            {
                return Expression.Constant(this.fake, node.Type);
            }
        }
    }
}
