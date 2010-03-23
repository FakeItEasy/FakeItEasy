namespace FakeItEasy.Expressions
{
    using System;
    using FakeItEasy.Core;

    /// <summary>
    /// Provides static methods for the ArgumentConstraint{T} class.
    /// </summary>
    public static class ArgumentConstraint
    {
        /// <summary>
        /// Creates a new constraint.
        /// </summary>
        /// <param name="scope">The scope of the constraint.</param>
        /// <param name="predicate">A predicate that's used to validate arguments.</param>
        /// <param name="description">A description of the constraint.</param>
        /// <returns>An ArgumentConstraint.</returns>
        public static ArgumentConstraint<T> Create<T>(ArgumentConstraintScope<T> scope, Func<T, bool> predicate, string description)
        {
            Guard.IsNotNull(scope, "scope");
            Guard.IsNotNull(predicate, "predicate");
            Guard.IsNotNullOrEmpty(description, "description");

            return new PredicateArgumentConstraint<T>(scope) { Validation = predicate, DescriptionField = description };
        }

        /// <summary>
        /// Allows you to combine the current constraint with another constraint, where only
        /// one of them has to be valid.
        /// </summary>
        /// <param name="constraint">The constraint to extend.</param>
        /// <param name="otherConstraint">A delegate that returns the constraint to combine with.</param>
        /// <returns>A combined constraint.</returns>
        public static ArgumentConstraint<T> Or<T>(this ArgumentConstraint<T> constraint, Func<ArgumentConstraintScope<T>, ArgumentConstraint<T>> otherConstraint)
        {
            Guard.IsNotNull(constraint, "constraint");
            Guard.IsNotNull(otherConstraint, "otherConstraint");

            return constraint.Or(otherConstraint.Invoke(A<T>.That));
        }

        private class PredicateArgumentConstraint<T>
           : ArgumentConstraint<T>
        {
            public PredicateArgumentConstraint(ArgumentConstraintScope<T> scope)
                : base(scope)
            {
            }

            public Func<T, bool> Validation { get; set; }

            public string DescriptionField { get; set; }

            protected override string Description
            {
                get { return this.DescriptionField; }
            }

            protected override bool Evaluate(T value)
            {
                return this.Validation.Invoke(value);
            }
        }
    }

    /// <summary>
    /// An object that can determine if an argument of the type T is valid or not.
    /// </summary>
    /// <typeparam name="T">The type of argument to validate.</typeparam>
    public abstract class ArgumentConstraint<T>
        : IArgumentConstraint
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArgumentConstraint&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="scope">The scope of the constraint.</param>
        protected ArgumentConstraint(ArgumentConstraintScope<T> scope)
        {
            this.Scope = scope;
        }

        /// <summary>
        /// A constraint for an interface type can not be implicitly converted to the
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
        /// Produces a new scope to combine the current constraint with another constraint.
        /// </summary>
        public ArgumentConstraintScope<T> And
        {
            get
            {
                return new AndValidations() { ParentConstraint = this };
            }
        }

        /// <summary>
        /// The scope of the constraint.
        /// </summary>
        internal ArgumentConstraintScope<T> Scope
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the full description of the constraint, together with any parent validations
        /// and constraints descriptions.
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
        /// Gets a description of this constraint.
        /// </summary>
        protected abstract string Description { get; }

        /// <summary>
        /// Converst a constraint to the the type of the constrained argument.
        /// </summary>
        /// <param name="constraint"></param>
        /// <returns></returns>
        public static implicit operator T(ArgumentConstraint<T> constraint)
        {
            return constraint.Argument;
        }

        /// <summary>
        /// Allows you to combine the current constraint with another constraint, where only
        /// one of them has to be valid.
        /// </summary>
        /// <param name="otherConstraint">The constraint to combine with.</param>
        /// <returns>A combined constraint.</returns>
        public ArgumentConstraint<T> Or(ArgumentConstraint<T> otherConstraint)
        {
            return new OrConstraint(this, otherConstraint);
        }

        /// <summary>
        /// Gets a value indicating if the value is valid.
        /// </summary>
        /// <param name="value">The value to evaluate.</param>
        /// <returns>True if the value is valid.</returns>
        public bool IsValid(T value)
        {
            return this.Scope.IsValid(value) && this.Scope.ResultOfChildConstraintIsValid(this.Evaluate(value));
        }

        /// <summary>
        /// Gets a formatted description of this constraint.
        /// </summary>
        /// <returns>A description of this constraint.</returns>
        public override string ToString()
        {
            return string.Concat("<", this.FullDescription, ">");
        }

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
        /// When implemented evaluates if the argument is valid.
        /// </summary>
        /// <param name="value">The value to evaluate.</param>
        /// <returns>True if the argument is valid.</returns>
        protected abstract bool Evaluate(T value);

        private class OrConstraint
            : ArgumentConstraint<T>
        {
            private ArgumentConstraint<T> first;
            private ArgumentConstraint<T> second;

            public OrConstraint(ArgumentConstraint<T> first, ArgumentConstraint<T> second)
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
            : ArgumentConstraintScope<T>
        {
            public ArgumentConstraint<T> ParentConstraint { get; set; }

            public override string ToString()
            {
                return string.Concat(this.ParentConstraint.FullDescription, " and");
            }

            internal override bool IsValid(T argument)
            {
                return this.ParentConstraint.IsValid(argument);
            }

            internal override bool ResultOfChildConstraintIsValid(bool result)
            {
                return result;
            }
        }
    }
}