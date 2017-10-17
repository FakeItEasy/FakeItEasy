namespace FakeItEasy.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.CompilerServices;
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

            var constraint = this.GetArgumentConstraintFromExpression(argument.Expression, out var argumentValue);
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

        private static void CheckArgumentExpressionIsValid(Expression expression)
        {
            expression = GetExpressionWithoutConversion(expression);

            if (expression is MemberExpression memberExpression && IsMemberOfA(memberExpression.Member))
            {
                // It's A._, or A.Ignore, so it's safe.
                return;
            }

            var visitor = new ArgumentConstraintExpressionVisitor();
            if (expression is MethodCallExpression methodCallExpression)
            {
                // A method call. It might be A<T>.That.Matches, or one of the other extension methods, so don't
                // check the method node itself. Instead, look at all the arguments (except the first, if it's an
                // extension method).
                int argumentsToSkip = methodCallExpression.Method.IsDefined(typeof(ExtensionAttribute), false) ? 1 : 0;
                foreach (var argument in methodCallExpression.Arguments.Skip(argumentsToSkip))
                {
                    visitor.Visit(argument);
                }
            }
            else
            {
                // An unknown kind of expression - could be a constructor, or almost anything else. Play it safe and
                // check it out.
                visitor.Visit(expression);
            }
        }

        /// <summary>
        /// Removes the conversion node introduced in a Linq expression by implicit conversion.
        /// </summary>
        /// <param name="expression">The expression from which to remove the conversion.</param>
        /// <returns>The original expression, if no conversion is happening, or the expression that would be converted.</returns>
        private static Expression GetExpressionWithoutConversion(Expression expression)
        {
            if (expression is UnaryExpression conversion && conversion.NodeType == ExpressionType.Convert)
            {
                return conversion.Operand;
            }

            return expression;
        }

        private static bool IsMemberOfA(MemberInfo member)
        {
            return GetGenericTypeDefinition(member.DeclaringType) == typeof(A<>);
        }

        private static Type GetGenericTypeDefinition(Type type)
        {
            var typeInfo = type.GetTypeInfo();
            if (typeInfo.IsGenericType && !typeInfo.IsGenericTypeDefinition)
            {
                return typeInfo.GetGenericTypeDefinition();
            }

            return type;
        }

        private IArgumentConstraint GetArgumentConstraintFromExpression(Expression expression, out object value)
        {
            CheckArgumentExpressionIsValid(expression);

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
                result.Add(this.GetArgumentConstraintFromExpression(argumentExpression, out _));
            }

            return new AggregateArgumentConstraint(result);
        }

        private class ArgumentConstraintExpressionVisitor : ExpressionVisitor
        {
            protected override Expression VisitMember(MemberExpression node)
            {
                if (IsMemberOfA(node.Member))
                {
                    throw new InvalidOperationException(ExceptionMessages.ArgumentConstraintCannotBeNestedInArgument);
                }

                return base.VisitMember(node);
            }
        }
    }
}
