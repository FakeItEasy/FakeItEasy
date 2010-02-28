namespace FakeItEasy.Expressions
{
    using System;
    using System.ComponentModel;
    using FakeItEasy.Api;
    using FakeItEasy.Configuration;

    /// <summary>
    /// An object that can determine if an argument of the type T is valid or not.
    /// </summary>
    /// <typeparam name="T">The type of argument to validate.</typeparam>
    public abstract class ArgumentValidator<T>
        : IArgumentConstraint, IHideObjectMembers
    {
        protected ArgumentValidator(ArgumentValidatorScope<T> scope)
        {
            this.Scope = scope;
        }

        /// <summary>
        /// The scope of the validator.
        /// </summary>
        internal ArgumentValidatorScope<T> Scope
        {
            get;
            private set;
        }

        /// <summary>
        /// A validator for an interface type can not be implicitly converted to the
        /// argument type, use this property in these cases.
        /// </summary>
        /// <example>
        /// A.CallTo(() => foo.Bar(A&lt;string&gt;Ignored, A&lt;IComparable%gt;.Ignored.Argument)).Throws(new Exception());
        /// </example>
        public T Argument
        {
            get
            {
                return default(T);
            }
        }

        /// <summary>
        /// Produces a new scope to combine the current validator with another validator.
        /// </summary>
        public ArgumentValidatorScope<T> And
        {
            get
            {
                return new AndValidations() { ParentValidator = this };
            }
        }

        /// <summary>
        /// Gets a description of this validator.
        /// </summary>
        protected abstract string Description { get; }

        /// <summary>
        /// Gets a value indicating if the value is valid.
        /// </summary>
        /// <param name="value">The value to evaluate.</param>
        /// <returns>True if the value is valid.</returns>
        public bool IsValid(T value)
        {
            return this.Scope.IsValid(value) && this.Scope.ResultOfChildValidatorIsValid(this.Evaluate(value));
        }

        /// <summary>
        /// Gets the full description of the validator, together with any parent validations
        /// and validators descriptions.
        /// </summary>
        internal string FullDescription
        {
            get
            {
                var validationsDescription = this.Scope.ToString();

                if (string.IsNullOrEmpty(validationsDescription))
                {
                    return this.Description;
                }
                else
                {
                    return string.Concat(validationsDescription, " ", this.Description);
                }
            }
        }

        /// <summary>
        /// Allows you to combine the current validator with another validator, where only
        /// one of them has to be valid.
        /// </summary>
        /// <param name="otherValidator">The validator to combine with.</param>
        /// <returns>A combined validator.</returns>
        public ArgumentValidator<T> Or(ArgumentValidator<T> otherValidator)
        {
            return new OrValidator(this, otherValidator);
        }

        /// <summary>
        /// When implemented evaluates if the argument is valid.
        /// </summary>
        /// <param name="value">The value to evaluate.</param>
        /// <returns>True if the argument is valid.</returns>
        protected abstract bool Evaluate(T value);

        /// <summary>
        /// Gets whether the argument is valid.
        /// </summary>
        /// <param name="argument">The argument to validate.</param>
        /// <returns>True if the argument is valid.</returns>
        bool IArgumentConstraint.IsValid(object argument)
        {
            return this.IsValid((T)argument);
        }

        /// <summary>
        /// Gets a formatted description of this validator.
        /// </summary>
        /// <returns>A description of this validator.</returns>
        public override string ToString()
        {
            return string.Concat("<", this.FullDescription, ">");
        }

        /// <summary>
        /// Creates a new validator.
        /// </summary>
        /// <param name="validations">The scope of the validator.</param>
        /// <param name="predicate">A predicate that's used to validate arguments.</param>
        /// <param name="description">A description of the validator.</param>
        /// <returns>An ArgumentValidator.</returns>
        [Obsolete("Use the ArgumentValidator.Create-method (on the non generic ArgumentValidator-class) instead.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static ArgumentValidator<T> Create<T>(ArgumentValidatorScope<T> scope, Func<T, bool> predicate, string description)
        {
            return ArgumentValidator.Create(scope, predicate, description);
        }

        private class OrValidator
            : ArgumentValidator<T>
        {
            private ArgumentValidator<T> first;
            private ArgumentValidator<T> second;

            public OrValidator(ArgumentValidator<T> first, ArgumentValidator<T> second)
                : base(new RootValidations<T>())
            {
                this.first = first;
                this.second = second;
            }

            protected override string Description
            {
                get { return "{0} or ({1})".FormatInvariant(this.first.FullDescription, this.second.FullDescription); }
            }

            protected override bool Evaluate(T value)
            {
                return this.first.IsValid(value) || this.second.IsValid(value);
            }
        }

        private class AndValidations
            : ArgumentValidatorScope<T>
        {
            public ArgumentValidator<T> ParentValidator;

            internal override bool IsValid(T argument)
            {
                return this.ParentValidator.IsValid(argument);
            }

            public override string ToString()
            {
                return string.Concat(this.ParentValidator.FullDescription, " and");
            }

            internal override bool ResultOfChildValidatorIsValid(bool result)
            {
                return result;
            }
        }

        public static implicit operator T(ArgumentValidator<T> validator)
        {
            return validator.Argument;
        }
    }

    /// <summary>
    /// Provides static methods for the ArgumentValidator{T} class.
    /// </summary>
    public static class ArgumentValidator
    {
        private class PredicateArgumentValidator<T>
           : ArgumentValidator<T>
        {
            public PredicateArgumentValidator(ArgumentValidatorScope<T> scope)
                : base(scope)
            {

            }

            public Func<T, bool> Validation;
            public string DescriptionField;

            protected override string Description
            {
                get { return this.DescriptionField; }
            }

            protected override bool Evaluate(T value)
            {
                return this.Validation.Invoke(value);
            }
        }

        /// <summary>
        /// Creates a new validator.
        /// </summary>
        /// <param name="validations">The scope of the validator.</param>
        /// <param name="predicate">A predicate that's used to validate arguments.</param>
        /// <param name="description">A description of the validator.</param>
        /// <returns>An ArgumentValidator.</returns>
        public static ArgumentValidator<T> Create<T>(ArgumentValidatorScope<T> scope, Func<T, bool> predicate, string description)
        {
            Guard.IsNotNull(scope, "scope");
            Guard.IsNotNull(predicate, "predicate");
            Guard.IsNotNullOrEmpty(description, "description");

            return new PredicateArgumentValidator<T>(scope) { Validation = predicate, DescriptionField = description };
        }
    }
}