namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Concurrent;

    /// <summary>
    /// Provides a non-expiring cache of values, accessible by key. If there's no value for a key, one will be created, cached, and returned.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <remarks>
    /// Instances of <see cref="Cache{TKey, TValue}"/> are thread-safe, in that there will be no errors thrown if one is accessed by 
    /// multiple threads at once. However, there is no protection against calling the value-provider method more than once during
    /// concurrent accesses; the last value produced will be cached.
    /// However, once a value has been created and cached, the value provider will not be called again for that key.
    /// </remarks>
    internal class Cache<TKey, TValue>
    {
        private readonly ConcurrentDictionary<TKey, TValue> cachedValues;

        private readonly Func<TKey, TValue> valueFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="Cache{TKey, TValue}"/> class.
        /// </summary>
        /// <param name="valueFactory">Will be called to create a new instance of <typeparamref name="TValue"/> when
        /// no cached value is found for a given key. If <see cref="this"/> is called concurrently for the same key, <see cref="valueFactory"/> 
        /// may be called more than once with the same key value. See the Remarks for more details.</param>
        public Cache(Func<TKey, TValue> valueFactory)
        {
            this.cachedValues = new ConcurrentDictionary<TKey, TValue>();
            this.valueFactory = valueFactory;
        }

        /// <summary>
        /// Gets the cached value for <paramref name="key"/>. If no value is present, will use the valueFactory provided in the constructor
        /// to create one, which will then be cached.
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <returns>The cached value for the key, or a newly-created and cached value.</returns>
        public TValue this[TKey key]
        {
            get { return this.cachedValues.GetOrAdd(key, this.valueFactory); }
        }
    }
}
