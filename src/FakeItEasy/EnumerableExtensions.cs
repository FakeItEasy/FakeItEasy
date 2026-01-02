namespace FakeItEasy
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Provides extension methods for generic usage of <see cref="IEnumerable{T}"/>.
    /// </summary>
    internal static class EnumerableExtensions
    {
        /// <summary>
        /// Joins the collection to a string.
        /// </summary>
        /// <typeparam name="T">The type of items in the collection.</typeparam>
        /// <param name="items">The items to join.</param>
        /// <param name="stringConverter">A function that converts from an item to a string value.</param>
        /// <param name="separator">Separator to insert between each item.</param>
        /// <returns>A string representation of the collection.</returns>
        public static string ToCollectionString<T>(this IEnumerable<T> items, Func<T, string?> stringConverter, string separator)
        {
            var result = new StringBuilder();

            foreach (var item in items)
            {
                if (result.Length > 0)
                {
                    result.Append(separator);
                }

                result.Append(stringConverter(item));
            }

            return result.ToString();
        }

        public static IEnumerable<T> Concat<T>(this IEnumerable<T> source, T item)
        {
            return source.Concat(new[] { item });
        }

        /// <summary>
        /// Returns the sequence as a list in order to avoid multiple enumerations of the original sequence.
        /// </summary>
        /// <typeparam name="T">The type of items in the sequence.</typeparam>
        /// <param name="source">The sequence to return as a list.</param>
        /// <returns>The sequence cast as a list if it's actually a list; otherwise, a new list with the elements from the sequence.</returns>
        public static IList<T> AsList<T>(this IEnumerable<T> source)
        {
            return source as IList<T> ?? source.ToList();
        }

        /// <summary>
        /// Returns the sequence as a list in order to avoid multiple enumerations of the original sequence.
        /// </summary>
        /// <param name="source">The sequence to return as a list.</param>
        /// <returns>The sequence cast as a list if it's actually a list; otherwise, a new list with the elements from the sequence.</returns>
        public static IList<object> AsList(this IEnumerable source)
        {
            return source as IList<object> ?? source.Cast<object>().ToList();
        }

        public static bool HasSameElementsAs<T>(
            this IEnumerable<T> collection,
            IEnumerable<T> other,
            IEqualityComparer<T>? comparer)
        {
            Guard.AgainstNull(collection);
            Guard.AgainstNull(other);

            // Fail fast if counts differ, when we can get the count without enumerating
            if (collection.TryGetCount(out int collectionCount)
                && other.TryGetCount(out int otherCount)
                && collectionCount != otherCount)
            {
                return false;
            }

            var boxComparer = new BoxEqualityComparer<T>(comparer);
            var elementCounts = other
                .GroupBy(x => new Box<T>(x), boxComparer)
                .ToDictionary(g => g.Key, g => g.Count(), boxComparer);
            foreach (var element in collection)
            {
                var box = new Box<T>(element);
                if (!elementCounts.TryGetValue(box, out var count))
                {
                    // collection contains element that isn't in other
                    return false;
                }

                if (--count == 0)
                {
                    elementCounts.Remove(box);
                }
                else
                {
                    elementCounts[box] = count;
                }
            }

            // Ensure we consumed all elements from other.
            // If not, collection had fewer elements than other
            return elementCounts.Count == 0;
        }

        /// <summary>
        /// Tries to get the count of items in the collection without enumerating it.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="count">The count of items, if it could be determined without enumeration; otherwise, zero.</param>
        /// <typeparam name="T">The type of items in the collection.</typeparam>
        /// <returns>true if the count could be determined without enumeration; otherwise, false.</returns>
        public static bool TryGetCount<T>(this IEnumerable<T> collection, out int count)
        {
#if LACKS_NONENUMERATEDCOUNT
            if (collection is ICollection<T> genericCollection)
            {
                count = genericCollection.Count;
                return true;
            }

            if (collection is ICollection nonGenericCollection)
            {
                count = nonGenericCollection.Count;
                return true;
            }

            if (collection is IReadOnlyCollection<T> readOnlyCollection)
            {
                count = readOnlyCollection.Count;
                return true;
            }

            count = 0;
            return false;
#else
            return collection.TryGetNonEnumeratedCount(out count);
#endif
        }

        /// <summary>
        /// Wraps a possibly null value in a struct to allow usage as a key in dictionaries.
        /// </summary>
        /// <param name="Value">The value to wrap.</param>
        /// <typeparam name="T">The type of the value.</typeparam>
        private record struct Box<T>(T Value);

        private class BoxEqualityComparer<T> : IEqualityComparer<Box<T>>
        {
            private readonly IEqualityComparer<T> comparer;

            public BoxEqualityComparer(IEqualityComparer<T>? comparer)
            {
                this.comparer = comparer ?? EqualityComparer<T>.Default;
            }

            public bool Equals(Box<T> x, Box<T> y)
            {
                return this.comparer.Equals(x.Value, y.Value);
            }

            public int GetHashCode(Box<T> obj)
            {
                return this.comparer.GetHashCode(obj.Value!);
            }
        }
    }
}
