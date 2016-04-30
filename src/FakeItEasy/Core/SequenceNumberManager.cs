namespace FakeItEasy.Core
{
    using System.Runtime.CompilerServices;
    using System.Threading;

    internal static class SequenceNumberManager
    {
        private static readonly ConditionalWeakTable<ICompletedFakeObjectCall, StrongBox<int>> SequenceNumbers =
            new ConditionalWeakTable<ICompletedFakeObjectCall, StrongBox<int>>();

        private static int lastSequenceNumber;

        public static int RecordSequenceNumber(ICompletedFakeObjectCall call)
        {
            int sequenceNumber = GetNextSequenceNumber();
            var box = SequenceNumbers.GetOrCreateValue(call);
            box.Value = sequenceNumber;
            return sequenceNumber;
        }

        public static int GetSequenceNumber(ICompletedFakeObjectCall call) => SequenceNumbers.GetOrCreateValue(call).Value;

        private static int GetNextSequenceNumber() => Interlocked.Increment(ref lastSequenceNumber);
    }
}
