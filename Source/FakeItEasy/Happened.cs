namespace FakeItEasy
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using FakeItEasy.Api;

    /// <summary>
    /// Provides syntax for specifying the number of times a call must have been repeated when asserting on 
    /// fake object calls.
    /// </summary>
    /// <example>A.CallTo(() => foo.Bar()).Assert(Happened.Once.Exactly);</example>
    public abstract class Happened
    {
        /// <summary>
        /// Asserts that a call has not happened at all.
        /// </summary>
        public static Happened Never
        {
            get
            {
                return new LowerBoundHappened(0).Exactly;
            }
        }

        /// <summary>
        /// Asserts that a call has happened once or more.
        /// </summary>
        public static LowerBoundHappened Once
        {
            get
            {
                return new LowerBoundHappened(1);
            }
        }

        /// <summary>
        /// Asserts that a call has happend twice or more.
        /// </summary>
        public static LowerBoundHappened Twice
        {
            get
            {
                return new LowerBoundHappened(2);
            }
        }

        /// <summary>
        /// Asserts that a call has happened the specified number of times
        /// or more.
        /// </summary>
        /// <param name="numberOfTimes">The number of times the call must have happened.</param>
        /// <returns>A HappenedNoUpperBound instance.</returns>
        public static LowerBoundHappened Times(int numberOfTimes)
        {
            return new LowerBoundHappened(numberOfTimes);
        }

        /// <summary>
        /// When implemented gets a value indicating if the repeat is matched
        /// by the Happened-instance.
        /// </summary>
        /// <param name="repeat">The repeat of a call.</param>
        /// <returns>True if the repeat is a match.</returns>
        internal abstract bool Matches(int repeat);

        internal static Happened ConvertFromExpression(Expression<Func<int, bool>> repeatValidation)
        {
            return new ExpressionHappened(repeatValidation);
        }

        private class ExpressionHappened
            : Happened
        {
            private Expression<Func<int, bool>> repeatValidation;

            public ExpressionHappened(Expression<Func<int, bool>> repeatValidation)
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
