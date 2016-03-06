namespace FakeItEasy.Core
{
    using System.Threading;

    internal static class SequenceNumberManager
    {
        private static int sequenceNumber;

        public static int Next()
        {
            return Interlocked.Increment(ref sequenceNumber);
        }
    }
}
