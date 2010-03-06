namespace FakeItEasy.Expressions
{
    using System.Linq.Expressions;
    using FakeItEasy.Configuration;
    using FakeItEasy.Expressions;
    using FakeItEasy.Core;

    /// <summary>
    /// An implementation of the <see cref="IFakeObjectCallRule" /> interface that uses
    /// expressions for evaluating if the rule is applicable to a specific call.
    /// </summary>
    internal class ExpressionCallRule
        : BuildableCallRule
    {
        #region Fields
        
        #endregion

        #region Construction
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionCallRule"/> class.
        /// </summary>
        /// <param name="callSpecification">The call specification.</param>
        public ExpressionCallRule(ExpressionCallMatcher expressionMatcher)
        {
            Guard.IsNotNull(expressionMatcher, "expressionMatcher");

            this.ExpressionMatcher = expressionMatcher;
        } 
        #endregion

        #region Delegates
        /// <summary>
        /// Handles the instantiation of ExpressionCallRule instance.
        /// </summary>
        /// <param name="callSpecification">An expression specifying the call.</param>
        /// <returns>A rule instance.</returns>
        public delegate ExpressionCallRule Factory(LambdaExpression callSpecification);
        #endregion

        #region Properties
        /// <summary>
        /// Gets the expression matcher used by this rule.
        /// </summary>
        private ExpressionCallMatcher ExpressionMatcher { get; set; }
        #endregion

        #region Methods
        protected override bool OnIsApplicableTo(IFakeObjectCall fakeObjectCall)
        {
            return this.ExpressionMatcher.Matches(fakeObjectCall);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.ExpressionMatcher.ToString();
        }

        public override void UsePredicateToValidateArguments(System.Func<ArgumentCollection, bool> argumentsPredicate)
        {
            this.ExpressionMatcher.UsePredicateToValidateArguments(argumentsPredicate);
        }
        #endregion
    }
}