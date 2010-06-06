namespace FakeItEasy.Expressions
{
    using System.Linq.Expressions;
    using System.Reflection;
    using FakeItEasy.Core;
    using FakeItEasy.Expressions.ArgumentConstraints;

    /// <summary>
    /// Responsible for creating argument constraints from arguments in an expression.
    /// </summary>
    public class ArgumentConstraintFactory
    {
        /// <summary>
        /// Gets an argument constraint for the argument represented by the expression.
        /// </summary>
        /// <param name="argument">The argument.</param>
        /// <returns>An IArgumentConstraint used to validated arguments in IFakeObjectCalls.</returns>
        public virtual IArgumentConstraint GetArgumentConstraint(Expression argument)
        {
            IArgumentConstraint result = null;
            
            if (!TryGetArgumentConstraint(argument, out result))
            {
                result = new EqualityArgumentConstraint(Helpers.GetValueProducedByExpression(argument));
            }

            return result;
        }

        private static bool TryGetArgumentConstraint(Expression argument, out IArgumentConstraint result)
        {
            if (TryGetAbstractConstraint(argument, out result))
            {
                return true;
            }

            result = null;
            return false;
        }

        private static bool TryGetAbstractConstraint(Expression argument, out IArgumentConstraint result)
        {
            var unary = argument as UnaryExpression;
            if (unary != null && IsArgumentConstraintConversionMethod(unary.Method))
            {
                result = Helpers.GetValueProducedByExpression(unary.Operand) as IArgumentConstraint;
                return true;
            }

            var member = argument as MemberExpression;
            if (member != null && IsArgumentConstraintArgumentProperty(member))
            {
                result = Helpers.GetValueProducedByExpression(member.Expression) as IArgumentConstraint;
                return true;
            }

            result = Helpers.GetValueProducedByExpression(argument) as IArgumentConstraint;
            return result != null;
        }

        private static bool IsArgumentConstraintArgumentProperty(MemberExpression member)
        {
            return
                member.Member.Name == "Argument"
                && member.Member.DeclaringType.GetGenericTypeDefinition().Equals(typeof(ArgumentConstraint<>));
        }

        private static bool IsArgumentConstraintConversionMethod(MethodInfo method)
        {
            return
                method != null
                && method.Name.Equals("op_Implicit")
                && method.DeclaringType.GetGenericTypeDefinition().Equals(typeof(ArgumentConstraint<>));
        }
    }
}