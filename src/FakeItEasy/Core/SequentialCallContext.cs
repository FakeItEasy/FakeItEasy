namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class SequentialCallContext
    {
        private readonly CallWriter callWriter;
        private readonly HashSet<FakeManager> fakeManagers;
        private readonly List<AssertedCall> assertedCalls;
        private int currentSequenceNumber;

        public SequentialCallContext(CallWriter callWriter)
        {
            Guard.AgainstNull(callWriter, nameof(callWriter));
            this.callWriter = callWriter;
            this.fakeManagers = new HashSet<FakeManager>();
            this.assertedCalls = new List<AssertedCall>();
            this.currentSequenceNumber = -1;
        }

        public void CheckNextCall(
            FakeManager fakeManager,
            Func<IFakeObjectCall, bool> callPredicate,
            string callDescription,
            Repeated repeatConstraint)
        {
            Guard.AgainstNull(fakeManager, nameof(fakeManager));
            Guard.AgainstNull(callPredicate, nameof(callPredicate));
            Guard.AgainstNull(callDescription, nameof(callDescription));
            Guard.AgainstNull(repeatConstraint, nameof(repeatConstraint));
            this.fakeManagers.Add(fakeManager);
            this.assertedCalls.Add(
                new AssertedCall { CallDescription = callDescription, RepeatDescription = repeatConstraint.ToString() });

            var allCalls = this.fakeManagers.SelectMany(f => f.GetRecordedCalls()).OrderBy(SequenceNumberManager.GetSequenceNumber).ToList();

            int matchedCallCount = 0;
            foreach (var currentCall in allCalls.SkipWhile(c => SequenceNumberManager.GetSequenceNumber(c) <= this.currentSequenceNumber))
            {
                if (repeatConstraint.Matches(matchedCallCount))
                {
                    return;
                }

                if (callPredicate(currentCall))
                {
                    matchedCallCount++;
                    this.currentSequenceNumber = SequenceNumberManager.GetSequenceNumber(currentCall);
                }
            }

            if (!repeatConstraint.Matches(matchedCallCount))
            {
                ThrowExceptionWhenAssertionFailed(this.assertedCalls, this.callWriter, allCalls);
            }
        }

        private static void ThrowExceptionWhenAssertionFailed(
            List<AssertedCall> assertedCalls, CallWriter callWriter, List<ICompletedFakeObjectCall> originalCallList)
        {
            var message = new StringBuilderOutputWriter();

            message.WriteLine();
            message.WriteLine();

            using (message.Indent())
            {
                message.Write("Assertion failed for the following calls:");
                message.WriteLine();

                using (message.Indent())
                {
                    foreach (var call in assertedCalls)
                    {
                        message.Write("'");
                        message.Write(call.CallDescription);
                        message.Write("' ");
                        message.Write("repeated ");
                        message.Write(call.RepeatDescription);
                        message.WriteLine();
                    }
                }

                message.Write("The calls were found but not in the correct order among the calls:");
                message.WriteLine();

                using (message.Indent())
                {
                    callWriter.WriteCalls(originalCallList, message);
                }
            }

            throw new ExpectationException(message.Builder.ToString());
        }

        private struct AssertedCall
        {
            public string CallDescription;
            public string RepeatDescription;
        }
    }
}
