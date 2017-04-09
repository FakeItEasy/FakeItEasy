namespace FakeItEasy.Configuration
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using FakeItEasy.Core;
    using FakeItEasy.Expressions;

    internal class PropertyExpressionHelper
    {
        public static ParsedCallExpression BuildSetterFromGetter<TValue>(
            ParsedCallExpression parsedCallExpression)
        {
            var propertyName = GetPropertyName(parsedCallExpression);
            if (propertyName == null)
            {
                var expressionDescription = GetExpressionDescription(parsedCallExpression);
                throw new ArgumentException("Expression '" + expressionDescription +
                                            "' must refer to a property or indexer getter, but doesn't.");
            }

            var parsedArgumentExpressions = parsedCallExpression.ArgumentsExpressions ?? new ParsedArgumentExpression[0];
            var parameterTypes = parsedArgumentExpressions
                .Select(p => p.ArgumentInformation.ParameterType)
                .Concat(new[] { parsedCallExpression.CalledMethod.ReturnType })
                .ToArray();

            var indexerSetterInfo = parsedCallExpression.CallTarget.GetType()
                .GetMethod("set_" + propertyName, parameterTypes);

            if (indexerSetterInfo == null)
            {
                if (parsedArgumentExpressions.Any())
                {
                    var expressionDescription = GetExpressionDescription(parsedCallExpression);
                    throw new ArgumentException("Expression '" + expressionDescription +
                                                "' refers to an indexed property that does not have a setter.");
                }

                throw new ArgumentException($"The property {propertyName} does not have a setter.");
            }

            var originalParameterInfos = indexerSetterInfo.GetParameters();

            var newParsedSetterValueExpression = new ParsedArgumentExpression(
                BuildArgumentThatMatchesAnything<TValue>(),
                originalParameterInfos.Last());

            var arguments = parsedArgumentExpressions
                .Take(originalParameterInfos.Length - 1)
                .Concat(new[] { newParsedSetterValueExpression });

            return new ParsedCallExpression(indexerSetterInfo, parsedCallExpression.CallTarget, arguments);
        }

        private static string GetExpressionDescription(ParsedCallExpression parsedCallExpression)
        {
            var matcher = new ExpressionCallMatcher(
                parsedCallExpression,
                ServiceLocator.Current.Resolve<ExpressionArgumentConstraintFactory>(),
                ServiceLocator.Current.Resolve<MethodInfoManager>());

            return matcher.DescriptionOfMatchingCall;
        }

        private static string GetPropertyName(ParsedCallExpression parsedCallExpression)
        {
            var calledMethod = parsedCallExpression.CalledMethod;
            if (HasThis(calledMethod) && calledMethod.IsSpecialName)
            {
                var methodName = calledMethod.Name;
                if (methodName.StartsWith("get_", StringComparison.Ordinal))
                {
                    return methodName.Substring(4);
                }
            }

            return null;
        }

        private static bool HasThis(MethodInfo methodCall)
        {
            return methodCall.CallingConvention.HasFlag(CallingConventions.HasThis);
        }

        private static Expression BuildArgumentThatMatchesAnything<TValue>()
        {
            Expression<Func<TValue>> value = () => A<TValue>.Ignored;
            return value.Body;
        }
    }
}
