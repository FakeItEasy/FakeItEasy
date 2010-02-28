namespace FakeItEasy.Expressions
{
    using System;
    using System.Linq.Expressions;
    using FakeItEasy.Api;
    using System.Linq;
using System.Reflection;

    /// <summary>
    /// Responsible for creating argument validators from arguments in an expression.
    /// </summary>
    public class ArgumentConstraintFactory
    {
        /// <summary>
        /// Gets an argument validator for the argument represented by the expression.
        /// </summary>
        /// <param name="argument">The argument.</param>
        /// <returns>An IArgumentValidator used to validated arguments in IFakeObjectCalls.</returns>
        public virtual IArgumentConstraint GetArgumentValidator(Expression argument)
        {
            IArgumentConstraint result = null;
            
            if (!TryGetArgumentValidator(argument, out result))
            {
                result = new EqualityArgumentConstraint(ExpressionManager.GetValueProducedByExpression(argument));
            }

            return result;
        }

        private static bool TryGetArgumentValidator(Expression argument, out IArgumentConstraint result)
        {
            if (TryGetAbstractValidator(argument, out result))
            {
                return true;
            }

            result = null;
            return false;
        }

        private static bool TryGetAbstractValidator(Expression argument, out IArgumentConstraint result)
        {
            var unary = argument as UnaryExpression;
            if (unary != null && IsArgumentValidatorConversionMethod(unary.Method))
            {
                result = ExpressionManager.GetValueProducedByExpression(unary.Operand) as IArgumentConstraint;
                return true;
            }

            var member = argument as MemberExpression;
            if (member != null && IsArgumentValidatorArgumentProperty(member))
            {
                result = ExpressionManager.GetValueProducedByExpression(member.Expression) as IArgumentConstraint;
                return true;
            }

            result = ExpressionManager.GetValueProducedByExpression(argument) as IArgumentConstraint;
            return result != null;
        }

        private static bool IsArgumentValidatorArgumentProperty(MemberExpression member)
        {
            return
                member.Member.Name == "Argument"
                && member.Member.DeclaringType.GetGenericTypeDefinition().Equals(typeof(ArgumentConstraint<>));
        }

        private static bool IsArgumentValidatorConversionMethod(MethodInfo method)
        {
            return
                method != null
                && method.Name.Equals("op_Implicit")
                && method.DeclaringType.GetGenericTypeDefinition().Equals(typeof(ArgumentConstraint<>));
        }
    }
}