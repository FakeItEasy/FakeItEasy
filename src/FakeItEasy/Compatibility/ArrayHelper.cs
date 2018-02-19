namespace FakeItEasy.Compatibility
{
    internal static class ArrayHelper
    {
#if FEATURE_ARRAY_EMPTY
        public static T[] Empty<T>() => System.Array.Empty<T>();
#else
        public static T[] Empty<T>() => EmptyArrayCache<T>.EmptyArray;

        private static class EmptyArrayCache<T>
        {
            public static readonly T[] EmptyArray = new T[0];
        }
#endif
    }
}
