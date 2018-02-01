namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    internal class SequentialCallContext
    {
        private readonly CallWriter callWriter;
        private readonly StringBuilderOutputWriter.Factory outputWriterFactory;
        private readonly HashSet<FakeManager> fakeManagers;
        private readonly List<AssertedCall> assertedCalls;
        private int currentSequenceNumber;

        public SequentialCallContext(CallWriter callWriter, StringBuilderOutputWriter.Factory outputWriterFactory)
        {
            Guard.AgainstNull(callWriter, nameof(callWriter));
            this.callWriter = callWriter;
            this.outputWriterFactory = outputWriterFactory;
            this.fakeManagers = new HashSet<FakeManager>();
            this.assertedCalls = new List<AssertedCall>();
            this.currentSequenceNumber = -1;
        }

        public void CheckNextCall(
            FakeManager fakeManager,
            Func<IFakeObjectCall, bool> callPredicate,
            Action<IOutputWriter> callDescriber,
            CallCountConstraint callCountConstraint)
        {
            Guard.AgainstNull(fakeManager, nameof(fakeManager));
            Guard.AgainstNull(callPredicate, nameof(callPredicate));
            Guard.AgainstNull(callDescriber, nameof(callDescriber));
            Guard.AgainstNull(callCountConstraint, nameof(callCountConstraint));
            this.fakeManagers.Add(fakeManager);
            this.assertedCalls.Add(
                new AssertedCall { CallDescriber = callDescriber, MatchingCountDescription = callCountConstraint.ToString() });

            var allCalls = this.fakeManagers.SelectMany(f => f.GetRecordedCalls()).OrderBy(SequenceNumberManager.GetSequenceNumber).ToList();

            int matchedCallCount = 0;
            foreach (var currentCall in allCalls.SkipWhile(c => SequenceNumberManager.GetSequenceNumber(c) <= this.currentSequenceNumber))
            {
                if (callCountConstraint.Matches(matchedCallCount))
                {
                    return;
                }

                if (callPredicate(currentCall))
                {
                    matchedCallCount++;
                    this.currentSequenceNumber = SequenceNumberManager.GetSequenceNumber(currentCall);
                }
            }

            if (!callCountConstraint.Matches(matchedCallCount))
            {
                this.ThrowExceptionWhenAssertionFailed(allCalls);
            }
        }

        private void ThrowExceptionWhenAssertionFailed(List<ICompletedFakeObjectCall> originalCallList)
        {
            var message = this.outputWriterFactory(new StringBuilder());

            message.WriteLine();
            message.WriteLine();

            using (message.Indent())
            {
                message.Write("Assertion failed for the following calls:");
                message.WriteLine();

                using (message.Indent())
                {
                    foreach (var call in this.assertedCalls)
                    {
                        message.Write("'");
                        call.CallDescriber.Invoke(message);
                        message.Write("' ");
                        message.Write(call.MatchingCountDescription);
                        message.WriteLine();
                    }
                }

                message.Write("The calls were found but not in the correct order among the calls:");
                message.WriteLine();

                using (message.Indent())
                {
                    this.callWriter.WriteCalls(originalCallList, message);
                }
            }

            throw new ExpectationException(message.Builder.ToString());
        }

        private struct AssertedCall
        {
            public Action<IOutputWriter> CallDescriber;
            public string MatchingCountDescription;
        }
    }
}
