namespace FakeItEasy.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;
    using FakeItEasy.Configuration;
    using FakeItEasy.Core;
    using FakeItEasy.Expressions.ArgumentConstraints;

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
        /// Initializes a new instance of the <see cref="ExpressionCallMatcher" /> class.
        /// </summary>
        /// <param name="callSpecification">The call specification.</param>
        /// <param name="constraintFactory">The constraint factory.</param>
        /// <param name="methodInfoManager">The method info manager to use.</param>
        /// <param name="callExpressionParser">A parser to use to parse call expressions.</param>
        public ExpressionCallMatcher(LambdaExpression callSpecification, ExpressionArgumentConstraintFactory constraintFactory, MethodInfoManager methodInfoManager, ICallExpressionParser callExpressionParser)
        {
            this.methodInfoManager = methodInfoManager;
            
            var parsedExpression = callExpressionParser.Parse(callSpecification);
            this.Method = parsedExpression.CalledMethod;

            this.argumentConstraints = GetArgumentConstraints(parsedExpression.ArgumentsExpressions, constraintFactory).ToArray();
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
        /// Matches the specified call against the expression.
        /// </summary>
        /// <param name="call">The call to match.</param>
        /// <returns>True if the call is matched by the expression.</returns>
        public virtual bool Matches(IFakeObjectCall call)
        {
            Guard.AgainstNull(call, "call");

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
            result.Append(this.Method.GetGenericArgumentsCSharp());
            
            this.AppendArgumentsListString(result);

            return result.ToString();
        }

        public virtual void UsePredicateToValidateArguments(Func<ArgumentCollection, bool> predicate)
        {
            this.argumentsPredicate = predicate;

            var numberOfValdiators = this.argumentConstraints.Count();
            this.argumentConstraints = Enumerable.Repeat<IArgumentConstraint>(new PredicatedArgumentConstraint(), numberOfValdiators);
        }

        public Func<IFakeObjectCall, ICollection<object>> GetOutAndRefParametersValueProducer()
        {
            var values = new List<object>();
            foreach (var constraint in this.argumentConstraints)
            {
                var valueProvidingConstraint = constraint as IArgumentValueProvider;
                if (valueProvidingConstraint != null)
                {
                    values.Add(valueProvidingConstraint.Value);
                }
            }

            if (values.Any())
            {
                return call => values;
            }

            return null;
        }

        private static IEnumerable<IArgumentConstraint> GetArgumentConstraints(IEnumerable<ParsedArgumentExpression> argumentExpressions, ExpressionArgumentConstraintFactory constraintFactory)
        {
            if (argumentExpressions == null)
            {
                return Enumerable.Empty<IArgumentConstraint>();    
            }

            return
                from argument in argumentExpressions
                select constraintFactory.GetArgumentConstraint(argument);
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

                constraint.WriteDescription(new StringBuilderOutputWriter(result));
            }

            result.Append(")");
        }

        private bool ArgumentsMatches(ArgumentCollection argumentCollection)
        {
            return this.argumentsPredicate(argumentCollection);
        }

        private bool ArgumentsMatchesArgumentConstraints(ArgumentCollection argumentCollection)
        {
            return argumentCollection
                .AsEnumerable()
                .Zip(this.argumentConstraints, (x, y) => new { ArgumentValue = x, Constraint = y })
                .All(x => x.Constraint.IsValid(x.ArgumentValue));
        }

        private class PredicatedArgumentConstraint
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

            public void WriteDescription(IOutputWriter writer)
            {
                writer.Write(this.ToString());
            }
        }
    }
}