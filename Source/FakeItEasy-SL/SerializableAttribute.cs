namespace FakeItEasy
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    [Conditional("Near_a_tree_by_a_river_theres_a_hole_in_the_ground")]
    public class SerializableAttribute
        : Attribute
    {
    }

    [Conditional("Near_a_tree_by_a_river_theres_a_hole_in_the_ground")]
    public class NonSerializedAttribute
        : Attribute
    {
    }

    public static class Tuple
    {
        public static Tuple<T1, T2> Create<T1, T2>(T1 item1, T2 item2)
        {
            return new Tuple<T1, T2>(item1, item2);
        }
    }

    public class Tuple<T1, T2>
    {
        public Tuple(T1 item1, T2 item2)
        {
            this.Item1 = item1;
            this.Item2 = item2;
        }

        public T1 Item1 { get; set; }
        
        public T2 Item2 { get; set; }
    }

    public class HashSet<T>
    {
        private readonly Dictionary<T, object> dictionary = new Dictionary<T, object>();


        public bool Contains(T value)
        {
            return this.dictionary.ContainsKey(value);
        }

        public void Add(T value)
        {
            this.dictionary.Add(value, null);
        }

        public bool Remove(T value)
        {
            return this.dictionary.Remove(value);
        }
    }
}
