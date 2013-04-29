namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;

    internal class FakeAsserter : IFakeAsserter
    {
        private readonly CallWriter callWriter;
        private readonly IEnumerable<IFakeObjectCall> calls;

        public FakeAsserter(IEnumerable<IFakeObjectCall> calls, CallWriter callWriter)
        {
            Guard.AgainstNull(calls, "calls");
            Guard.AgainstNull(callWriter, "callWriter");

            this.calls = calls;
            this.callWriter = callWriter;
        }

        public delegate IFakeAsserter Factory(IEnumerable<IFakeObjectCall> calls);

        public virtual void AssertWasCalled(
            Func<IFakeObjectCall, bool> callPredicate, string callDescription, Func<int, bool> repeatPredicate, string repeatDescription)
        {
            var matchedCallCount = this.calls.Count(callPredicate);
            if (!repeatPredicate(matchedCallCount))
            {
                var message = CreateExceptionMessage(this.calls, this.callWriter, callDescription, repeatDescription, matchedCallCount);
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