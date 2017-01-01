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
        /// Parses the specified expression that represents a call to a natural fake.
        /// </summary>
        /// <param name="callExpression">The expression to parse.</param>
        /// <returns>The parsed expression.</returns>
        /// <remarks>The expression should have no parameter.</remarks>
        ParsedCallExpression Parse(LambdaExpression callExpression);

        /// <summary>
        /// Parses the specified expression that represents a call to an unnatural fake.
        /// </summary>
        /// <param name="callExpression">The expression to parse.</param>
        /// <param name="fake">The fake configured by this expression.</param>
        /// <returns>The parsed expression.</returns>
        /// <remarks>The expression should have one parameter which corresponds to the fake being configured.</remarks>
        ParsedCallExpression Parse(LambdaExpression callExpression, object fake);
    }
}
