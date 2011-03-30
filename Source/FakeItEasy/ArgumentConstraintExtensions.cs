namespace FakeItEasy
{
    using System;
    using System.Collections;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;
    using Core;

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
        public static T IsNull<T>(this IArgumentConstraintManager<T> scope) where T : class
        {
            return scope.Matches(x => x == null, "NULL");
        }

        /// <summary>
        /// Validates that the string argument contains the specified text.
        /// </summary>
        /// <param name="scope">The scope of the constraint.</param>
        /// <param name="value">The string the argument string should contain.</param>
        /// <returns>An argument constraint.</returns>
        public static string Contains(this IArgumentConstraintManager<string> scope, string value)
        {
            return scope.Matches(x => x != null && x.Contains(value), "String that contains \"{0}\"", value);
        }

        /// <summary>
        /// Validates that the collection argument contains the specified value.
        /// </summary>
        /// <param name="scope">The scope of the constraint.</param>
        /// <param name="value">The value the collection should contain.</param>
        /// <returns>An argument constraint.</returns>
        public static T Contains<T>(this IArgumentConstraintManager<T> scope, object value) where T : IEnumerable
        {
            return scope.Matches(
                x => x.Cast<object>().Contains(value),
                "sequence that contains the value {0}", 
                value);
        }

        /// <summary>
        /// Validates that the string argument starts with the specified text.
        /// </summary>
        /// <param name="scope">The scope of the constraint.</param>
        /// <param name="value">The string the argument string should start with.</param>
        /// <returns>An argument constraint.</returns>
        public static string StartsWith(this IArgumentConstraintManager<string> scope, string value)
        {
            return scope.Matches(x => x != null && x.StartsWith(value, StringComparison.Ordinal), "String that starts with \"{0}\"", value);
        }

        /// <summary>
        /// Validates that the string argument is null or the empty string.
        /// </summary>
        /// <param name="scope">The scope of the constraint.</param>
        /// <returns>An argument constraint.</returns>
        public static string IsNullOrEmpty(this IArgumentConstraintManager<string> scope)
        {
            return scope.Matches(x => string.IsNullOrEmpty(x), "(NULL or string.Empty)");
        }

        /// <summary>
        /// Validates that the argument is greater than the specified value.
        /// </summary>
        /// <typeparam name="T">The type of the argument.</typeparam>
        /// <param name="scope">The scope of the constraint.</param>
        /// <param name="value">The value that the argument has to be greatere than.</param>
        /// <returns>An argument constraint.</returns>
        public static T IsGreaterThan<T>(this IArgumentConstraintManager<T> scope, T value) where T : IComparable
        {
            return scope.Matches(x => x.CompareTo(value) > 0, "Greater than {0}", value);
        }

        /// <summary>
        /// The tested argument collection should contain the same elements as the
        /// as the specified collection.
        /// </summary>
        /// <typeparam name="T">The type of collection.</typeparam>
        /// <param name="scope">The scope of the constraint.</param>
        /// <param name="value">The sequence to test against.</param>
        /// <returns>An argument constraint.</returns>
        public static T IsSameSequenceAs<T>(this IArgumentConstraintManager<T> scope, IEnumerable value) where T : IEnumerable
        {
            return scope.Matches(
                x => x != null && x.Cast<object>().SequenceEqual(value.Cast<object>()), 
                "specified sequence");
        }

        /// <summary>
        /// Tests that the IEnumerable contains no items.
        /// </summary>
        /// <typeparam name="T">The type of argument.</typeparam>
        /// <param name="scope">The scope of the constraint.</param>
        /// <returns>An argument constraint.</returns>
        public static T IsEmpty<T>(this IArgumentConstraintManager<T> scope) where T : IEnumerable
        {
            return scope.Matches(
                         x => x != null && !x.Cast<object>().Any(), 
                         "empty collection");
        }

        /// <summary>
        /// Tests that the passed in argument is equal to the specified value.
        /// </summary>
        /// <typeparam name="T">The type of the argument.</typeparam>
        /// <param name="scope">The scope of the constraint.</param>
        /// <param name="value">The value to compare to.</param>
        /// <returns>An argument constraint.</returns>
        public static T IsEqualTo<T>(this IArgumentConstraintManager<T> scope, T value)
        {
            return scope.Matches(
                         x => Equals(value, x), 
                         "equal to {0}", 
                         value);
        }

        /// <summary>
        /// Constraines the argument to be of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of argument in the method signature.</typeparam>
        /// <param name="manager">The constraint manager.</param>
        /// <param name="type">The type to constrain the argument with.</param>
        /// <returns>A dummy value.</returns>
        public static T IsInstanceOf<T>(this IArgumentConstraintManager<T> manager, Type type)
        {
            return manager.Matches(x => type.IsAssignableFrom(x.GetType()), "Instance of " + type.FullName);
        }

        /// <summary>
        /// Constrains the argument with a predicate.
        /// </summary>
        /// <param name="predicate">The predicate that should constrain the argument.</param>
        /// <param name="description">A human readable description of the constraint.</param>
        /// <param name="scope">The constraint manager.</param>
        /// <typeparam name="T">The type of argument in the method signature.</typeparam>
        /// <returns>A dummy argument value.</returns>
        public static T Matches<T>(this IArgumentConstraintManager<T> scope, Func<T, bool> predicate, string description)
        {
            return scope.Matches(predicate, x => x.Write(description));
        }

        /// <summary>
        /// Constrains the argument with a predicate.
        /// </summary>
        /// <param name="predicate">The predicate that should constrain the argument.</param>
        /// <param name="descriptionFormat">A human readable description of the constraint format string.</param>
        /// <param name="args">Arguments for the format string.</param>
        /// <param name="scope">The constraint manager.</param>
        /// <typeparam name="T">The type of argument in the method signature.</typeparam>
        /// <returns>A dummy argument value.</returns>
        public static T Matches<T>(this IArgumentConstraintManager<T> scope, Func<T, bool> predicate, string descriptionFormat, params object[] args)
        {
            return scope.Matches(predicate, x => x.Write(string.Format(descriptionFormat, args)));
        }

        /// <summary>
        /// Constrains the argument with a predicate.
        /// </summary>
        /// <param name="predicate">The predicate that should constrain the argument.</param>
        /// <param name="scope">The constraint manager.</param>
        /// <typeparam name="T">The type of argument in the method signature.</typeparam>
        /// <returns>A dummy argument value.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Appropriate for Linq expressions.")]
        public static T Matches<T>(this IArgumentConstraintManager<T> scope, Expression<Func<T, bool>> predicate)
        {
            return scope.Matches(predicate.Compile(), predicate.ToString());
        }
    }
}