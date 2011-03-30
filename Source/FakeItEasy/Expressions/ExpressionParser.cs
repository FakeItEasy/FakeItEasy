namespace FakeItEasy.Expressions
{
    using System;
    using System.Linq.Expressions;
    using Core;

    /// <summary>
    /// Manages breaking call specification expression into their various parts.
    /// </summary>
    internal class ExpressionParser
        : IExpressionParser
    {
        private readonly ICallExpressionParser innerParser;

        public ExpressionParser(ICallExpressionParser innerParser)
        {
            this.innerParser = innerParser;
        }

        /// <summary>
        /// Gets the fake object an expression is called on.
        /// </summary>
        /// <param name="fakeObjectCall">The call expression.</param>
        /// <returns>A FakeObject.</returns>
        /// <exception cref="ArgumentNullException">The fakeObjectCall is null.</exception>
        /// <exception cref="ArgumentException">The specified expression is not an expression where a call is made to a faked object.</exception>
        public FakeManager GetFakeManagerCallIsMadeOn(LambdaExpression fakeObjectCall)
        {
            Guard.AgainstNull(fakeObjectCall, "fakeObjectCall");

            var instance = this.innerParser.Parse(fakeObjectCall).CallTarget;

            if (instance == null)
            {
                throw new ArgumentException("The specified call is not made on a fake object.");
            }

            return Fake.GetFakeManager(instance);
        }
    }
}