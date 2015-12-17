namespace FakeItEasy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class ConditionalWeakTable<TKey, TValue>
    {
        private List<Entry> entries = new List<Entry>();

        public bool TryGetValue(TKey key, out TValue result)
        {
            this.Purge();
            foreach (var entry in this.entries)
            {
                if (KeyEquals(key, entry.Reference))
                {
                    result = entry.Value;
                    return true;
                }
            }

            result = default(TValue);
            return false;
        }

        public void Add(TKey key, TValue value)
        {
            this.Purge();
            this.entries.Add(new Entry(new WeakReference(key), value));
        }

        private static bool KeyEquals(TKey key, WeakReference keyReference)
        {
            var strongKeyReference = keyReference.Target;

            return strongKeyReference != null && object.ReferenceEquals(key, strongKeyReference);
        }

        private void Purge()
        {
            this.entries = (from entry in this.entries
                where entry.Reference.IsAlive
                select entry).ToList();
        }

        private class Entry
        {
            public Entry(WeakReference reference, TValue value)
            {
                this.Reference = reference;
                this.Value = value;
            }

            public WeakReference Reference { get; private set; }

            public TValue Value { get; private set; }
        }
    }
}