namespace FakeItEasy.Expressions
{
    using System;
    using System.Linq;
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

            var propertyExpression = callExpression.Body as MemberExpression;
            if (propertyExpression != null)
            {
                return ParsePropertyCallExpression(propertyExpression);
            }

            return ParseInvokationExpression((InvocationExpression) callExpression.Body);
        }

        private static ParsedCallExpression ParseInvokationExpression(InvocationExpression expression)
        {
            var target = Helpers.GetValueProducedByExpression(expression.Expression);
            var method = target.GetType().GetMethod("Invoke");

            return new ParsedCallExpression(
                calledMethod: method, 
                callTargetExpression: expression.Expression,
                argumentsExpressions: from argument in expression.Arguments.Zip(method.GetParameters(), (x, y) => new { Expression = x, ParameterInfo = y })
                                      select new ParsedArgumentExpression(argument.Expression, argument.ParameterInfo));
        }

        private static ParsedCallExpression ParseMethodCallExpression(MethodCallExpression expression)
        {
            return new ParsedCallExpression(
                calledMethod: expression.Method,
                callTargetExpression: expression.Object,
                argumentsExpressions: from argument in expression.Arguments.Zip(expression.Method.GetParameters(), (x, y) => new { Expression = x, ParameterInfo = y})
                                      select new ParsedArgumentExpression(argument.Expression, argument.ParameterInfo));
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