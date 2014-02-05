namespace FakeItEasy
{
    using System.Linq.Expressions;

    /// <summary>
    /// Provides extension methods for <see cref="Expression"/>.
    /// </summary>
    internal static class ExpressionExtensions
    {
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
    }
}