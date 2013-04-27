namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;

    internal class OrderedFakeAsserter : IFakeAsserter
    {
        private readonly IEnumerable<IFakeObjectCall> originalCallList;
        private readonly CallWriter callWriter;
        private readonly Queue<IFakeObjectCall> calls;
        private readonly List<AssertedCall> assertedCalls = new List<AssertedCall>();

        public OrderedFakeAsserter(IEnumerable<IFakeObjectCall> calls, CallWriter callWriter)
        {
            Guard.AgainstNull(calls, "calls");
            Guard.AgainstNull(callWriter, "callWriter");

            this.originalCallList = calls;
            this.calls = new Queue<IFakeObjectCall>(calls);
            this.callWriter = callWriter;
        }

        public virtual void AssertWasCalled(
            Func<IFakeObjectCall, bool> callPredicate, string callDescription, Func<int, bool> repeatPredicate, string repeatDescription)
        {
            this.assertedCalls.Add(new AssertedCall { CallDescription = callDescription, RepeatDescription = repeatDescription });
            var matchedCallCount = 0;
            while (!repeatPredicate(matchedCallCount))
            {
                if (this.calls.Count == 0)
                {
                    ThrowExceptionWhenAssertionFailed(this.assertedCalls, this.callWriter, this.originalCallList);
                }

                var currentCall = this.calls.Dequeue();
                if (callPredicate(currentCall))
                {
                    matchedCallCount++;
                }
            }
        }

        private static void ThrowExceptionWhenAssertionFailed(
            List<AssertedCall> assertedCalls, CallWriter callWriter, IEnumerable<IFakeObjectCall> originalCallList)
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

                message.Write("The calls where found but not in the correct order among the calls:");
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