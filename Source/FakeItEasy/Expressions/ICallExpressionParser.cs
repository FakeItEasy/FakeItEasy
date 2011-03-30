namespace FakeItEasy.Expressions
{
    using System.Linq.Expressions;

    /// <summary>
    /// Represents a class that can parse a lambda expression
    /// that represents a method or property call.
    /// </summary>
    internal interface ICallExpressionParser
    {
        /// <summary>
        /// Parses the specified expression.
        /// </summary>
        /// <param name="callExpression">The expression to parse.</param>
        /// <returns>The parsed expression.</returns>
        ParsedCallExpression Parse(LambdaExpression callExpression);
    }
}