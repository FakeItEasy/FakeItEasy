namespace FakeItEasy
{
    using System;
    using System.ComponentModel;
    using System.Linq.Expressions;

    public static class Happened
    {
        /// <summary>
        /// Asserts that a call has not happened at all.
        /// </summary>
        [Obsolete, EditorBrowsable(EditorBrowsableState.Never)]
        public static Repeated Never
        {
            get
            {
                return Repeated.Never;
            }
        }

        /// <summary>
        /// Asserts that a call has happened once or more.
        /// </summary>
        [Obsolete, EditorBrowsable(EditorBrowsableState.Never)]
        public static LowerBoundRepeated Once
        {
            get
            {
                return Repeated.Once;
            }
        }

        /// <summary>
        /// Asserts that a call has happend twice or more.
        /// </summary>
        [Obsolete, EditorBrowsable(EditorBrowsableState.Never)]
        public static LowerBoundRepeated Twice
        {
            get
            {
                return Repeated.Twice;
            }
        }

        /// <summary>
        /// Asserts that a call has happened the specified number of times
        /// or more.
        /// </summary>
        /// <param name="numberOfTimes">The number of times the call must have happened.</param>
        /// <returns>A HappenedNoUpperBound instance.</returns>
        [Obsolete, EditorBrowsable(EditorBrowsableState.Never)]
        public static LowerBoundRepeated Times(int numberOfTimes)
        {
            return Repeated.Times(numberOfTimes);
        }

        /// <summary>
        /// Specifies that a call must have been repeated a number of times
        /// that is validated by the specified repeatValidation argument.
        /// </summary>
        /// <param name="repeatValidation">A predicate that specifies the number of times
        /// a call must have been made.</param>
        /// <returns>A Repeated-instance.</returns>
        [Obsolete, EditorBrowsable(EditorBrowsableState.Never)]
        public static Repeated Like(Expression<Func<int, bool>> repeatValidation)
        {
            return Repeated.Like(repeatValidation);
        }
    }
}
