namespace FakeItEasy.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using FakeItEasy.Core;
    using FakeItEasy.Expressions.ArgumentConstraints;

    internal class ExpressionArgumentConstraintFactory
    {
        private readonly IArgumentConstraintTrapper argumentConstraintTrapper;

        public ExpressionArgumentConstraintFactory(IArgumentConstraintTrapper argumentConstraintTrapper)
        {
            this.argumentConstraintTrapper = argumentConstraintTrapper;
        }

        public virtual IArgumentConstraint GetArgumentConstraint(ParsedArgumentExpression argument)
        {
            if (IsParamArrayExpression(argument))
            {
                return this.CreateParamArrayConstraint((NewArrayExpression)argument.Expression);
            }

            return this.GetArgumentConstraintFromExpression(argument.Expression);
        }

        private static IArgumentConstraint TryCreateConstraintFromTrappedConstraints(ICollection<IArgumentConstraint> trappedConstraints)
        {
            return trappedConstraints.FirstOrDefault();
        }

        private static bool IsParamArrayExpression(ParsedArgumentExpression argument)
        {
            return IsTaggedWithParamArrayAttribute(argument) && argument.Expression is NewArrayExpression;
        }

        private static bool IsTaggedWithParamArrayAttribute(ParsedArgumentExpression argument)
        {
            return argument.ArgumentInformation.GetCustomAttributes(typeof(ParamArrayAttribute), true).Any();
        }

        private static IArgumentConstraint CreateEqualityConstraint(object expressionValue)
        {
            return new EqualityArgumentConstraint(expressionValue);
        }

        private static object InvokeExpression(Expression expression)
        {
            return Expression.Lambda(expression).Compile().DynamicInvoke();
        }

        private IArgumentConstraint GetArgumentConstraintFromExpression(Expression expression)
        {
            object expressionValue = null;

            var trappedConstraints = this.argumentConstraintTrapper.TrapConstraints(() =>
            {
                expressionValue = InvokeExpression(expression);
            });

            return TryCreateConstraintFromTrappedConstraints(trappedConstraints.ToArray()) ?? CreateEqualityConstraint(expressionValue);
        }

        private IArgumentConstraint CreateParamArrayConstraint(NewArrayExpression expression)
        {
            var result = new List<IArgumentConstraint>();

            foreach (var argumentExpression in expression.Expressions)
            {
                result.Add(this.GetArgumentConstraintFromExpression(argumentExpression));
            }

            return new AggregateArgumentConstraint(result);
        }
    }
}