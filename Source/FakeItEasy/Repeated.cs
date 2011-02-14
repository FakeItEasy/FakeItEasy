namespace FakeItEasy
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq.Expressions;

    /// <summary>
    /// Provides syntax for specifying the number of times a call must have been repeated when asserting on 
    /// fake object calls.
    /// </summary>
    /// <example>A.CallTo(() => foo.Bar()).Assert(Happened.Once.Exactly);</example>
    public abstract class Repeated
    {
        /// <summary>
        /// Asserts that a call has not happened at all.
        /// </summary>
        public static Repeated Never
        {
            get { return new ExactlyRepeatSpecification().Times(0); }
        }

        public static IRepeatSpecification Exactly
        {
            get { return new ExactlyRepeatSpecification(); }
        }

        public static IRepeatSpecification AtLeast
        {
            get { return new AtLeastRepeatSpecification(); }
        }

        public static IRepeatSpecification NoMoreThan
        {
            get { return new NoMoreThanRepeatSpecification(); }
        }

        private class ExactlyRepeatSpecification : IRepeatSpecification
        {
            public Repeated Once
            {
                get { return new ExpressionRepeated(x => x == 1); }
            }

            public Repeated Twice
            {
                get { return new ExpressionRepeated(x => x == 2); }
            }

            public Repeated Times(int numberOfTimes)
            {
                return new ExpressionRepeated(x => x == numberOfTimes);
            }
        }

        private class AtLeastRepeatSpecification : IRepeatSpecification
        {
            public Repeated Once
            {
                get { return new ExpressionRepeated(x => x >= 1); }
            }

            public Repeated Twice
            {
                get { return new ExpressionRepeated(x => x >= 2); }
            }

            public Repeated Times(int numberOfTimes)
            {
                return new ExpressionRepeated(x => x >= numberOfTimes);
            }
        }

        private class NoMoreThanRepeatSpecification : IRepeatSpecification
        {
            public Repeated Once
            {
                get { return new ExpressionRepeated(x => x <= 1); }
            }

            public Repeated Twice
            {
                get { return new ExpressionRepeated(x => x <= 2); }
            }

            public Repeated Times(int numberOfTimes)
            {
                return new ExpressionRepeated(x => x <= numberOfTimes);
            }
        }

        /// <summary>
        /// Specifies that a call must have been repeated a number of times
        /// that is validated by the specified repeatValidation argument.
        /// </summary>
        /// <param name="repeatValidation">A predicate that specifies the number of times
        /// a call must have been made.</param>
        /// <returns>A Repeated-instance.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is by design when using the Expression-, Action- and Func-types.")]
        public static Repeated Like(Expression<Func<int, bool>> repeatValidation)
        {
            return new ExpressionRepeated(repeatValidation);
        }

        /// <summary>
        /// When implemented gets a value indicating if the repeat is matched
        /// by the Happened-instance.
        /// </summary>
        /// <param name="repeat">The repeat of a call.</param>
        /// <returns>True if the repeat is a match.</returns>
        internal abstract bool Matches(int repeat);

        private class ExpressionRepeated
            : Repeated
        {
            private readonly Expression<Func<int, bool>> repeatValidation;

            public ExpressionRepeated(Expression<Func<int, bool>> repeatValidation)
            {
                this.repeatValidation = repeatValidation;
            }

            public override string ToString()
            {
                return "the number of times specified by the predicate '{0}'".FormatInvariant(this.repeatValidation.ToString());
            }

            internal override bool Matches(int repeat)
            {
                return this.repeatValidation.Compile().Invoke(repeat);
            }
        }
    }
}