namespace FakeItEasy.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
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
        private IArgumentConstraint[] argumentConstraints;
        private Func<ArgumentCollection, bool> argumentsPredicate;
        private bool useExplicitArgumentsPredicate;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionCallMatcher" /> class.
        /// </summary>
        /// <param name="parsedExpression">The parsed call specification.</param>
        /// <param name="constraintFactory">The constraint factory.</param>
        /// <param name="methodInfoManager">The method info manager to use.</param>
        public ExpressionCallMatcher(ParsedCallExpression parsedExpression, ExpressionArgumentConstraintFactory constraintFactory, MethodInfoManager methodInfoManager)
        {
            this.methodInfoManager = methodInfoManager;
            this.Method = parsedExpression.CalledMethod;

            var constraints = new IArgumentConstraint[parsedExpression.ArgumentsExpressions.Length];
            for (var i = 0; i < constraints.Length; i++)
            {
                constraints[i] = constraintFactory.GetArgumentConstraint(parsedExpression.ArgumentsExpressions[i]);
            }

            this.argumentConstraints = constraints;
            this.argumentsPredicate = this.ArgumentsMatchesArgumentConstraints;
        }

        private MethodInfo Method { get; }

        /// <summary>
        /// Writes a description of calls the rule is applicable to.
        /// </summary>
        /// <param name="writer">The writer on which to describe the call.</param>
        public virtual void DescribeCallOn(IOutputWriter writer) => CallConstraintDescriber.DescribeCallOn(writer, this.Method, this.argumentConstraints);

        /// <summary>
        /// Matches the specified call against the expression.
        /// </summary>
        /// <param name="call">The call to match.</param>
        /// <returns>True if the call is matched by the expression.</returns>
        public virtual bool Matches(IFakeObjectCall call)
        {
            Guard.AgainstNull(call, nameof(call));

            return this.InvokesSameMethodOnTarget(call.FakedObject.GetType(), call.Method, this.Method)
                && this.ArgumentsMatches(call.Arguments);
        }

        public virtual void UsePredicateToValidateArguments(Func<ArgumentCollection, bool> predicate)
        {
            this.argumentsPredicate = predicate;
            this.argumentConstraints = this.argumentConstraints.Select(a => new PredicatedArgumentConstraint()).ToArray();
            this.useExplicitArgumentsPredicate = true;
        }

        public Func<IFakeObjectCall, ICollection<object?>> GetOutAndRefParametersValueProducer()
        {
            IList<object?>? values = null;

            foreach (var argumentConstraint in this.argumentConstraints)
            {
                if (argumentConstraint is IArgumentValueProvider valueProvidingConstraint)
                {
                    if (values is null)
                    {
                        values = new List<object?>();
                    }

                    values.Add(valueProvidingConstraint.Value);
                }
            }

            return values is null ? BuildableCallRule.DefaultOutAndRefParametersValueProducer : call => values!;
        }

        private bool InvokesSameMethodOnTarget(Type type, MethodInfo first, MethodInfo second)
        {
            return this.methodInfoManager.WillInvokeSameMethodOnTarget(type, first, second);
        }

        private bool ArgumentsMatches(ArgumentCollection argumentCollection)
        {
            try
            {
                return this.argumentsPredicate(argumentCollection);
            }
            catch (Exception ex) when (this.useExplicitArgumentsPredicate && !(ex is FakeConfigurationException))
            {
                throw new UserCallbackException(ExceptionMessages.UserCallbackThrewAnException("Arguments predicate"), ex);
            }
        }

        private bool ArgumentsMatchesArgumentConstraints(ArgumentCollection argumentCollection)
        {
            for (int i = 0; i < argumentCollection.Count; ++i)
            {
                if (!this.argumentConstraints[i].IsValid(argumentCollection[i]))
                {
                    return false;
                }
            }

            return true;
        }

        private class PredicatedArgumentConstraint
            : IArgumentConstraint
        {
            public bool IsValid(object? argument)
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
