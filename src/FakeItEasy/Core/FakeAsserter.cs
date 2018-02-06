namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    internal class FakeAsserter : IFakeAsserter
    {
        private readonly CallWriter callWriter;
        private readonly StringBuilderOutputWriter.Factory outputWriterFactory;
        private readonly IEnumerable<ICompletedFakeObjectCall> calls;

        public FakeAsserter(IEnumerable<ICompletedFakeObjectCall> calls, CallWriter callWriter, StringBuilderOutputWriter.Factory outputWriterFactory)
        {
            Guard.AgainstNull(calls, nameof(calls));
            Guard.AgainstNull(callWriter, nameof(callWriter));

            this.calls = calls;
            this.callWriter = callWriter;
            this.outputWriterFactory = outputWriterFactory;
        }

        public delegate IFakeAsserter Factory(IEnumerable<ICompletedFakeObjectCall> calls);

        public virtual void AssertWasCalled(
            Func<ICompletedFakeObjectCall, bool> callPredicate, Action<IOutputWriter> callDescriber, CallCountConstraint callCountConstraint)
        {
            var lastCall = this.calls.LastOrDefault();
            int lastSequenceNumber = lastCall != null ? SequenceNumberManager.GetSequenceNumber(lastCall) : -1;

            bool IsBeforeAssertionStart(ICompletedFakeObjectCall call) => SequenceNumberManager.GetSequenceNumber(call) <= lastSequenceNumber;

            var matchedCallCount = this.calls.Count(c => IsBeforeAssertionStart(c) && callPredicate(c));
            if (!callCountConstraint.Matches(matchedCallCount))
            {
                var description = this.outputWriterFactory(new StringBuilder());
                callDescriber.Invoke(description);

                var message = this.CreateExceptionMessage(this.calls.Where(IsBeforeAssertionStart), description.Builder.ToString(), callCountConstraint.ToString(), matchedCallCount);
                throw new ExpectationException(message);
            }
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

        private static void AppendExpectation(IEnumerable<IFakeObjectCall> calls, string callCountDescription, int matchedCallCount, IOutputWriter writer)
        {
            writer.Write("Expected to find it {0} ", callCountDescription);

            if (calls.Any())
            {
                switch (matchedCallCount)
                {
                    case 0:
                        writer.Write("but didn't find it among the calls:");
                        break;
                    case 1:
                        writer.Write("but found it once among the calls:");
                        break;
                    case 2:
                        writer.Write("but found it twice among the calls:");
                        break;
                    default:
                        writer.Write("but found it {0} times among the calls:", matchedCallCount);
                        break;
                }
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

        private string CreateExceptionMessage(
            IEnumerable<IFakeObjectCall> calls, string callDescription, string callCountDescription, int matchedCallCount)
        {
            var writer = this.outputWriterFactory(new StringBuilder());
            writer.WriteLine();

            using (writer.Indent())
            {
                AppendCallDescription(callDescription, writer);
                AppendExpectation(calls, callCountDescription, matchedCallCount, writer);
                AppendCallList(calls, this.callWriter, writer);
                writer.WriteLine();
            }

            return writer.Builder.ToString();
        }
    }
}
