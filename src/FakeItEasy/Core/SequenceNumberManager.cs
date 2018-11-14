namespace FakeItEasy.Core
{
    using System.Threading;

    internal static class SequenceNumberManager
    {
        private static int lastSequenceNumber;

        public static int GetNextSequenceNumber() => Interlocked.Increment(ref lastSequenceNumber);
    }
}
