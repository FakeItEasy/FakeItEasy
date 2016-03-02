namespace System.Collections.Concurrent
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    [Serializable]
    internal class ConcurrentQueue<T> : IEnumerable<T>
    {
        private readonly Queue<T> queue = new Queue<T>();

        public void Enqueue(T item)
        {
            lock (this.queue)
            {
                this.queue.Enqueue(item);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            lock (this.queue)
            {
                return this.queue.ToList().GetEnumerator();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
