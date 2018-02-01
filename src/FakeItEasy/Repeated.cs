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
    /// <remarks>
    /// Assertions using the <c>Repeated</c> class are being phased out and will be deprecated in
    /// version 5.0.0 and removed in version 6.0.0.
    /// Prefer <see cref="IAssertConfiguration"/> methods and <see cref="AssertConfigurationExtensions"/>
    /// extension methods that do not use <c>Repeated</c>.
    /// </remarks>
    /// <example><code>A.CallTo(() => foo.Bar()).MustHaveHappened(Repeated.Exactly.Once);</code></example>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1623:PropertySummaryDocumentationMustMatchAccessors", Justification = "Fluent API.")]
    public abstract class Repeated
    {
        /// <summary>
        /// Asserts that a call has not happened at all.
        /// </summary>
        /// <remarks>
        /// Assertions using the <c>Repeated</c> class are being phased out and will be deprecated in
        /// version 5.0.0 and removed in version 6.0.0.
        /// Prefer <see cref="AssertConfigurationExtensions.MustNotHaveHappened"/>.
        /// </remarks>
        public static Repeated Never => new NeverRepeated();

        /// <summary>
        /// The call must have happened exactly the number of times that is specified in the next step.
        /// </summary>
        /// <remarks>
        /// Assertions using the <c>Repeated</c> class are being phased out and will be deprecated in
        /// version 5.0.0 and removed in version 6.0.0.
        /// Prefer <see cref="AssertConfigurationExtensions.MustHaveHappenedOnceExactly"/>,
        /// <see cref="AssertConfigurationExtensions.MustHaveHappenedTwiceExactly"/>, or
        /// <see cref="IAssertConfiguration.MustHaveHappened(Int32, Times)"/> (using <see cref="Times.Exactly"/>).
        /// </remarks>
        public static IRepeatSpecification Exactly
        {
            get { return new RepeatSpecification((actual, expected) => actual == expected, "exactly"); }
        }

        /// <summary>
        /// The call must have happened any number of times greater than or equal to the number of times that is specified
        /// in the next step.
        /// </summary>
        /// <remarks>
        /// Assertions using the <c>Repeated</c> class are being phased out and will be deprecated in
        /// version 5.0.0 and removed in version 6.0.0.
        /// Prefer <see cref="AssertConfigurationExtensions.MustHaveHappenedOnceOrMore"/>,
        /// <see cref="AssertConfigurationExtensions.MustHaveHappenedTwiceOrMore"/>, or
        /// <see cref="IAssertConfiguration.MustHaveHappened(Int32, Times)"/> (using <see cref="Times.OrMore"/>).
        /// </remarks>
        public static IRepeatSpecification AtLeast
        {
            get { return new RepeatSpecification((actual, expected) => actual >= expected, "at least"); }
        }

        /// <summary>
        /// The call must have happened any number of times less than or equal to the number of times that is specified
        /// in the next step.
        /// </summary>
        /// <remarks>
        /// Assertions using the <c>Repeated</c> class are being phased out and will be deprecated in
        /// version 5.0.0 and removed in version 6.0.0.
        /// Prefer <see cref="AssertConfigurationExtensions.MustHaveHappenedOnceOrLess"/>,
        /// <see cref="AssertConfigurationExtensions.MustHaveHappenedTwiceOrLess"/>, or
        /// <see cref="IAssertConfiguration.MustHaveHappened(Int32, Times)"/> (using <see cref="Times.OrLess"/>).
        /// </remarks>
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
        /// <remarks>
        /// Assertions using the <c>Repeated</c> class are being phased out and will be deprecated in
        /// version 5.0.0 and removed in version 6.0.0.
        /// <see cref="IAssertConfiguration.MustHaveHappenedANumberOfTimesMatching"/>.
        /// </remarks>
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
