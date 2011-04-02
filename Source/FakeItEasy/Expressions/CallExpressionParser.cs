namespace FakeItEasy.Expressions
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;

    internal class CallExpressionParser : ICallExpressionParser
    {
        public ParsedCallExpression Parse(LambdaExpression callExpression)
        {
            var methodExpression = callExpression.Body as MethodCallExpression;
            if (methodExpression != null)
            {
                return ParseMethodCallExpression(methodExpression);
            }

            return ParsePropertyCallExpression((MemberExpression)callExpression.Body);
        }

        private static ParsedCallExpression ParseMethodCallExpression(MethodCallExpression expression)
        {
            return new ParsedCallExpression(
                calledMethod: expression.Method,
                callTargetExpression: expression.Object,
                argumentsExpressions: expression.Arguments);
        }

        private static ParsedCallExpression ParsePropertyCallExpression(MemberExpression expression)
        {
            var property = expression.Member as PropertyInfo;

            if (property == null)
            {
                throw new ArgumentException("The specified expression is not a method call or property getter.");
            }

            return new ParsedCallExpression(
                calledMethod: property.GetGetMethod(true),
                callTargetExpression: expression.Expression,
                argumentsExpressions: null);
        }
    }
}