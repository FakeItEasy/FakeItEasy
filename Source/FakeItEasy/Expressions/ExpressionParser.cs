namespace FakeItEasy.Expressions
{
    using System;
    using System.Linq.Expressions;
    using FakeItEasy.Core;

    /// <summary>
    /// Manages breaking call specification expression into their various parts.
    /// </summary>
    internal class ExpressionParser
        : IExpressionParser
    {
        /// <summary>
        /// Gets the fake object an expression is called on.
        /// </summary>
        /// <param name="fakeObjectCall">The call expression.</param>
        /// <returns>A FakeObject.</returns>
        /// <exception cref="ArgumentNullException">The fakeObjectCall is null.</exception>
        /// <exception cref="ArgumentException">The specified expression is not an expression where a call is made to a faked object.</exception>
        public FakeObject GetFakeObjectCallIsMadeOn(LambdaExpression fakeObjectCall)
        {
            Guard.AgainstNull(fakeObjectCall, "fakeObjectCall");

            Expression callTargetExpression = null;

            var methodExpression = fakeObjectCall.Body as MethodCallExpression;
            if (methodExpression != null)
            {
                callTargetExpression = methodExpression.Object;
            }
            else
            {
                var propertyExpression = fakeObjectCall.Body as MemberExpression;
                callTargetExpression = propertyExpression.Expression;
            }

            if (callTargetExpression == null)
            {
                throw new ArgumentException("The specified call is not made on a fake object.");
            }

            return Fake.GetFakeObject(ExpressionManager.GetValueProducedByExpression(callTargetExpression));
        }
    }
}
