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
                        return new CallCountConstraint(n => n == 1, "once exactly");
                    case 2:
                        return new CallCountConstraint(n => n == 2, "twice exactly");
                    default:
                        return new CallCountConstraint(n => n == numberOfTimes, $"{numberOfTimes} times exactly");
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
                        return new CallCountConstraint(n => n >= 1, "once or more");
                    case 2:
                        return new CallCountConstraint(n => n >= 2, "twice or more");
                    default:
                        return new CallCountConstraint(n => n >= numberOfTimes, $"{numberOfTimes} times or more");
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
                        return new CallCountConstraint(n => n <= 1, "once or less");
                    case 2:
                        return new CallCountConstraint(n => n <= 2, "twice or less");
                    default:
                        return new CallCountConstraint(n => n <= numberOfTimes, $"{numberOfTimes} times or less");
                }
            }
        }
    }
}
