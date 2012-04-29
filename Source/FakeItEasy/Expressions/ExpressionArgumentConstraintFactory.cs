using System.Linq;
using System.Linq.Expressions;
using FakeItEasy.Core;
using FakeItEasy.Expressions.ArgumentConstraints;

namespace FakeItEasy.Expressions
{
    internal class ExpressionArgumentConstraintFactory
    {
        private readonly IArgumentConstraintTrapper argumentConstraintTrapper;

        public ExpressionArgumentConstraintFactory(IArgumentConstraintTrapper argumentConstraintTrapper)
        {
            this.argumentConstraintTrapper = argumentConstraintTrapper;
        }

        public virtual IArgumentConstraint GetArgumentConstraint(ParsedArgumentExpression argument)
        {
            object expressionValue = null;
            var result = this.argumentConstraintTrapper.TrapConstraints(() => 
                                                                            {
                                                                                expressionValue = InvokeExpression(argument.Expression);
                                                                            }).SingleOrDefault();

            return result ?? CreateEqualityConstraint(expressionValue);
        }

        private static IArgumentConstraint CreateEqualityConstraint(object expressionValue)
        {
            return new EqualityArgumentConstraint(expressionValue);
        }

        private static object InvokeExpression(Expression expression)
        {
            return Expression.Lambda(expression).Compile().DynamicInvoke();
        }
    }
}