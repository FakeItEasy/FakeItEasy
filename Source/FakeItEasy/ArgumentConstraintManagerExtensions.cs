namespace FakeItEasy
{
    using System;
    using System.Collections;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary>
    /// Provides validation extensions for <see cref="IArgumentConstraintManager{T}"/>.
    /// </summary>
    public static class ArgumentConstraintManagerExtensions
    {
        /// <summary>
        /// Constrains an argument so that it must be null (Nothing in VB).
        /// </summary>
        /// <typeparam name="T">The type of the argument.</typeparam>
        /// <param name="manager">The constraint manager to match the constraint.</param>
        /// <returns>A dummy argument value.</returns>
        public static T IsNull<T>(this IArgumentConstraintManager<T> manager) where T : class
        {
            Guard.AgainstNull(manager, "manager");

            return manager.Matches(x => x == null, x => x.Write("NULL"));
        }

        /// <summary>
        /// Constrains the string argument to contain the specified text.
        /// </summary>
        /// <param name="manager">The constraint manager to match the constraint.</param>
        /// <param name="value">The string the argument string should contain.</param>
        /// <returns>A dummy argument value.</returns>
        public static string Contains(this IArgumentConstraintManager<string> manager, string value)
        {
            return manager.NullCheckedMatches(x => x.Contains(value), x => x.Write("string that contains ").WriteArgumentValue(value));
        }

        /// <summary>
        /// Constrains the sequence so that it must contain the specified value.
        /// </summary>
        /// <param name="manager">The constraint manager to match the constraint.</param>
        /// <param name="value">The value the collection should contain.</param>
        /// <typeparam name="T">The type of sequence.</typeparam>
        /// <returns>A dummy argument value.</returns>
        public static T Contains<T>(this IArgumentConstraintManager<T> manager, object value) where T : IEnumerable
        {
            return manager.NullCheckedMatches(
                x => x.Cast<object>().Contains(value),
                x => x.Write("sequence that contains the value ").WriteArgumentValue(value));
        }

        /// <summary>
        /// Constrains the string so that it must start with the specified value.
        /// </summary>
        /// <param name="manager">The constraint manager to match the constraint.</param>
        /// <param name="value">The value the string should start with.</param>
        /// <returns>A dummy argument value.</returns>
        public static string StartsWith(this IArgumentConstraintManager<string> manager, string value)
        {
            return manager.NullCheckedMatches(x => x.StartsWith(value, StringComparison.Ordinal), x => x.Write("string that starts with ").WriteArgumentValue(value));
        }

        /// <summary>
        /// Constrains the string so that it must end with the specified value.
        /// </summary>
        /// <param name="manager">The constraint manager to match the constraint.</param>
        /// <param name="value">The value the string should end with.</param>
        /// <returns>A dummy argument value.</returns>
        public static string EndsWith(this IArgumentConstraintManager<string> manager, string value)
        {
            return manager.NullCheckedMatches(x => x.EndsWith(value, StringComparison.Ordinal), x => x.Write("string that ends with ").WriteArgumentValue(value));
        }

        /// <summary>
        /// Constrains the string so that it must be null or empty.
        /// </summary>
        /// <param name="manager">The constraint manager to match the constraint.</param>
        /// <returns>A dummy argument value.</returns>
        public static string IsNullOrEmpty(this IArgumentConstraintManager<string> manager)
        {
            return manager.Matches(x => string.IsNullOrEmpty(x), "NULL or string.Empty");
        }

        /// <summary>
        /// Constrains argument value so that it must be greater than the specified value.
        /// </summary>
        /// <param name="manager">The constraint manager to match the constraint.</param>
        /// <param name="value">The value the string should start with.</param>
        /// <typeparam name="T">The type of argument to constrain.</typeparam>
        /// <returns>A dummy argument value.</returns>
        public static T IsGreaterThan<T>(this IArgumentConstraintManager<T> manager, T value) where T : IComparable
        {
            Guard.AgainstNull(manager, "manager");

            return manager.Matches(x => x.CompareTo(value) > 0, x => x.Write("greater than ").WriteArgumentValue(value));
        }

        /// <summary>
        /// The tested argument collection should contain the same elements as the
        /// as the specified collection.
        /// </summary>
        /// <param name="manager">The constraint manager to match the constraint.</param>
        /// <param name="value">The sequence to test against.</param>
        /// <typeparam name="T">The type of argument to constrain.</typeparam>
        /// <returns>A dummy argument value.</returns>
        public static T IsSameSequenceAs<T>(this IArgumentConstraintManager<T> manager, IEnumerable value) where T : IEnumerable
        {
            return manager.NullCheckedMatches(
                x => x.Cast<object>().SequenceEqual(value.Cast<object>()),
                x => x.Write("specified sequence"));
        }

        /// <summary>
        /// Tests that the IEnumerable contains no items.
        /// </summary>
        /// <typeparam name="T">The type of argument.</typeparam>
        /// <param name="manager">The constraint manager to match the constraint.</param>
        /// <returns>A dummy argument value.</returns>
        public static T IsEmpty<T>(this IArgumentConstraintManager<T> manager) where T : IEnumerable
        {
            return manager.NullCheckedMatches(
                x => !x.Cast<object>().Any(),
                x => x.Write("empty collection"));
        }

        /// <summary>
        /// Tests that the passed in argument is equal to the specified value.
        /// </summary>
        /// <typeparam name="T">The type of the argument.</typeparam>
        /// <param name="manager">The constraint manager to match the constraint.</param>
        /// <param name="value">The value to compare to.</param>
        /// <returns>A dummy argument value.</returns>
        public static T IsEqualTo<T>(this IArgumentConstraintManager<T> manager, T value)
        {
            Guard.AgainstNull(manager, "manager");

            return manager.Matches(
                x => object.Equals(value, x),
                x => x.Write("equal to ").WriteArgumentValue(value));
        }
        
        /// <summary>
        /// Tests that the passed in argument is the same instance (reference) as the specified value.
        /// </summary>
        /// <typeparam name="T">The type of the argument.</typeparam>
        /// <param name="manager">The constraint manager to match the constraint.</param>
        /// <param name="value">The reference to compare to.</param>
        /// <returns>A dummy argument value.</returns>
        public static T IsSameAs<T>(this IArgumentConstraintManager<T> manager, T value)
        {
            Guard.AgainstNull(manager, "manager");

            return manager.Matches(
                x => object.ReferenceEquals(value, x),
                x => x.Write("same as ").WriteArgumentValue(value));
        }

        /// <summary>
        /// Constrains the argument to be of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of argument in the method signature.</typeparam>
        /// <param name="manager">The constraint manager.</param>
        /// <param name="type">The type to constrain the argument with.</param>
        /// <returns>A dummy value.</returns>
        public static T IsInstanceOf<T>(this IArgumentConstraintManager<T> manager, Type type)
        {
            return manager.NullCheckedMatches(x => type.IsAssignableFrom(x.GetType()), x => x.Write("Instance of ").Write(type.FullName));
        }

        /// <summary>
        /// Constrains the argument with a predicate.
        /// </summary>
        /// <param name="scope">
        /// The constraint manager.
        /// </param>
        /// <param name="predicate">
        /// The predicate that should constrain the argument.
        /// </param>
        /// <param name="description">
        /// A human readable description of the constraint.
        /// </param>
        /// <typeparam name="T">
        /// The type of argument in the method signature.
        /// </typeparam>
        /// <returns>
        /// A dummy argument value.
        /// </returns>
        public static T Matches<T>(this IArgumentConstraintManager<T> scope, Func<T, bool> predicate, string description)
        {
            Guard.AgainstNull(scope, "scope");

            return scope.Matches(predicate, x => x.Write(description));
        }

        /// <summary>
        /// Constrains the argument with a predicate.
        /// </summary>
        /// <param name="manager">
        /// The constraint manager.
        /// </param>
        /// <param name="predicate">
        /// The predicate that should constrain the argument.
        /// </param>
        /// <param name="descriptionFormat">
        /// A human readable description of the constraint format string.
        /// </param>
        /// <param name="args">
        /// Arguments for the format string.
        /// </param>
        /// <typeparam name="T">
        /// The type of argument in the method signature.
        /// </typeparam>
        /// <returns>
        /// A dummy argument value.
        /// </returns>
        public static T Matches<T>(this IArgumentConstraintManager<T> manager, Func<T, bool> predicate, string descriptionFormat, params object[] args)
        {
            Guard.AgainstNull(manager, "manager");

            return manager.Matches(predicate, x => x.Write(string.Format(CultureInfo.CurrentCulture, descriptionFormat, args)));
        }

        /// <summary>
        /// Constrains the argument with a predicate.
        /// </summary>
        /// <param name="scope">
        /// The constraint manager.
        /// </param>
        /// <param name="predicate">
        /// The predicate that should constrain the argument.
        /// </param>
        /// <typeparam name="T">
        /// The type of argument in the method signature.
        /// </typeparam>
        /// <returns>
        /// A dummy argument value.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Appropriate for Linq expressions.")]
        public static T Matches<T>(this IArgumentConstraintManager<T> scope, Expression<Func<T, bool>> predicate)
        {
            Guard.AgainstNull(predicate, "predicate");

            return scope.Matches(predicate.Compile(), predicate.ToString());
        }

        /// <summary>
        /// Constrains the argument to be not null (Nothing in VB) and to match
        /// the specified predicate.
        /// </summary>
        /// <typeparam name="T">The type of the argument to constrain.</typeparam>
        /// <param name="manager">The constraint manager.</param>
        /// <param name="predicate">The predicate that constrains non null values.</param>
        /// <param name="descriptionWriter">An action that writes a description of the constraint
        /// to the output.</param>
        /// <returns>A dummy argument value.</returns>
        public static T NullCheckedMatches<T>(this IArgumentConstraintManager<T> manager, Func<T, bool> predicate, Action<IOutputWriter> descriptionWriter)
        {
            Guard.AgainstNull(manager, "manager");

            return manager.Matches(
                x => ((object)x) != null && predicate(x),
                descriptionWriter);
        }
    }
}
