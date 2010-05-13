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
            get
            {
                return new LowerBoundRepeated(0).Exactly;
            }
        }

        /// <summary>
        /// Asserts that a call has happened once or more.
        /// </summary>
        public static LowerBoundRepeated Once
        {
            get
            {
                return new LowerBoundRepeated(1);
            }
        }

        /// <summary>
        /// Asserts that a call has happend twice or more.
        /// </summary>
        public static LowerBoundRepeated Twice
        {
            get
            {
                return new LowerBoundRepeated(2);
            }
        }

        /// <summary>
        /// Asserts that a call has happened the specified number of times
        /// or more.
        /// </summary>
        /// <param name="numberOfTimes">The number of times the call must have happened.</param>
        /// <returns>A HappenedNoUpperBound instance.</returns>
        public static LowerBoundRepeated Times(int numberOfTimes)
        {
            return new LowerBoundRepeated(numberOfTimes);
        }

        /// <summary>
        /// When implemented gets a value indicating if the repeat is matched
        /// by the Happened-instance.
        /// </summary>
        /// <param name="repeat">The repeat of a call.</param>
        /// <returns>True if the repeat is a match.</returns>
        internal abstract bool Matches(int repeat);

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

        private class ExpressionRepeated
            : Repeated
        {
            private Expression<Func<int, bool>> repeatValidation;

            public ExpressionRepeated(Expression<Func<int, bool>> repeatValidation)
            {
                this.repeatValidation = repeatValidation;
            }

            internal override bool Matches(int repeat)
            {
                return this.repeatValidation.Compile().Invoke(repeat);
            }

            public override string ToString()
            {
                return "the number of times specified by the predicate '{0}'".FormatInvariant(this.repeatValidation.ToString());
            }
        }
    }
}
