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

        private static void CheckArgumentExpressionIsValid(Expression expression)
        {
            expression = GetExpressionWithoutBoxing(expression);

            if (expression is MemberExpression)
            {
                // It's A._, or A.Ignore, or some other property/field, so it's safe.
                return;
            }

            var visitor = new ArgumentConstraintExpressionVisitor();
            var methodCallExpression = expression as MethodCallExpression;
            if (methodCallExpression == null)
            {
                // An unknown kind of expression - could be a constructor, or almost anything else. Play it safe and
                // check it out.
                visitor.Visit(expression);
            }
            else
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
        }

        /// <summary>
        /// Removes the explicit conversion introduced in a Linq expression by implicit boxing.
        /// </summary>
        /// <param name="expression">The expression from which to remove the boxing.</param>
        /// <returns>The original expression, if no boxing is happening, or the expression that would be converted.</returns>
        private static Expression GetExpressionWithoutBoxing(Expression expression)
        {
            if (expression.NodeType == ExpressionType.Convert &&
                (expression.Type == typeof(object) || expression.Type.IsNullable() || expression.Type.GetTypeInfo().IsInterface))
            {
                var conversion = (UnaryExpression)expression;
                if (conversion.Operand.Type.GetTypeInfo().IsValueType)
                {
                    return conversion.Operand;
                }
            }

            return expression;
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
                object ignored;
                result.Add(this.GetArgumentConstraintFromExpression(argumentExpression, out ignored));
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
        }
    }
}
