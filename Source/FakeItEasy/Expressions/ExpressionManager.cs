namespace FakeItEasy.Expressions
{
    using System.Linq.Expressions;
using FakeItEasy.Core;

    /// <summary>
    /// Handles operations on expressions.
    /// </summary>
    internal static class ExpressionManager
    {
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