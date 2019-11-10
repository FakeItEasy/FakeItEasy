namespace FakeItEasy.Configuration
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using FakeItEasy.Expressions;

    internal static class PropertyExpressionHelper
    {
        /// <summary>
        /// Builds a parsed call expression for a property setter from the matching getter. The property must be interceptable as a faked method.
        /// </summary>
        /// <typeparam name="TValue">The return type of the getter.</typeparam>
        /// <param name="parsedCallExpression">
        /// The parsed call expression describing the getter.
        /// Must have a non-null <see cref="ParsedCallExpression.CallTarget"/>.
        /// </param>
        /// <returns>A parsed call expression describing the setter that matches the supplied getter.</returns>
        public static ParsedCallExpression BuildSetterFromGetter<TValue>(ParsedCallExpression parsedCallExpression)
        {
            var propertyName = GetPropertyName(parsedCallExpression);
            var parameterTypes = new Type[parsedCallExpression.ArgumentsExpressions.Length + 1];
            for (int i = 0; i < parsedCallExpression.ArgumentsExpressions.Length; ++i)
            {
                parameterTypes[i] = parsedCallExpression.ArgumentsExpressions[i].ArgumentInformation.ParameterType;
            }

            parameterTypes[parsedCallExpression.ArgumentsExpressions.Length] = parsedCallExpression.CalledMethod.ReturnType;

            // The DeclaringType may be null, so fall back to the faked object type.
            // (It's unlikely to happen, though, and would require the client code to have passed a specially
            // constructed expression to ACallToSet; perhaps a dynamically generated method created
            // via lightweight code generation.)
            var callTargetType = parsedCallExpression.CalledMethod.DeclaringType
                                 ?? Fake.GetFakeManager(parsedCallExpression.CallTarget!).FakeObjectType;
            var setPropertyName = "set_" + propertyName;
            var indexerSetterInfo = callTargetType.GetMethod(setPropertyName, parameterTypes);

            if (indexerSetterInfo is null)
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

            var arguments = new ParsedArgumentExpression[originalParameterInfos.Length];
            Array.Copy(parsedCallExpression.ArgumentsExpressions, arguments, originalParameterInfos.Length - 1);
            arguments[originalParameterInfos.Length - 1] = newParsedSetterValueExpression;

            return new ParsedCallExpression(indexerSetterInfo, parsedCallExpression.CallTarget, arguments);
        }

        private static string GetExpressionDescription(ParsedCallExpression parsedCallExpression)
        {
            var writer = ServiceLocator.Resolve<StringBuilderOutputWriter.Factory>().Invoke();
            var constraintFactory = ServiceLocator.Resolve<ExpressionArgumentConstraintFactory>();

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

            var expressionDescription = GetExpressionDescription(parsedCallExpression);
            throw new ArgumentException("Expression '" + expressionDescription +
                                        "' must refer to a property or indexer getter, but doesn't.");
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
