namespace FakeItEasy.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Implementation of a thread-safe list that also maintains
    /// item order.
    /// </summary>
    /// <typeparam name="T">
    /// The type of element held in the list.
    /// </typeparam>
    /// <seealso cref="System.Collections.Generic.IList{T}" />
    /// <seealso cref="System.Collections.IList" />
    /// <remarks>
    /// <para>
    /// This list differs from the framework
    /// <see cref="T:System.Collections.Generic.SynchronizedCollection{T}"/>
    /// in that enumeration is thread-safe. It differs from the framework
    /// <see cref="T:System.Collections.Concurrent.ConcurrentBag{T}"/> in
    /// that the order of elements is maintained like an
    /// <see cref="IList{T}"/>.
    /// </para>
    /// <para>
    /// The type is also serializable, unlike the other concurrent collection
    /// types noted.
    /// </para>
    /// </remarks>
    [Serializable]
    internal class ConcurrentList<T> : IList<T>, ISerializable
    {
        /// <summary>
        /// The synchronization object for ensuring the list is thread-safe.
        /// </summary>
        [NonSerialized]
        private readonly object syncRoot;

        /// <summary>
        /// The actual list of items.
        /// </summary>
        [NonSerialized]
        private readonly List<T> items;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrentList{T}"/> class.
        /// </summary>
        public ConcurrentList()
        {
            this.syncRoot = new object();
            this.items = new List<T>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrentList{T}"/> class
        /// with the specified initial capacity.
        /// </summary>
        /// <param name="capacity">
        /// The capacity the list should have to start.
        /// </param>
        public ConcurrentList(int capacity)
        {
            this.syncRoot = new object();
            this.items = new List<T>(capacity);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrentList{T}"/> class
        /// with elements copied from an initial collection.
        /// </summary>
        /// <param name="collection">
        /// The collection containing the elements to copy into this list.
        /// </param>
        public ConcurrentList(IEnumerable<T> collection)
        {
            this.syncRoot = new object();
            this.items = new List<T>(collection);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrentList{T}"/> class
        /// for serialization.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> containing the object data.</param>
        /// <param name="context">The source for this deserialization.</param>
        protected ConcurrentList(SerializationInfo info, StreamingContext context)
        {
            this.syncRoot = new object();
            this.items = (List<T>)info.GetValue("items", typeof(List<T>));
        }

        /// <summary>
        /// Gets the number of elements contained in the
        /// <see cref="System.Collections.Generic.ICollection{T}" />.
        /// </summary>
        /// <value>
        /// An <see cref="int"/> with the number of elements.
        /// </value>
        public int Count
        {
            get
            {
                lock (this.syncRoot)
                {
                    return this.items.Count;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the
        /// <see cref="System.Collections.Generic.ICollection{T}" /> is read-only.
        /// </summary>
        /// <value>
        /// Always returns <see langword="false" />.
        /// </value>
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <returns>
        /// The <typeparamref name="T"/> at the index.
        /// </returns>
        /// <param name="index">The index of the element to retrieve.</param>
        public T this[int index]
        {
            get
            {
                lock (this.syncRoot)
                {
                    return this.items[index];
                }
            }

            set
            {
                lock (this.syncRoot)
                {
                    this.items[index] = value;
                }
            }
        }

        /// <summary>
        /// Adds the specified item to the list.
        /// </summary>
        /// <param name="item">
        /// The item to add.
        /// </param>
        public void Add(T item)
        {
            lock (this.syncRoot)
            {
                this.items.Add(item);
            }
        }

        /// <summary>
        /// Removes all items from the
        /// <see cref="System.Collections.Generic.ICollection{T}" />.
        /// </summary>
        public void Clear()
        {
            lock (this.syncRoot)
            {
                this.items.Clear();
            }
        }

        /// <summary>
        /// Determines whether the list contains the specified item.
        /// </summary>
        /// <param name="item">
        /// The value to find.
        /// </param>
        /// <returns>
        /// <see langword="true" /> if the value is found; <see langword="false" /> if not.
        /// </returns>
        public bool Contains(T item)
        {
            lock (this.syncRoot)
            {
                return this.items.Contains(item);
            }
        }

        /// <summary>
        /// Copies the contents of this list into an array.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional array that is the destination of the elements copied from this list.
        /// </param>
        /// <param name="arrayIndex">
        /// The zero-based index in the array at which copying begins.
        /// </param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (this.syncRoot)
            {
                this.items.CopyTo(array, arrayIndex);
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="System.Collections.Generic.IEnumerator{T}" /> that can be
        /// used to iterate through the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            lock (this.syncRoot)
            {
                // Create a copy of the list so the enumeration won't
                // change as other threads add/remove items.
                var enumerable = new List<T>(this.items);
                return enumerable.GetEnumerator();
            }
        }

        /// <summary>
        /// Populates a <see cref="SerializationInfo"/> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> to populate with data.</param>
        /// <param name="context">The destination for this serialization.</param>
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            lock (this.syncRoot)
            {
                info.AddValue("items", this.items, typeof(List<T>));
            }
        }

        /// <summary>
        /// Determines the index of a specific item in the list.
        /// </summary>
        /// <param name="item">The item to locate in the list.</param>
        /// <returns>The index of value if found in the list; otherwise, -1.</returns>
        public int IndexOf(T item)
        {
            lock (this.syncRoot)
            {
                return this.items.IndexOf(item);
            }
        }

        /// <summary>
        /// Inserts an item to the list at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which value should be inserted.</param>
        /// <param name="item">The value to insert.</param>
        public void Insert(int index, T item)
        {
            lock (this.syncRoot)
            {
                this.items.Insert(index, item);
            }
        }

        /// <summary>
        /// Removes the specified value from the list.
        /// </summary>
        /// <param name="item">The value to remove.</param>
        /// <returns>
        /// <see langword="true" /> if the value was successfully removed;
        /// <see langword="false" /> if not.
        /// </returns>
        public bool Remove(T item)
        {
            lock (this.syncRoot)
            {
                return this.items.Remove(item);
            }
        }

        /// <summary>
        /// Removes the element at the specified index.
        /// </summary>
        /// <param name="index">The index of the element to remove.</param>
        public void RemoveAt(int index)
        {
            lock (this.syncRoot)
            {
                this.items.RemoveAt(index);
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="System.Collections.IEnumerator" /> object that can
        /// be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            lock (this.syncRoot)
            {
                // Create a copy of the list so the enumeration won't
                // change as other threads add/remove items.
                var enumerable = this.items.ToArray();
                return enumerable.GetEnumerator();
            }
        }
    }
}
