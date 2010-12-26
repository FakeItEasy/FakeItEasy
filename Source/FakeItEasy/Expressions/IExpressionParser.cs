namespace FakeItEasy.Expressions
{
    using System.Linq.Expressions;
    using FakeItEasy.Core;

    /// <summary>
    /// Manages breaking call specification expression into their various parts.
    /// </summary>
    internal interface IExpressionParser
    {
        /// <summary>
        /// Gets the fake object an expression is called on.
        /// </summary>
        /// <param name="fakeObjectCall">The call expression.</param>
        /// <returns>The FakeManager instance that manages the faked object the call is made on.</returns>
        /// <exception cref="System.ArgumentNullException">The fakeObjectCall is null.</exception>
        /// <exception cref="System.ArgumentException">The specified expression is not an expression where a call is made to a faked object.</exception>
        FakeManager GetFakeManagerCallIsMadeOn(LambdaExpression fakeObjectCall);
    }
}