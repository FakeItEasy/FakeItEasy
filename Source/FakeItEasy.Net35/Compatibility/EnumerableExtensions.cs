namespace FakeItEasy
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Tidier.")]
    internal static partial class EnumerableExtensions
    {
        public static IEnumerable<TReturn> Zip<T, T2, TReturn>(this IEnumerable<T> sequence, IEnumerable<T2> otherSequence, Func<T, T2, TReturn> selector)
        {
            return new ZipEnumerable<T, T2, TReturn>(sequence, otherSequence, selector);
        }

        private class ZipEnumerable<TFirst, TSecond, TReturn>
            : IEnumerable<TReturn>
        {
            private readonly IEnumerable<TFirst> firstCollection;
            private readonly IEnumerable<TSecond> secondCollection;
            private readonly Func<TFirst, TSecond, TReturn> selector;

            public ZipEnumerable(IEnumerable<TFirst> firstCollection, IEnumerable<TSecond> secondCollection, Func<TFirst, TSecond, TReturn> selector)
            {
                this.firstCollection = firstCollection;
                this.secondCollection = secondCollection;
                this.selector = selector;
            }

            public IEnumerator<TReturn> GetEnumerator()
            {
                return new ZipEnumerator(this.firstCollection, this.secondCollection, this.selector);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            private class ZipEnumerator
                : IEnumerator<TReturn>
            {
                private readonly Func<TFirst, TSecond, TReturn> selector;
                private readonly IEnumerable<TFirst> firstCollection;
                private readonly IEnumerable<TSecond> secondCollection;
                private IEnumerator<TFirst> firsts;
                private IEnumerator<TSecond> seconds;

                public ZipEnumerator(IEnumerable<TFirst> firstCollection, IEnumerable<TSecond> secondCollection, Func<TFirst, TSecond, TReturn> selector)
                {
                    this.firstCollection = firstCollection;
                    this.secondCollection = secondCollection;
                    this.selector = selector;
                    this.Reset();
                }

                public TReturn Current
                {
                    get
                    {
                        return this.selector.Invoke(this.firsts.Current, this.seconds.Current);
                    }
                }

                object IEnumerator.Current
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