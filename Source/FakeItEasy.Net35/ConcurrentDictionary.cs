namespace System.Collections.Concurrent
{
    using System.Collections.Generic;

    internal class ConcurrentDictionary<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> dictionary;

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

        public bool ContainsKey(TKey key)
        {
            return this.dictionary.ContainsKey(key);
        }

        public bool TryRemove(TKey key, out TValue value)
        {
            lock (this.dictionary)
            {
                if (this.dictionary.TryGetValue(key, out value))
                {
                    this.dictionary.Remove(key);
                    return true;
                }

                return false;
            }
        }
    }
}
