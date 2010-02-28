namespace FakeItEasy.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;
    using FakeItEasy.Api;

    /// <summary>
    /// Handles the matching of fake object calls to expressions.
    /// </summary>
    internal class ExpressionCallMatcher
        : ICallMatcher
    {
        private IEnumerable<IArgumentConstraint> argumentValidators;
        private MethodInfoManager methodInfoManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionCallMatcher"/> class.
        /// </summary>
        /// <param name="callSpecification">The call specification.</param>
        /// <param name="validatorFactory">The validator factory.</param>
        public ExpressionCallMatcher(LambdaExpression callSpecification, ArgumentValidatorFactory validatorFactory, MethodInfoManager methodInfoManager)
        {
            this.methodInfoManager = methodInfoManager;
            this.Method = GetMethodInfo(callSpecification);

            this.argumentValidators = GetArgumentValidators(callSpecification, validatorFactory).ToArray();
            this.argumentsPredicate = this.ArgumentsMatchesArgumentValidators;
        }

        private MethodInfo Method { get; set; }

        private static MethodInfo GetMethodInfo(LambdaExpression callSpecification)
        {
            var methodExpression = callSpecification.Body as MethodCallExpression;
            if (methodExpression != null)
            {
                return methodExpression.Method;
            }

            var memberExpression = callSpecification.Body as MemberExpression;
            if (memberExpression != null && memberExpression.Member.MemberType == MemberTypes.Property)
            {
                var property = memberExpression.Member as PropertyInfo;
                return property.GetGetMethod(true);
            }

            throw new ArgumentException(ExceptionMessages.CreatingExpressionCallMatcherWithNonMethodOrPropertyExpression);
        }

        private static IEnumerable<IArgumentConstraint> GetArgumentValidators(LambdaExpression callSpecification, ArgumentValidatorFactory validatorFactory)
        {
            var methodExpression = callSpecification.Body as MethodCallExpression;
            if (methodExpression != null)
            {
                return
                    (from argument in methodExpression.Arguments
                     select validatorFactory.GetArgumentValidator(argument));
            }

            return Enumerable.Empty<IArgumentConstraint>();
        }

        /// <summary>
        /// Matcheses the specified call against the expression.
        /// </summary>
        /// <param name="call">The call to match.</param>
        /// <returns>True if the call is matched by the expression.</returns>
        public virtual bool Matches(IFakeObjectCall call)
        {
            return this.InvokesSameMethodOnTarget(call.FakedObject.GetType(), call.Method, this.Method)
                && this.ArgumentsMatches(call.Arguments);
        }

        public virtual void UsePredicateToValidateArguments(Func<ArgumentCollection, bool> argumentsPredicate)
        {
            this.argumentsPredicate = argumentsPredicate;

            var numberOfValdiators = this.argumentValidators.Count();
            this.argumentValidators = Enumerable.Repeat<IArgumentConstraint>(new PredicatedArgumentValidator(), numberOfValdiators);
        }

        private bool InvokesSameMethodOnTarget(Type type, MethodInfo first, MethodInfo second)
        {
            return this.methodInfoManager.WillInvokeSameMethodOnTarget(type, first, second);
        }

        public override string ToString()
        {
            var result = new StringBuilder();

            result.Append(this.Method.DeclaringType.FullName);
            result.Append(".");
            result.Append(this.Method.Name);
            this.AppendArgumentsListString(result);

            return result.ToString();
        }

        private void AppendArgumentsListString(StringBuilder result)
        {
            result.Append("(");
            bool firstArgument = true;

            foreach (var validator in this.argumentValidators)
            {
                if (!firstArgument)
                {
                    result.Append(", ");
                }
                else
                {
                    firstArgument = false;
                }

                result.Append(validator.ToString());
            }

            result.Append(")");
        }

        private Func<ArgumentCollection, bool> argumentsPredicate;

        private bool ArgumentsMatches(ArgumentCollection argumentCollection)
        {
            return this.argumentsPredicate(argumentCollection);
        }

        private bool ArgumentsMatchesArgumentValidators(ArgumentCollection argumentCollection)
        {
            foreach (var argumentValidatorPair in argumentCollection.AsEnumerable().Zip(this.argumentValidators))
            {
                if (!argumentValidatorPair.Second.IsValid(argumentValidatorPair.First))
                {
                    return false;
                }
            }

            return true;
        }

        private class PredicatedArgumentValidator
            : IArgumentConstraint
        {
            public bool IsValid(object argument)
            {
                return true;
            }

            public override string ToString()
            {
                return "<Predicated>";
            }
        }
    }

    internal interface IExpressionCallMatcherFactory
    {
        ICallMatcher CreateCallMathcer(LambdaExpression callSpecification);
    }
}