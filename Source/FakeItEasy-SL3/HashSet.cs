namespace System.Collections.Generic
{
    public class HashSet<T>
        : IEnumerable<T>, ICollection<T>
    {
        private Dictionary<T, T> dictionary = new Dictionary<T,T>();

        public IEnumerator<T> GetEnumerator()
        {
            return this.dictionary.Keys.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public void Add(T item)
        {
            this.dictionary.Add(item, item);
        }

        public void Clear()
        {
            this.dictionary.Clear();
        }

        public bool Contains(T item)
        {
            return this.dictionary.ContainsKey(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            this.dictionary.Keys.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return this.dictionary.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(T item)
        {
            return this.dictionary.Remove(item);
        }
    }
}
