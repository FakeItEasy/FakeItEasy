namespace FakeItEasy
{
    using System.Linq.Expressions;
    using FakeItEasy.Core;

    internal static class Helpers
    {
        public static string GetDescription(this IFakeObjectCall fakeObjectCall)
        {
            var method = fakeObjectCall.Method;
            return "{0}.{1}({2})".FormatInvariant(method.DeclaringType.FullName, method.Name, GetParametersString(fakeObjectCall));
        }

        /// <summary>
        /// Evaluates an expression by compiling it into a delegate and invoking the delegate.
        /// </summary>
        /// <param name="expression">The expression to be evaluated.</param>
        /// <returns>The value returned from the delegate compiled from the expression.</returns>
        public static object Evaluate(this Expression expression)
        {
            var lambda = Expression.Lambda(expression).Compile();
            return lambda.DynamicInvoke();
        }

        private static string GetParametersString(IFakeObjectCall fakeObjectCall)
        {
            return fakeObjectCall.Arguments.ToCollectionString(x => GetArgumentAsString(x), ", ");
        }

        private static string GetArgumentAsString(object argument)
        {
            if (argument == null)
            {
                return "<NULL>";
            }

            var stringArgument = argument as string;
            if (stringArgument != null)
            {
                return stringArgument.Length > 0 ? string.Concat("\"", stringArgument, "\"") : "<string.Empty>";
            }

            return argument.ToString();
        }
    }
}