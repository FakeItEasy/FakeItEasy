namespace FakeItEasy
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    // Minimal implementation for compatibility with .net 3.5,
    // has potential memory leaks and performance issues but should
    // be fine for this particular use.
#pragma warning disable 1591
    public class ConditionalWeakTable<TKey, TValue>
    {
        private List<Tuple<WeakReference, TValue>> entries = new List<Tuple<WeakReference, TValue>>();

        public bool TryGetValue(TKey key, out TValue result)
        {
            this.Purge();
            foreach (var entry in this.entries)
            {
                if (KeyEquals(key, entry.Item1))
                {
                    result = entry.Item2;
                    return true;
                }
            }

            result = default(TValue);
            return false;
        }

        public void Add(TKey key, TValue value)
        {
            this.Purge();
            this.entries.Add(Tuple.Create(new WeakReference(key), value));
        }

        private static bool KeyEquals(TKey key, WeakReference keyReference)
        {
            var strongKeyReference = keyReference.Target;

            return strongKeyReference != null && object.ReferenceEquals(key, strongKeyReference);
        }

        private void Purge()
        {
            this.entries = (from entry in this.entries
                            where entry.Item1.IsAlive
                            select entry).ToList();
        }
    }
}

namespace System.ComponentModel.Composition
{
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes", Justification = "Mimicks net40 BCL type.")]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Tidier.")]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = true)]
    public class InheritedExportAttribute
        : Attribute
    {
    }

    [SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes", Justification = "Mimicks net40 BCL type.")]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Tidier.")]
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public class ImportManyAttribute
        : Attribute
    {
    }
}

namespace System
{
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Tidier.")]
    public static class Tuple
    {
        public static Tuple<T1, T2> Create<T1, T2>(T1 item1, T2 item2)
        {
            return new Tuple<T1, T2>(item1, item2);
        }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Tidier.")]
    public class Tuple<T1, T2>
    {
        public Tuple(T1 item1, T2 item2)
        {
            this.Item1 = item1;
            this.Item2 = item2;
        }

        public T1 Item1 { get; private set; }

        public T2 Item2 { get; private set; }
    }
}

namespace System.Linq
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Tidier.")]
    public static class EnumerableExtensions
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

            Collections.IEnumerator Collections.IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            private class ZipEnumerator
                : IEnumerator<TReturn>
            {
                private readonly Func<TFirst, TSecond, TReturn> selector;
                private IEnumerable<TFirst> firstCollection;
                private IEnumerable<TSecond> secondCollection;
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
#pragma warning restore 1591
}