namespace FakeItEasy.Expressions
{
    using System;
    using System.Linq.Expressions;
    using Configuration;
    using Core;

    /// <summary>
    /// An implementation of the <see cref="IFakeObjectCallRule" /> interface that uses
    /// expressions for evaluating if the rule is applicable to a specific call.
    /// </summary>
    internal class ExpressionCallRule
        : BuildableCallRule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionCallRule"/> class.
        /// </summary>
        /// <param name="expressionMatcher">The expression matcher to use.</param>
        public ExpressionCallRule(ExpressionCallMatcher expressionMatcher)
        {
            Guard.AgainstNull(expressionMatcher, "expressionMatcher");

            this.ExpressionMatcher = expressionMatcher;
            this.OutAndRefParametersValueProducer = expressionMatcher.GetOutAndRefParametersValueProducer();
        }
        
        /// <summary>
        /// Handles the instantiation of ExpressionCallRule instance.
        /// </summary>
        /// <param name="callSpecification">An expression specifying the call.</param>
        /// <returns>A rule instance.</returns>
        public delegate ExpressionCallRule Factory(LambdaExpression callSpecification);

        public override string DescriptionOfValidCall
        {
            get { return this.ExpressionMatcher.DescriptionOfMatchingCall; }
        }

        private ExpressionCallMatcher ExpressionMatcher { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.DescriptionOfValidCall;
        }

        public override void UsePredicateToValidateArguments(Func<ArgumentCollection, bool> argumentsPredicate)
        {
            this.ExpressionMatcher.UsePredicateToValidateArguments(argumentsPredicate);
        }

        protected override bool OnIsApplicableTo(IFakeObjectCall fakeObjectCall)
        {
            return this.ExpressionMatcher.Matches(fakeObjectCall);
        }
    }
}