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
    }
}