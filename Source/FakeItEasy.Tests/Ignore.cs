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
            [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Testing only.")]
            public T Value;
        }
    }
}