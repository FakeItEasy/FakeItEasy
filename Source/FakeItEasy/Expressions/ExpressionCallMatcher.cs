namespace FakeItEasy.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;
    using FakeItEasy.Core;

    /// <summary>
    /// Handles the matching of fake object calls to expressions.
    /// </summary>
    internal class ExpressionCallMatcher
        : ICallMatcher
    {
        private readonly MethodInfoManager methodInfoManager;
        private IEnumerable<IArgumentConstraint> argumentConstraints;
        private Func<ArgumentCollection, bool> argumentsPredicate;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionCallMatcher"/> class.
        /// </summary>
        /// <param name="callSpecification">The call specification.</param>
        /// <param name="constraintFactory">The constraint factory.</param>
        /// <param name="methodInfoManager">The method infor manager to use.</param>
        public ExpressionCallMatcher(LambdaExpression callSpecification, ArgumentConstraintFactory constraintFactory, MethodInfoManager methodInfoManager)
        {
            this.methodInfoManager = methodInfoManager;
            this.Method = GetMethodInfo(callSpecification);

            this.argumentConstraints = GetArgumentConstraints(callSpecification, constraintFactory).ToArray();
            this.argumentsPredicate = this.ArgumentsMatchesArgumentConstraints;
        }

        /// <summary>
        /// Gets a human readable description of calls that will be matched by this
        /// matcher.
        /// </summary>
        public virtual string DescriptionOfMatchingCall
        {
            get { return this.ToString(); }
        }

        private MethodInfo Method { get; set; }

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

        /// <summary>
        /// Gets a description of the call.
        /// </summary>
        /// <returns>Description of the call.</returns>
        public override string ToString()
        {
            var result = new StringBuilder();

            result.Append(this.Method.DeclaringType.FullName);
            result.Append(".");
            result.Append(this.Method.Name);
            this.AppendArgumentsListString(result);

            return result.ToString();
        }

        public virtual void UsePredicateToValidateArguments(Func<ArgumentCollection, bool> argumentsPredicate)
        {
            this.argumentsPredicate = argumentsPredicate;

            var numberOfValdiators = this.argumentConstraints.Count();
            this.argumentConstraints = Enumerable.Repeat<IArgumentConstraint>(new PredicatedArgumentConstraint(), numberOfValdiators);
        }

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

        private static IEnumerable<IArgumentConstraint> GetArgumentConstraints(LambdaExpression callSpecification, ArgumentConstraintFactory constraintFactory)
        {
            var methodExpression = callSpecification.Body as MethodCallExpression;
            if (methodExpression != null)
            {
                return
                    from argument in methodExpression.Arguments
                    select constraintFactory.GetArgumentConstraint(argument);
            }

            return Enumerable.Empty<IArgumentConstraint>();
        }

        private bool InvokesSameMethodOnTarget(Type type, MethodInfo first, MethodInfo second)
        {
            return this.methodInfoManager.WillInvokeSameMethodOnTarget(type, first, second);
        }

        private void AppendArgumentsListString(StringBuilder result)
        {
            result.Append("(");
            var firstArgument = true;

            foreach (var constraint in this.argumentConstraints)
            {
                if (!firstArgument)
                {
                    result.Append(", ");
                }
                else
                {
                    firstArgument = false;
                }

                result.Append(constraint.ConstraintDescription);
            }

            result.Append(")");
        }

        private bool ArgumentsMatches(ArgumentCollection argumentCollection)
        {
            return this.argumentsPredicate(argumentCollection);
        }

        private bool ArgumentsMatchesArgumentConstraints(ArgumentCollection argumentCollection)
        {
            foreach (var argumentConstraintPair in argumentCollection.AsEnumerable().Zip(this.argumentConstraints))
            {
                if (!argumentConstraintPair.Item2.IsValid(argumentConstraintPair.Item1))
                {
                    return false;
                }
            }

            return true;
        }

        private class PredicatedArgumentConstraint
            : IArgumentConstraint
        {
            public string ConstraintDescription
            {
                get { return this.ToString(); }
            }

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
}