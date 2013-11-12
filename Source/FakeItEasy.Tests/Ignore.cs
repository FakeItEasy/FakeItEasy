namespace FakeItEasy.Tests
{
    using System.Diagnostics.CodeAnalysis;

    public static class Ignore
    {
        public static ValueCarrier<T> This<T>()
        {
            return new ValueCarrier<T>();
        }

        public class ValueCarrier<T>
        {
            [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Required for testing.")]
            public T Value;
        }
    }
}