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
                switch (numberOfTimes)
                {
                    case 0:
                        return Repeated.Never;
                    case 1:
                        return Repeated.Exactly.Once;
                    case 2:
                        return Repeated.Exactly.Twice;
                    default:
                        return Repeated.Exactly.Times(numberOfTimes);
                }
            }
        }

        private class TimesOrMore : Times
        {
            internal override Repeated ToRepeated(int numberOfTimes)
            {
                switch (numberOfTimes)
                {
                    case 1:
                        return Repeated.AtLeast.Once;
                    case 2:
                        return Repeated.AtLeast.Twice;
                    default:
                        return Repeated.AtLeast.Times(numberOfTimes);
                }
            }
        }

        private class TimesOrLess : Times
        {
            internal override Repeated ToRepeated(int numberOfTimes)
            {
                switch (numberOfTimes)
                {
                    case 0:
                        return Repeated.Never;
                    case 1:
                        return Repeated.NoMoreThan.Once;
                    case 2:
                        return Repeated.NoMoreThan.Twice;
                    default:
                        return Repeated.NoMoreThan.Times(numberOfTimes);
                }
            }
        }
    }
}
