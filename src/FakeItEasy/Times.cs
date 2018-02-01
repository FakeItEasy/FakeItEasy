namespace FakeItEasy
{
    using System.Diagnostics.CodeAnalysis;
    using FakeItEasy.Core;

    /// <summary>
    /// Helps define the number of times to expect a faked call to have occurred.
    /// Can be used to indicate whether the call must have occurred exactly the specified number of
    /// times, at least the specified number of times, or at most the specified number of times
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1623:PropertySummaryDocumentationMustMatchAccessors", Justification = "Fluent API.")]
    public abstract class Times
    {
        private Times()
        {
        }

        /// <summary>
        /// The call must have happened exactly the specified number of times.
        /// </summary>
        public static Times Exactly { get; } = new TimesExactly();

        /// <summary>
        /// The call must have happened at least the specified number of times.
        /// </summary>
        public static Times OrMore { get; } = new TimesOrMore();

        /// <summary>
        /// The call must have happened at most the specified number of times.
        /// </summary>
        public static Times OrLess { get; } = new TimesOrLess();

        internal abstract CallCountConstraint ToCallCountConstraint(int numberOfTimes);

        private class TimesExactly : Times
        {
            internal override CallCountConstraint ToCallCountConstraint(int numberOfTimes)
            {
                switch (numberOfTimes)
                {
                    case 0:
                        return new CallCountConstraint(n => n == 0, "never");
                    case 1:
                        return new CallCountConstraint(n => n == 1, "exactly once");
                    case 2:
                        return new CallCountConstraint(n => n == 2, "exactly twice");
                    default:
                        return new CallCountConstraint(n => n == numberOfTimes, $"exactly {numberOfTimes} times");
                }
            }
        }

        private class TimesOrMore : Times
        {
            internal override CallCountConstraint ToCallCountConstraint(int numberOfTimes)
            {
                switch (numberOfTimes)
                {
                    case 1:
                        return new CallCountConstraint(n => n >= 1, "at least once");
                    case 2:
                        return new CallCountConstraint(n => n >= 2, "at least twice");
                    default:
                        return new CallCountConstraint(n => n >= numberOfTimes, $"at least {numberOfTimes} times");
                }
            }
        }

        private class TimesOrLess : Times
        {
            internal override CallCountConstraint ToCallCountConstraint(int numberOfTimes)
            {
                switch (numberOfTimes)
                {
                    case 0:
                        return new CallCountConstraint(n => n <= 0, "never");
                    case 1:
                        return new CallCountConstraint(n => n <= 1, "no more than once");
                    case 2:
                        return new CallCountConstraint(n => n <= 2, "no more than twice");
                    default:
                        return new CallCountConstraint(n => n <= numberOfTimes, $"no more than {numberOfTimes} times");
                }
            }
        }
    }
}
