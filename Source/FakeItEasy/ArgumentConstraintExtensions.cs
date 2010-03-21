namespace FakeItEasy
{
    using System;
    using System.Collections;
    using FakeItEasy.Expressions;
    using FakeItEasy.Expressions.ArgumentConstraints;

    /// <summary>
    /// Provides validation extension to the Argumentscope{T} class.
    /// </summary>
    public static class ArgumentConstraintExtensions
    {
        /// <summary>
        /// Validates that an argument is null.
        /// </summary>
        /// <typeparam name="T">The type of the argument.</typeparam>
        /// <param name="scope">The scope of the constraint.</param>
        /// <returns>An argument constraint.</returns>
        public static ArgumentConstraint<T> IsNull<T>(this ArgumentConstraintScope<T> scope) where T : class
        {
            return scope.CreateConstraint(x => x == null, "NULL");
        }

        /// <summary>
        /// Validates that the string argument contains the specified text.
        /// </summary>
        /// <param name="scope">The scope of the constraint.</param>
        /// <param name="value">The string the argument string should contain.</param>
        /// <returns>An argument constraint.</returns>
        public static ArgumentConstraint<string> Contains(this ArgumentConstraintScope<string> scope, string value)
        {
            return scope.CreateConstraint(x => x != null && x.Contains(value), "String that contains \"{0}\"", value);
        }

        /// <summary>
        /// Validates that the collection argument contains the specified value.
        /// </summary>
        /// <param name="scope">The scope of the constraint.</param>
        /// <param name="value">The value the collection should contain.</param>
        /// <returns>An argument constraint.</returns>
        public static ArgumentConstraint<T> Contains<T>(this ArgumentConstraintScope<T> scope, object value) where T : IEnumerable
        {
            return new EnumerableContainsConstraint<T>(scope, value);
        }

        /// <summary>
        /// Validates that the string argument starts with the specified text.
        /// </summary>
        /// <param name="scope">The scope of the constraint.</param>
        /// <param name="value">The string the argument string should start with.</param>
        /// <returns>An argument constraint.</returns>
        public static ArgumentConstraint<string> StartsWith(this ArgumentConstraintScope<string> scope, string value)
        {
            return scope.CreateConstraint(x => x != null && x.StartsWith(value), "String that starts with \"{0}\"", value);
        }

        /// <summary>
        /// Validates that the string argument is null or the empty string.
        /// </summary>
        /// <param name="scope">The scope of the constraint.</param>
        /// <returns>An argument constraint.</returns>
        public static ArgumentConstraint<string> IsNullOrEmpty(this ArgumentConstraintScope<string> scope)
        {
            return scope.CreateConstraint(x => string.IsNullOrEmpty(x), "(NULL or string.Empty)");
        }

        /// <summary>
        /// Validates that the argument is greater than the specified value.
        /// </summary>
        /// <typeparam name="T">The type of the argument.</typeparam>
        /// <param name="scope">The scope of the constraint.</param>
        /// <param name="value">The value that the argument has to be greatere than.</param>
        /// <returns>An argument constraint.</returns>
        public static ArgumentConstraint<T> IsGreaterThan<T>(this ArgumentConstraintScope<T> scope, T value) where T : IComparable
        {
            return scope.CreateConstraint(x => x.CompareTo(value) > 0, "Greater than {0}", value);
        }

        private static ArgumentConstraint<T> CreateConstraint<T>(this ArgumentConstraintScope<T> scope, Func<T, bool> predicate, string description)
        {
            return ArgumentConstraint.Create(scope, predicate, description);
        }
        
        private static ArgumentConstraint<T> CreateConstraint<T>(this ArgumentConstraintScope<T> scope, Func<T, bool> predicate, string descriptionFormat, params object[] args)
        {
            return ArgumentConstraint.Create(scope, predicate, descriptionFormat.FormatInvariant(args));
        }
    }
}
