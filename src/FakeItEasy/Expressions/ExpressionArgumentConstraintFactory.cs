namespace FakeItEasy.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
#if FEATURE_NETCORE_REFLECTION
    using System.Reflection;
#endif
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

            var isByRefArgument = IsByRefArgument(argument);

            object argumentValue;
            var constraint = this.GetArgumentConstraintFromExpression(argument.Expression, out argumentValue);
            if (isByRefArgument)
            {
                if (IsOutArgument(argument))
                {
                    constraint = new OutArgumentConstraint(argumentValue);
                }
                else
                {
                    constraint = new RefArgumentConstraint(constraint, argumentValue);
                }
            }

            return constraint;
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

        private static bool IsOutArgument(ParsedArgumentExpression argument)
        {
            return argument.ArgumentInformation.IsOut;
        }

        private static bool IsByRefArgument(ParsedArgumentExpression argument)
        {
            return argument.ArgumentInformation.ParameterType.IsByRef;
        }

        private static IArgumentConstraint CreateEqualityConstraint(object expressionValue)
        {
            return new EqualityArgumentConstraint(expressionValue);
        }

        private static object InvokeExpression(Expression expression)
        {
            return Expression.Lambda(expression).Compile().DynamicInvoke();
        }

        private IArgumentConstraint GetArgumentConstraintFromExpression(Expression expression, out object value)
        {
            object expressionValue = null;

            var trappedConstraints = this.argumentConstraintTrapper.TrapConstraints(() =>
            {
                expressionValue = InvokeExpression(expression);
            });

            value = expressionValue;

            return TryCreateConstraintFromTrappedConstraints(trappedConstraints.ToArray()) ?? CreateEqualityConstraint(expressionValue);
        }

        private IArgumentConstraint CreateParamArrayConstraint(NewArrayExpression expression)
        {
            var result = new List<IArgumentConstraint>();

            foreach (var argumentExpression in expression.Expressions)
            {
                object ignored;
                result.Add(this.GetArgumentConstraintFromExpression(argumentExpression, out ignored));
            }

            return new AggregateArgumentConstraint(result);
        }
    }
}
