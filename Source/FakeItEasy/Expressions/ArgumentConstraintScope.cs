namespace FakeItEasy.Expressions
{
    using System.Linq.Expressions;
    using FakeItEasy.Configuration;
    using System;

    public abstract class ArgumentConstraintScope<T>
        : IHideObjectMembers
    {
        /// <summary>
        /// Gets a value indicating if the argument is valid in the context
        /// of this ArgumentValidations-object.
        /// </summary>
        /// <param name="argument">The argument to validate.</param>
        /// <returns>True if the argument is valid.</returns>
        internal abstract bool IsValid(T argument);

        /// <summary>
        /// Gets a value indicating if the result of a child validators IsValid-call
        /// is valid in the context of this ArgumentValidations.
        /// </summary>
        /// <param name="result">The result of the call to the child validators IsValid-method.</param>
        /// <returns>True if the result is valid.</returns>
        internal abstract bool ResultOfChildValidatorIsValid(bool result);

        /// <summary>
        /// Reverse the is valid of the validator that comes after the not, so that
        /// if the validator is valid the result is false and if the validator is not
        /// valid the result is true.
        /// </summary>
        public ArgumentConstraintScope<T> Not
        {
            get
            {
                return new NotArgumentValidatorScope<T>(this);
            }
        }

        /// <summary>
        /// Gets an ArgumentValidator that is valid if the specified predicate returns true.
        /// </summary>
        /// <param name="predicate">A predicate that validates the argument.</param>
        /// <returns>An ArgumentValidator.</returns>
        public virtual ArgumentConstraint<T> Matches(Func<T, bool> predicate)
        {
            return ArgumentConstraint.Create(this, predicate, "Predicate");
        }

        /// <summary>
        /// Gets an argument validator that validates that the argument is
        /// of the specified type or any derivative.
        /// </summary>
        /// <typeparam name="TType">The type to check for.</typeparam>
        /// <returns>An argument validator.</returns>
        public virtual ArgumentConstraint<T> IsInstanceOf<TType>()
        {
            return ArgumentConstraint.Create(this, x => x is TType, "Instance of {0}".FormatInvariant(typeof(TType)));
        }

        /// <summary>
        /// The base implementation returns the empty string.
        /// </summary>
        /// <returns>Empty string.</returns>
        public override string ToString()
        {
            return string.Empty;
        }
    }
}