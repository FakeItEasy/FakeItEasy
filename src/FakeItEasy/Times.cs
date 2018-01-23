namespace FakeItEasy
{
    using System.Diagnostics.CodeAnalysis;

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

        internal abstract Repeated ToRepeated(int numberOfTimes);

        private class TimesExactly : Times
        {
            internal override Repeated ToRepeated(int numberOfTimes)
            {
                return Repeated.Exactly.Times(numberOfTimes);
            }
        }

        private class TimesOrMore : Times
        {
            internal override Repeated ToRepeated(int numberOfTimes)
            {
                return Repeated.AtLeast.Times(numberOfTimes);
            }
        }

        private class TimesOrLess : Times
        {
            internal override Repeated ToRepeated(int numberOfTimes)
            {
                return Repeated.NoMoreThan.Times(numberOfTimes);
            }
        }
    }
}
