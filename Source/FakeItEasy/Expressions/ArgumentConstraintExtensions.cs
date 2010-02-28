namespace FakeItEasy.Expressions
{
    using System;

    /// <summary>
    /// Provides extension methods for the ArgumentConstraint(T) class.
    /// </summary>
    public static class ArgumentConstraintExtensions
    {
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
    }
}
