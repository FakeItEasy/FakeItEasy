namespace System.Collections.Concurrent
{
    using System.Collections.Generic;

    internal class ConcurrentDictionary<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> dictionary;

        public ConcurrentDictionary()
        {
            this.dictionary = new Dictionary<TKey, TValue>();
        }

        public ConcurrentDictionary(IEqualityComparer<TKey> comparer)
        {
            this.dictionary = new Dictionary<TKey, TValue>(comparer);
        }

        public TValue this[TKey key]
        {
            set
            {
                lock (this.dictionary)
                {
                    this.dictionary[key] = value;
                }
            }
        }

        public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
        {
            TValue value;

            lock (this.dictionary)
            {
                if (this.dictionary.TryGetValue(key, out value))
                {
                    return value;
                }

                this.dictionary[key] = value = valueFactory(key);
            }

            return value;
        }

        public bool TryRemove(TKey key, out TValue value)
        {
            lock (this.dictionary)
            {
                return this.dictionary.TryGetValue(key, out value) && this.dictionary.Remove(key);
            }
        }
    }
}