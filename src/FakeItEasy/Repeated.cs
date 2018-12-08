namespace FakeItEasy
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq.Expressions;
    using FakeItEasy.Configuration;
    using FakeItEasy.Core;

    /// <summary>
    /// Provides syntax for specifying the number of times a call must have occurred when asserting on
    /// fake object calls.
    /// </summary>
    /// <example><code>A.CallTo(() => foo.Bar()).MustHaveHappened(Repeated.Exactly.Once);</code></example>
    [Obsolete("Assertions using the Repeated class will be removed in version 6.0.0. Use variants of MustHaveHappened that specify the number of calls instead.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1623:PropertySummaryDocumentationMustMatchAccessors", Justification = "Fluent API.")]
    public abstract class Repeated
    {
        /// <summary>
        /// Asserts that a call has not happened at all.
        /// </summary>
        [Obsolete("Assertions using the Repeated class will be removed in version 6.0.0. Use MustNotHaveHappened instead.")]
        public static Repeated Never => new NeverRepeated();

        /// <summary>
        /// The call must have happened exactly the number of times that is specified in the next step.
        /// </summary>
        [Obsolete("Assertions using the Repeated class will be removed in version 6.0.0. Use MustHaveHappenedOnceExactly, MustHaveHappenedTwiceExactly, or MustHaveHappened(int, Times) instead.")]
        public static IRepeatSpecification Exactly
        {
            get { return new RepeatSpecification((actual, expected) => actual == expected, "exactly"); }
        }

        /// <summary>
        /// The call must have happened any number of times greater than or equal to the number of times that is specified
        /// in the next step.
        /// </summary>
        [Obsolete("Assertions using the Repeated class will be removed in version 6.0.0. Use MustHaveHappenedOnceOrMore, MustHaveHappenedTwiceOrMore, or MustHaveHappened(int, Times) instead.")]
        public static IRepeatSpecification AtLeast
        {
            get { return new RepeatSpecification((actual, expected) => actual >= expected, "at least"); }
        }

        /// <summary>
        /// The call must have happened any number of times less than or equal to the number of times that is specified
        /// in the next step.
        /// </summary>
        [Obsolete("Assertions using the Repeated class will be removed in version 6.0.0. Use MustHaveHappenedOnceOrLess, MustHaveHappenedTwiceOrLess, or MustHaveHappened(int, Times) instead.")]
        public static IRepeatSpecification NoMoreThan
        {
            get { return new RepeatSpecification((actual, expected) => actual <= expected, "no more than"); }
        }

        /// <summary>
        /// Specifies that a call must have been repeated a number of times
        /// that is validated by the specified repeatValidation argument.
        /// </summary>
        /// <param name="repeatValidation">A predicate that specifies the number of times
        /// a call must have been made.</param>
        /// <returns>A Repeated-instance.</returns>
        [Obsolete("Assertions using the Repeated class will be removed in version 6.0.0. Use MustHaveHappenedANumberOfTimesMatching instead.")]
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is by design when using the Expression-, Action- and Func-types.")]
        public static Repeated Like(Expression<Func<int, bool>> repeatValidation)
        {
            return new ExpressionRepeated(repeatValidation);
        }

        /// <summary>
        /// Creates a <c>CallCountConstraint</c> from this object.
        /// </summary>
        /// <returns>An equivalent <c>CallCountConstraint</c>.</returns>
        internal abstract CallCountConstraint ToCallCountConstraint();

        private class ExpressionRepeated
            : Repeated
        {
            private readonly Expression<Func<int, bool>> repeatValidation;

            public ExpressionRepeated(Expression<Func<int, bool>> repeatValidation)
            {
                this.repeatValidation = repeatValidation;
            }

            internal override CallCountConstraint ToCallCountConstraint()
            {
                return new CallCountConstraint(this.repeatValidation.Compile(), $"the number of times specified by the predicate '{this.repeatValidation}'");
            }
        }

        private class RepeatSpecification : IRepeatSpecification
        {
            private readonly RepeatValidator repeatValidator;
            private readonly string description;

            public RepeatSpecification(RepeatValidator repeatValidator, string description)
            {
                this.repeatValidator = repeatValidator;
                this.description = description;
            }

            public delegate bool RepeatValidator(int actualRepeat, int expectedRepeat);

            public Repeated Once
            {
                get { return new RepeatedWithDescription(x => this.repeatValidator(x, 1), this.description + " once"); }
            }

            public Repeated Twice
            {
                get { return new RepeatedWithDescription(x => this.repeatValidator(x, 2), this.description + " twice"); }
            }

            public Repeated Times(int numberOfTimes)
            {
                return new RepeatedWithDescription(x => this.repeatValidator(x, numberOfTimes), $"{this.description} {numberOfTimes} times");
            }

            private class RepeatedWithDescription : Repeated
            {
                private readonly Func<int, bool> repeatValidator;
                private readonly string description;

                public RepeatedWithDescription(Func<int, bool> repeatValidator, string description)
                {
                    this.repeatValidator = repeatValidator;
                    this.description = description;
                }

                internal override CallCountConstraint ToCallCountConstraint()
                {
                    return new CallCountConstraint(this.repeatValidator, this.description);
                }
            }
        }

        private class NeverRepeated : Repeated
        {
            internal override CallCountConstraint ToCallCountConstraint()
            {
                return new CallCountConstraint(n => n == 0, this.ToString());
            }
        }
    }
}
