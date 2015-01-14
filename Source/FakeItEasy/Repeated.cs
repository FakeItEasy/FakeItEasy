namespace FakeItEasy
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq.Expressions;
    using FakeItEasy.Configuration;

    /// <summary>
    /// Provides syntax for specifying the number of times a call must have been repeated when asserting on 
    /// fake object calls.
    /// </summary>
    /// <example><code>A.CallTo(() => foo.Bar()).Assert(Happened.Once.Exactly);</code></example>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1623:PropertySummaryDocumentationMustMatchAccessors", Justification = "Fluent API.")]
    public abstract class Repeated
    {
        /// <summary>
        /// Asserts that a call has not happened at all.
        /// </summary>
        public static Repeated Never
        {
            get { return new NeverRepeated(); }
        }

        /// <summary>
        /// The call must have happened exactly the number of times that is specified in the next step.
        /// </summary>
        public static IRepeatSpecification Exactly
        {
            get { return new RepeatSpecification((actual, expected) => actual == expected, "exactly"); }
        }

        /// <summary>
        /// The call must have happened any number of times greater than or equal to the number of times that is specified
        /// in the next step.
        /// </summary>
        public static IRepeatSpecification AtLeast
        {
            get { return new RepeatSpecification((actual, expected) => actual >= expected, "at least"); }
        }

        /// <summary>
        /// The call must have happened any number of times less than or equal to the number of times that is specified
        /// in the next step.
        /// </summary>
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
                return new RepeatedWithDescription(x => this.repeatValidator(x, numberOfTimes), "{0} {1} times".FormatInvariant(this.description, numberOfTimes));
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

                public override string ToString()
                {
                    return this.description;
                }

                internal override bool Matches(int repeat)
                {
                    return this.repeatValidator(repeat);
                }
            }
        }

        private class NeverRepeated : Repeated
        {
            public override string ToString()
            {
                return "never";
            }
            
            internal override bool Matches(int repeat)
            {
                return repeat == 0;
            }
        }
    }
}