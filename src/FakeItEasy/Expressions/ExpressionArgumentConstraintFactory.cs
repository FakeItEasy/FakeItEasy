namespace FakeItEasy.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using FakeItEasy.Configuration;
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
            var parameterType = argument.ArgumentInformation.ParameterType;

            if (IsParamArrayExpression(argument))
            {
                return this.CreateParamArrayConstraint((NewArrayExpression)argument.Expression, parameterType);
            }

            var isOutOrRefArgument = argument.ArgumentInformation.IsOutOrRef();

            var constraint = this.GetArgumentConstraintFromExpression(argument.Expression, parameterType, out var argumentValue);
            if (isOutOrRefArgument)
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

        private static bool IsParamArrayExpression(ParsedArgumentExpression argument)
        {
            return argument.Expression is NewArrayExpression && IsTaggedWithParamArrayAttribute(argument);
        }

        private static bool IsTaggedWithParamArrayAttribute(ParsedArgumentExpression argument)
        {
            return argument.ArgumentInformation.IsDefined(typeof(ParamArrayAttribute), true);
        }

        private static bool IsOutArgument(ParsedArgumentExpression argument)
        {
            return argument.ArgumentInformation.IsOut;
        }

        private static IArgumentConstraint CreateEqualityConstraint(object? expressionValue)
        {
            return new EqualityArgumentConstraint(expressionValue);
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
                int firstArgumentToCheck = IsArgumentConstraintManagerExtensionMethod(methodCallExpression) ? 1 : 0;
                for (var index = firstArgumentToCheck; index < methodCallExpression.Arguments.Count; index++)
                {
                    visitor.Visit(methodCallExpression.Arguments[index]);
                }
            }
            else
            {
                // An unknown kind of expression - could be a constructor, or almost anything else. Play it safe and
                // check it out.
                visitor.Visit(expression);
            }
        }

        private static bool IsArgumentConstraintManagerExtensionMethod(MethodCallExpression methodCallExpression)
        {
            if (methodCallExpression.Arguments.Count == 0)
            {
                return false;
            }

            // Checking for an extension method is very expensive, so check the common
            // case first - that the method is one of the predefined extension methods.
            if (GetGenericTypeDefinition(methodCallExpression.Method.DeclaringType) ==
                typeof(ArgumentConstraintManagerExtensions))
            {
                return true;
            }

            return methodCallExpression.Method.IsStatic &&
                   methodCallExpression.Method.IsDefined(typeof(ExtensionAttribute), false);
        }

        /// <summary>
        /// Removes the conversion node introduced in a Linq expression by implicit conversion.
        /// </summary>
        /// <param name="expression">The expression from which to remove the conversion.</param>
        /// <returns>The original expression, if no conversion is happening, or the expression that would be converted.</returns>
        private static Expression GetExpressionWithoutConversion(Expression expression)
        {
            while (expression is UnaryExpression conversion && conversion.NodeType == ExpressionType.Convert)
            {
                expression = conversion.Operand;
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

        private static void CheckConstraintIsCompatibleWithParameterType(ITypedArgumentConstraint constraint, Type parameterType)
        {
            parameterType = parameterType.IsByRef ? parameterType.GetElementType() : parameterType;

            if (!parameterType.IsAssignableFrom(constraint.Type))
            {
                throw new FakeConfigurationException(ExceptionMessages.ArgumentConstraintHasWrongType(constraint.Type, parameterType));
            }
        }

        private IArgumentConstraint GetArgumentConstraintFromExpression(Expression expression, Type parameterType, out object? value)
        {
            CheckArgumentExpressionIsValid(expression);

            object? expressionValue = null;

            IArgumentConstraint constraint;
            try
            {
                constraint = this.argumentConstraintTrapper.TrapConstraintOrCreate(
                    () => expressionValue = expression.Evaluate(),
                    () => CreateEqualityConstraint(expressionValue));
            }
            catch (TargetInvocationException ex)
            {
                throw new UserCallbackException(
                    ExceptionMessages.UserCallbackThrewAnException($"Argument constraint expression <{expression}>"),
                    ex.InnerException);
            }

            if (constraint is ITypedArgumentConstraint typedConstraint)
            {
                CheckConstraintIsCompatibleWithParameterType(typedConstraint, parameterType);
            }

            value = expressionValue;

            return constraint;
        }

        private IArgumentConstraint CreateParamArrayConstraint(NewArrayExpression expression, Type parameterType)
        {
            var result = new List<IArgumentConstraint>();
            var itemType = parameterType.GetElementType();

            foreach (var argumentExpression in expression.Expressions)
            {
                result.Add(this.GetArgumentConstraintFromExpression(argumentExpression, itemType, out _));
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
