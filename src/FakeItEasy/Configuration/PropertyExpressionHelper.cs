namespace FakeItEasy.Configuration
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using FakeItEasy.Expressions;

    internal static class PropertyExpressionHelper
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

            var parameterTypes = parsedCallExpression.ArgumentsExpressions
                .Select(p => p.ArgumentInformation.ParameterType)
                .Concat(new[] { parsedCallExpression.CalledMethod.ReturnType })
                .ToArray();

            // The DeclaringType may be null, so fall back to the faked object type.
            // (It's unlikely to happen, though, and would require the client code to have passed a specially
            // constructed expression to ACallToSet; perhaps a dynamically generated method created
            // via lightweight code generation.)
            var callTargetType = parsedCallExpression.CalledMethod.DeclaringType
                                 ?? Fake.GetFakeManager(parsedCallExpression.CallTarget).FakeObjectType;
            var setPropertyName = "set_" + propertyName;
            var indexerSetterInfo = callTargetType.GetMethod(setPropertyName, parameterTypes);

            if (indexerSetterInfo == null)
            {
                if (parsedCallExpression.ArgumentsExpressions.Any())
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

            var arguments = parsedCallExpression.ArgumentsExpressions
                .Take(originalParameterInfos.Length - 1)
                .Concat(new[] { newParsedSetterValueExpression });

            return new ParsedCallExpression(indexerSetterInfo, parsedCallExpression.CallTarget, arguments);
        }

        private static string GetExpressionDescription(ParsedCallExpression parsedCallExpression)
        {
            var writer = ServiceLocator.Current.Resolve<StringBuilderOutputWriter>();
            var constraintFactory = ServiceLocator.Current.Resolve<ExpressionArgumentConstraintFactory>();

            CallConstraintDescriber.DescribeCallOn(writer, parsedCallExpression.CalledMethod, parsedCallExpression.ArgumentsExpressions.Select(constraintFactory.GetArgumentConstraint));
            return writer.Builder.ToString();
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
