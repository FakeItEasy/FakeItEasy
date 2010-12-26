namespace FakeItEasy.Core
{
    using System;

    /// <summary>
    /// Keeps track of metadata for interceptions.
    /// </summary>
    [Serializable]
    internal class CallRuleMetadata
    {
        /// <summary>
        /// Gets or sets the number of times the rule has been used.
        /// </summary>
        public int CalledNumberOfTimes { get; set; }

        /// <summary>
        /// Gets or sets the rule this metadata object is tracking.
        /// </summary>
        internal IFakeObjectCallRule Rule { get; set; }

        /// <summary>
        /// Gets whether the rule has been called the number of times specified or not.
        /// </summary>
        /// <returns>True if the rule has not been called the number of times specified.</returns>
        public bool HasNotBeenCalledSpecifiedNumberOfTimes()
        {
            return this.Rule.NumberOfTimesToCall == null || this.CalledNumberOfTimes < this.Rule.NumberOfTimesToCall.Value;
        }

        public override string ToString()
        {
            return this.Rule.ToString();
        }
    }
}