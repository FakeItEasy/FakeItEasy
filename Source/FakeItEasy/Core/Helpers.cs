namespace FakeItEasy.Core
{
    using System;
    using System.Diagnostics;
    using System.Linq.Expressions;

    internal static class Helpers
    {
        public static string GetDescription(this IFakeObjectCall fakeObjectCall)
        {
            var method = fakeObjectCall.Method;

            return "{0}.{1}({2})".FormatInvariant(method.DeclaringType.FullName, method.Name, GetParametersString(fakeObjectCall));
        }

        private static string GetParametersString(IFakeObjectCall fakeObjectCall)
        {
            return fakeObjectCall.Arguments.AsEnumerable().ToCollectionString(x => GetArgumentAsString(x), ", ");
        }

        private static string GetArgumentAsString(object argument)
        {
            if (argument == null)
            {
                return "<NULL>";
            }

            string stringArgument = argument as string;
            if (stringArgument != null)
            {
                return stringArgument.Length > 0 ? string.Concat("\"", stringArgument, "\"") : "<string.Empty>";
            }

            return argument.ToString();
        }

        [DebuggerStepThrough]
        public static object GetDefaultValueOfType(Type type)
        {
            return type.IsValueType && !type.Equals(typeof(void)) ? Activator.CreateInstance(type) : null;
        }

        /// <summary>
        /// Gets the value produced by the specified expression when compiled and invoked.
        /// </summary>
        /// <param name="expression">The expression to get the value from.</param>
        /// <returns>The value produced by the expression.</returns>
        public static object GetValueProducedByExpression(Expression expression)
        {
            var lambda = Expression.Lambda(expression).Compile();
            return lambda.DynamicInvoke();
        }
    }
}
