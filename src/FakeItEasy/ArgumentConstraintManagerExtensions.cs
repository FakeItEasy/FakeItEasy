namespace FakeItEasy
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using FakeItEasy.Expressions.ArgumentConstraints;

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
            Guard.AgainstNull(manager);

            return manager.Matches(NullArgumentConstraint.Instance.IsValid, NullArgumentConstraint.Instance.ConstraintDescription);
        }

        /// <summary>
        /// Constrains an argument so that it must be null (Nothing in VB).
        /// </summary>
        /// <typeparam name="T">The type of the argument.</typeparam>
        /// <param name="manager">The constraint manager to match the constraint.</param>
        /// <returns>A dummy argument value.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is by design to support the fluent API.")]
        public static T? IsNull<T>(this IArgumentConstraintManager<T?> manager) where T : struct
        {
            Guard.AgainstNull(manager);

            return manager.Matches(x => NullArgumentConstraint.Instance.IsValid(x), NullArgumentConstraint.Instance.ConstraintDescription);
        }

        /// <summary>
        /// Constrains an argument so that it must not be null (Nothing in VB).
        /// </summary>
        /// <typeparam name="T">The type of the argument.</typeparam>
        /// <param name="manager">The constraint manager to match the constraint.</param>
        /// <returns>A dummy argument value.</returns>
        public static T IsNotNull<T>(this INegatableArgumentConstraintManager<T> manager) where T : class
        {
            Guard.AgainstNull(manager);

            return manager.Matches(x => x is not null, x => x.Write("NOT NULL"));
        }

        /// <summary>
        /// Constrains an argument so that it must not be null (Nothing in VB).
        /// </summary>
        /// <typeparam name="T">The type of the argument.</typeparam>
        /// <param name="manager">The constraint manager to match the constraint.</param>
        /// <returns>A dummy argument value.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is by design to support the fluent API.")]
        public static T? IsNotNull<T>(this INegatableArgumentConstraintManager<T?> manager) where T : struct
        {
            Guard.AgainstNull(manager);

            return manager.Matches(x => x is not null, x => x.Write("NOT NULL"));
        }

        /// <summary>
        /// Constrains the string argument to contain the specified text, using the <see cref="StringComparison.Ordinal" /> comparison type.
        /// </summary>
        /// <param name="manager">The constraint manager to match the constraint.</param>
        /// <param name="value">The string the argument string should contain.</param>
        /// <returns>A dummy argument value.</returns>
        public static string Contains(this IArgumentConstraintManager<string> manager, string value)
        {
            return manager.Contains(value, StringComparison.Ordinal);
        }

        /// <summary>
        /// Constrains the string argument to contain the specified text, using the specified comparison type.
        /// </summary>
        /// <param name="manager">The constraint manager to match the constraint.</param>
        /// <param name="value">The string the argument string should contain.</param>
        /// <param name="comparisonType">The type of string comparison to use.</param>
        /// <returns>A dummy argument value.</returns>
        public static string Contains(this IArgumentConstraintManager<string> manager, string value, StringComparison comparisonType)
        {
            Guard.AgainstNull(manager);
            Guard.AgainstNull(value);

#if LACKS_STRING_CONTAINS_COMPARISONTYPE
            return manager.NullCheckedMatches(x => x.IndexOf(value, comparisonType) >= 0, x => x.Write("string that contains ").WriteArgumentValue(value));
#else
            return manager.NullCheckedMatches(x => x.Contains(value, comparisonType), x => x.Write("string that contains ").WriteArgumentValue(value));
#endif
        }

        /// <summary>
        /// Constrains the sequence so that it must contain the specified value.
        /// </summary>
        /// <param name="manager">The constraint manager to match the constraint.</param>
        /// <param name="value">The value the collection should contain.</param>
        /// <typeparam name="T">The type of sequence.</typeparam>
        /// <returns>A dummy argument value.</returns>
        public static T Contains<T>(this IArgumentConstraintManager<T> manager, object? value) where T : IEnumerable
        {
            Guard.AgainstNull(manager);

            return manager.NullCheckedMatches(
                x => x.Cast<object>().Contains(value),
                x => x.Write("sequence that contains the value ").WriteArgumentValue(value));
        }

        /// <summary>
        /// Constrains the string so that it must start with the specified value,
        /// using the <see cref="StringComparison.Ordinal" /> comparison type.
        /// </summary>
        /// <param name="manager">The constraint manager to match the constraint.</param>
        /// <param name="value">The value the string should start with.</param>
        /// <returns>A dummy argument value.</returns>
        public static string StartsWith(this IArgumentConstraintManager<string> manager, string value)
        {
            return manager.StartsWith(value, StringComparison.Ordinal);
        }

        /// <summary>
        /// Constrains the string so that it must start with the specified value, using the specified comparison type.
        /// </summary>
        /// <param name="manager">The constraint manager to match the constraint.</param>
        /// <param name="value">The value the string should start with.</param>
        /// <param name="comparisonType">The type of string comparison to use.</param>
        /// <returns>A dummy argument value.</returns>
        public static string StartsWith(
            this IArgumentConstraintManager<string> manager,
            string value,
            StringComparison comparisonType)
        {
            Guard.AgainstNull(manager);
            Guard.AgainstNull(value);

            return manager.NullCheckedMatches(
                x => x.StartsWith(value, comparisonType),
                x => x.Write("string that starts with ").WriteArgumentValue(value));
        }

        /// <summary>
        /// Constrains the string so that it must end with the specified value,
        /// using the <see cref="StringComparison.Ordinal" /> comparison type.
        /// </summary>
        /// <param name="manager">The constraint manager to match the constraint.</param>
        /// <param name="value">The value the string should end with.</param>
        /// <returns>A dummy argument value.</returns>
        public static string EndsWith(this IArgumentConstraintManager<string> manager, string value)
        {
            Guard.AgainstNull(manager);
            Guard.AgainstNull(value);

            return manager.EndsWith(value, StringComparison.Ordinal);
        }

        /// <summary>
        /// Constrains the string so that it must end with the specified value, using the specified comparison type.
        /// </summary>
        /// <param name="manager">The constraint manager to match the constraint.</param>
        /// <param name="value">The value the string should end with.</param>
        /// <param name="comparisonType">The type of string comparison to use.</param>
        /// <returns>A dummy argument value.</returns>
        public static string EndsWith(
            this IArgumentConstraintManager<string> manager,
            string value,
            StringComparison comparisonType)
        {
            Guard.AgainstNull(manager);
            Guard.AgainstNull(value);

            return manager.NullCheckedMatches(
                x => x.EndsWith(value, comparisonType),
                x => x.Write("string that ends with ").WriteArgumentValue(value));
        }

        /// <summary>
        /// Constrains the string so that it must be null or empty.
        /// </summary>
        /// <param name="manager">The constraint manager to match the constraint.</param>
        /// <returns>A dummy argument value.</returns>
        public static string IsNullOrEmpty(this IArgumentConstraintManager<string> manager)
        {
            Guard.AgainstNull(manager);

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
            Guard.AgainstNull(manager);

            return manager.Matches(x => x.CompareTo(value) > 0, x => x.Write("greater than ").WriteArgumentValue(value));
        }

        /// <summary>
        /// Constrains the argument collection so that it must contain the same elements as the
        /// specified collection, in the same order.
        /// </summary>
        /// <param name="manager">The constraint manager to match the constraint.</param>
        /// <param name="values">The sequence to test against.</param>
        /// <param name="comparer">A comparer to test the collection elements for equality. If null, the default equality comparer for <see cref="TElement"/> will be used.</param>
        /// <typeparam name="T">The type of argument to constrain.</typeparam>
        /// <typeparam name="TElement">The type of the collection's elements.</typeparam>
        /// <returns>A dummy argument value.</returns>
        public static T IsSameSequenceAs<T, TElement>(
            this IArgumentConstraintManager<T> manager,
            IEnumerable<TElement> values,
            IEqualityComparer<TElement>? comparer = null)
            where T : IEnumerable<TElement>
        {
            Guard.AgainstNull(manager);
            Guard.AgainstNull(values);

            var list = values.AsList();
            return manager.NullCheckedMatches(
                x => x.SequenceEqual(list, comparer),
                x => x.WriteArgumentValues(list));
        }

        /// <summary>
        /// Constrains the argument collection so that it must contain the same elements as the
        /// specified collection, in the same order.
        /// </summary>
        /// <param name="manager">The constraint manager to match the constraint.</param>
        /// <param name="values">The sequence to test against.</param>
        /// <typeparam name="T">The type of argument to constrain.</typeparam>
        /// <typeparam name="TElement">The type of the collection's elements.</typeparam>
        /// <returns>A dummy argument value.</returns>
        public static T IsSameSequenceAs<T, TElement>(
            this IArgumentConstraintManager<T> manager,
            params TElement[] values)
            where T : IEnumerable<TElement>
        {
            Guard.AgainstNull(manager);
            Guard.AgainstNull(values);

            return manager.IsSameSequenceAs((IEnumerable<TElement>)values);
        }

        /// <summary>
        /// Constrains the argument collection so that it must contain the same elements as the
        /// specified collection, in any order.
        /// </summary>
        /// <param name="manager">The constraint manager to match the constraint.</param>
        /// <param name="values">The sequence to test against.</param>
        /// <param name="comparer">A comparer to test the collection elements for equality. If null, the default equality comparer for <see cref="TElement"/> will be used.</param>
        /// <typeparam name="T">The type of argument to constrain.</typeparam>
        /// <typeparam name="TElement">The type of the collection's elements.</typeparam>
        /// <returns>A dummy argument value.</returns>
        public static T HasSameElementsAs<T, TElement>(
            this IArgumentConstraintManager<T> manager,
            IEnumerable<TElement> values,
            IEqualityComparer<TElement>? comparer = null)
            where T : IEnumerable<TElement>
        {
            Guard.AgainstNull(manager);
            Guard.AgainstNull(values);

            var list = values.AsList();
            return manager.NullCheckedMatches(
                x => x.HasSameElementsAs(list, comparer),
                x => x.WriteArgumentValues(list).Write(" (in any order)"));
        }

        /// <summary>
        /// Constrains the argument collection so that it must contain the same elements as the
        /// specified collection, in any order.
        /// </summary>
        /// <param name="manager">The constraint manager to match the constraint.</param>
        /// <param name="values">The sequence to test against.</param>
        /// <typeparam name="T">The type of argument to constrain.</typeparam>
        /// <typeparam name="TElement">The type of the collection's elements.</typeparam>
        /// <returns>A dummy argument value.</returns>
        public static T HasSameElementsAs<T, TElement>(
            this IArgumentConstraintManager<T> manager,
            params TElement[] values)
            where T : IEnumerable<TElement>
        {
            Guard.AgainstNull(manager);
            Guard.AgainstNull(values);

            return manager.HasSameElementsAs((IEnumerable<TElement>)values);
        }

        /// <summary>
        /// Tests that the IEnumerable contains no items.
        /// </summary>
        /// <typeparam name="T">The type of argument.</typeparam>
        /// <param name="manager">The constraint manager to match the constraint.</param>
        /// <returns>A dummy argument value.</returns>
        public static T IsEmpty<T>(this IArgumentConstraintManager<T> manager) where T : IEnumerable
        {
            Guard.AgainstNull(manager);

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
            Guard.AgainstNull(manager);

            return manager.Matches(
                x => object.Equals(value, x),
                x => x.Write("equal to ").WriteArgumentValue(value));
        }

        /// <summary>
        /// Tests that the passed in argument is equal to the specified value using provided equality comparer.
        /// </summary>
        /// <typeparam name="T">The type of the argument.</typeparam>
        /// <param name="manager">The constraint manager to match the constraint.</param>
        /// <param name="value">The value to compare to.</param>
        /// <param name="comparer">The comparer to use for equality comparison.</param>
        /// <returns>A dummy argument value.</returns>
        public static T IsEqualTo<T>(this IArgumentConstraintManager<T> manager, T value, IEqualityComparer<T> comparer)
        {
            Guard.AgainstNull(manager);
            Guard.AgainstNull(comparer);

            return manager.Matches(
                x => comparer.Equals(value, x),
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
            Guard.AgainstNull(manager);

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
            Guard.AgainstNull(manager);
            Guard.AgainstNull(type);

            return manager.Matches(x => x is not null && type.IsAssignableFrom(x.GetType()), description => description.Write("Instance of ").Write(type.ToString()));
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
        /// <param name="description">
        /// A human readable description of the constraint.
        /// </param>
        /// <typeparam name="T">
        /// The type of argument in the method signature.
        /// </typeparam>
        /// <returns>
        /// A dummy argument value.
        /// </returns>
        public static T Matches<T>(this IArgumentConstraintManager<T> manager, Func<T, bool> predicate, string description)
        {
            Guard.AgainstNull(manager);
            Guard.AgainstNull(predicate);
            Guard.AgainstNull(description);

            return manager.Matches(predicate, x => x.Write(description));
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
        public static T Matches<T>(this IArgumentConstraintManager<T> manager, Func<T, bool> predicate, string descriptionFormat, params object?[] args)
        {
            Guard.AgainstNull(manager);
            Guard.AgainstNull(predicate);
            Guard.AgainstNull(descriptionFormat);
            Guard.AgainstNull(args);

            return manager.Matches(predicate, x => x.Write(descriptionFormat, args));
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
        /// <typeparam name="T">
        /// The type of argument in the method signature.
        /// </typeparam>
        /// <returns>
        /// A dummy argument value.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Appropriate for Linq expressions.")]
        public static T Matches<T>(this IArgumentConstraintManager<T> manager, Expression<Func<T, bool>> predicate)
        {
            Guard.AgainstNull(manager);
            Guard.AgainstNull(predicate);

            return manager.Matches(predicate.Compile(), predicate.ToString());
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
            Guard.AgainstNull(manager);
            Guard.AgainstNull(predicate);
            Guard.AgainstNull(descriptionWriter);

            return manager.Matches(
                x => x is not null && predicate(x),
                descriptionWriter);
        }

        /// <summary>
        /// Constrains the <see cref="CancellationToken"/> argument to be canceled (<c>IsCancellationRequested</c> is true).
        /// </summary>
        /// <param name="manager">The constraint manager.</param>
        /// <returns>A dummy argument value.</returns>
        public static CancellationToken IsCanceled(this IArgumentConstraintManager<CancellationToken> manager)
        {
            Guard.AgainstNull(manager);

            return manager.Matches(
                token => token.IsCancellationRequested,
                x => x.Write("canceled cancellation token"));
        }

        /// <summary>
        /// Constrains the <see cref="CancellationToken"/> argument to be not canceled (<c>IsCancellationRequested</c> is false).
        /// </summary>
        /// <param name="manager">The constraint manager.</param>
        /// <returns>A dummy argument value.</returns>
        public static CancellationToken IsNotCanceled(this INegatableArgumentConstraintManager<CancellationToken> manager)
        {
            Guard.AgainstNull(manager);

            return manager.Matches(
                token => !token.IsCancellationRequested,
                x => x.Write("non-canceled cancellation token"));
        }
    }
}
