namespace FakeItEasy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class ConditionalWeakTable<TKey, TValue>
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