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
        public static string ToCollectionString<T>(this IEnumerable<T> items, Func<T, string> stringConverter, string separator)
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
    }
}
