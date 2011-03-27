namespace FakeItEasy.Tests
{
    public static class Ignore
    {
        public class ValueCarrier<T>
        {
            public T Value;
        }

        public static ValueCarrier<T> This<T>()
        {
            return new ValueCarrier<T>();
        }
    }
}