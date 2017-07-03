namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class FakeAsserter : IFakeAsserter
    {
        private readonly CallWriter callWriter;
        private readonly IEnumerable<ICompletedFakeObjectCall> calls;

        public FakeAsserter(IEnumerable<ICompletedFakeObjectCall> calls, CallWriter callWriter)
        {
            Guard.AgainstNull(calls, nameof(calls));
            Guard.AgainstNull(callWriter, nameof(callWriter));

            this.calls = calls;
            this.callWriter = callWriter;
        }

        public delegate IFakeAsserter Factory(IEnumerable<ICompletedFakeObjectCall> calls);

        public virtual void AssertWasCalled(
            Func<ICompletedFakeObjectCall, bool> callPredicate, Action<IOutputWriter> callDescriber, Repeated repeatConstraint)
        {
            var lastCall = this.calls.LastOrDefault();
            int lastSequenceNumber = lastCall != null ? SequenceNumberManager.GetSequenceNumber(lastCall) : -1;

            bool IsBeforeAssertionStart(ICompletedFakeObjectCall call) => SequenceNumberManager.GetSequenceNumber(call) <= lastSequenceNumber;

            var matchedCallCount = this.calls.Count(c => IsBeforeAssertionStart(c) && callPredicate(c));
            if (!repeatConstraint.Matches(matchedCallCount))
            {
                var description = new StringBuilderOutputWriter();
                callDescriber.Invoke(description);

                var message = CreateExceptionMessage(this.calls.Where(IsBeforeAssertionStart), this.callWriter, description.Builder.ToString(), repeatConstraint.ToString(), matchedCallCount);
                throw new ExpectationException(message);
            }
        }

        private static string CreateExceptionMessage(
            IEnumerable<IFakeObjectCall> calls, CallWriter callWriter, string callDescription, string repeatDescription, int matchedCallCount)
        {
            var writer = new StringBuilderOutputWriter();
            writer.WriteLine();

            using (writer.Indent())
            {
                AppendCallDescription(callDescription, writer);
                AppendExpectation(calls, repeatDescription, matchedCallCount, writer);
                AppendCallList(calls, callWriter, writer);
                writer.WriteLine();
            }

            return writer.Builder.ToString();
        }

        private static void AppendCallDescription(string callDescription, IOutputWriter writer)
        {
            writer.WriteLine();
            writer.Write("Assertion failed for the following call:");
            writer.WriteLine();

            using (writer.Indent())
            {
                writer.Write(callDescription);
                writer.WriteLine();
            }
        }

        private static void AppendExpectation(IEnumerable<IFakeObjectCall> calls, string repeatDescription, int matchedCallCount, IOutputWriter writer)
        {
            writer.Write("Expected to find it {0} ", repeatDescription);

            if (calls.Any())
            {
                writer.Write("but found it #{0} times among the calls:", matchedCallCount);
            }
            else
            {
                writer.Write("but no calls were made to the fake object.");
            }

            writer.WriteLine();
        }

        private static void AppendCallList(IEnumerable<IFakeObjectCall> calls, CallWriter callWriter, IOutputWriter writer)
        {
            using (writer.Indent())
            {
                callWriter.WriteCalls(calls, writer);
            }
        }
    }
}
