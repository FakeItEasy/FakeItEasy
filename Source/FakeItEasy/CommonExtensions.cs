﻿namespace FakeItEasy
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    /// <summary>
    /// Provides extension methods for the common uses.
    /// </summary>
    internal static class CommonExtensions
    {
        /// <summary>
        /// Gets an enumerable of tuples where the first value of each tuple is a value
        /// from the first collection and the second value of each tuple is the value at the same position
        /// from the second collection.
        /// </summary>
        /// <typeparam name="TFirst">The type of values in the first collection.</typeparam>
        /// <typeparam name="TSecond">The type of values in the second collection.</typeparam>
        /// <param name="firstCollection">The first of the collections to combine.</param>
        /// <param name="secondCollection">The second of the collections to combine.</param>
        /// <returns>An enumerable of tuples.</returns>
        public static IEnumerable<Tuple<TFirst, TSecond>> Zip<TFirst, TSecond>(this IEnumerable<TFirst> firstCollection, IEnumerable<TSecond> secondCollection)
        {
            return new ZipEnumerable<TFirst, TSecond>(firstCollection, secondCollection);
        }

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

        /// <summary>
        /// Gets a dictionary containing the first element from the sequence that has a key specified by the key selector.
        /// </summary>
        /// <typeparam name="T">The type of items in the sequence.</typeparam>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="sequence">The sequence.</param>
        /// <param name="keySelector">The key selector.</param>
        /// <returns>A dictionary.</returns>
        public static IDictionary<TKey, T> FirstFromEachKey<T, TKey>(this IEnumerable<T> sequence, Func<T, TKey> keySelector)
        {
            return sequence.ToLookup(keySelector).ToDictionary(x => x.Key, x => x.First());
        }

        public static StringBuilder AppendIndented(this StringBuilder builder, string indentString, string value)
        {
            var lines = value == null ? new string[] { } : value.Split('\n');

            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                builder.Append(indentString).Append(line);

                if (i != lines.Length - 1)
                {
                    builder.Append('\n');
                }
            }

            return builder;
        }

        public static string GetGenericArgumentsCSharp(this MethodBase method)
        {
            var genericArguments = method.GetGenericArguments();
            if (genericArguments.Length == 0)
            {
                return string.Empty;
            }

            return string.Concat("<", string.Join(", ", genericArguments.Select(type => type.FullNameCSharp()).ToArray()), ">");
        }

        public static string FullNameCSharp(this Type type)
        {
            Guard.AgainstNull(type, "type");

            if (!type.IsGenericType)
            {
                return type.FullName;
            }

            var partName = type.FullName.Split('`')[0];
            var genericArgNames = type.GetGenericArguments().Select(arg => arg.FullNameCSharp()).ToArray();
            return string.Format(CultureInfo.InvariantCulture, "{0}<{1}>", partName, string.Join(", ", genericArgNames));
        }

        private class ZipEnumerable<TFirst, TSecond>
           : IEnumerable<Tuple<TFirst, TSecond>>
        {
            private IEnumerable<TFirst> firstCollection;
            private IEnumerable<TSecond> secondCollection;

            public ZipEnumerable(IEnumerable<TFirst> firstCollection, IEnumerable<TSecond> secondCollection)
            {
                this.firstCollection = firstCollection;
                this.secondCollection = secondCollection;
            }

            public IEnumerator<Tuple<TFirst, TSecond>> GetEnumerator()
            {
                return new ZipEnumerator(this.firstCollection, this.secondCollection);
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            private class ZipEnumerator
                : IEnumerator<Tuple<TFirst, TSecond>>
            {
                private IEnumerable<TFirst> firstCollection;
                private IEnumerable<TSecond> secondCollection;
                private IEnumerator<TFirst> firsts;
                private IEnumerator<TSecond> seconds;

                public ZipEnumerator(IEnumerable<TFirst> firstCollection, IEnumerable<TSecond> secondCollection)
                {
                    this.firstCollection = firstCollection;
                    this.secondCollection = secondCollection;
                    this.Reset();
                }

                public Tuple<TFirst, TSecond> Current
                {
                    get { return new Tuple<TFirst, TSecond>(this.firsts.Current, this.seconds.Current); }
                }

                object System.Collections.IEnumerator.Current
                {
                    get { return this.Current; }
                }

                public void Dispose()
                {
                    this.firsts.Dispose();
                    this.seconds.Dispose();
                    GC.SuppressFinalize(this);
                }

                public bool MoveNext()
                {
                    var firstsCanMove = this.firsts.MoveNext();
                    var secondsCanMove = this.seconds.MoveNext();

                    return firstsCanMove && secondsCanMove;
                }

                public void Reset()
                {
                    this.firsts = this.firstCollection.GetEnumerator();
                    this.seconds = this.secondCollection.GetEnumerator();
                }
            }
        }
    }
}