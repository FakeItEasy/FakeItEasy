namespace FakeItEasy.Core
{
    /// <summary>
    /// Keeps track of metadata for interceptions.
    /// </summary>
    internal class CallRuleMetadata
    {
        private CallRuleMetadata(IFakeObjectCallRule rule, int calledNumberOfTimes)
        {
            this.Rule = rule;
            this.CalledNumberOfTimes = calledNumberOfTimes;
        }

        /// <summary>
        /// Gets the number of times the rule has been used.
        /// </summary>
        public int CalledNumberOfTimes { get; private set; }

        /// <summary>
        /// Gets the rule this metadata object is tracking.
        /// </summary>
        internal IFakeObjectCallRule Rule { get; }

        /// <summary>
        /// Creates a CallRuleMetadata representing a rule that has been called once.
        /// </summary>
        /// <param name="rule">The rule.</param>
        /// <returns>The new CallRuleMetadata instance.</returns>
        public static CallRuleMetadata CalledOnce(IFakeObjectCallRule rule) => new CallRuleMetadata(rule, 1);

        /// <summary>
        /// Creates a CallRuleMetadata representing a rule that has not yet been called.
        /// </summary>
        /// <param name="rule">The rule.</param>
        /// <returns>The new CallRuleMetadata instance.</returns>
        public static CallRuleMetadata NeverCalled(IFakeObjectCallRule rule) => new CallRuleMetadata(rule, 0);

        /// <summary>
        /// Gets whether the rule has been called the number of times specified or not.
        /// </summary>
        /// <returns>True if the rule has not been called the number of times specified.</returns>
        public bool HasNotBeenCalledSpecifiedNumberOfTimes()
        {
            return this.Rule.NumberOfTimesToCall is null || this.CalledNumberOfTimes < this.Rule.NumberOfTimesToCall.Value;
        }

        /// <summary>
        /// Records that this rule has been called an additional time.
        /// </summary>
        public void RecordCall() => ++this.CalledNumberOfTimes;

        public override string? ToString()
        {
            return this.Rule.ToString();
        }

        public CallRuleMetadata GetSnapshot()
        {
            var rule = this.Rule is IStatefulFakeObjectCallRule statefulRule
                ? statefulRule.GetSnapshot()
                : this.Rule;
            return new CallRuleMetadata(rule, this.CalledNumberOfTimes);
        }
    }
}
