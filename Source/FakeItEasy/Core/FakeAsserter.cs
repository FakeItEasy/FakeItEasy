namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;

    internal class FakeAsserter : FakeItEasy.Core.IFakeAsserter
    {
        protected IEnumerable<IFakeObjectCall> calls;
        private CallWriter callWriter;

        public delegate IFakeAsserter Factory(IEnumerable<IFakeObjectCall> calls);
        
        public FakeAsserter(IEnumerable<IFakeObjectCall> calls, CallWriter callWriter)
        {
            this.calls = calls;
            this.callWriter = callWriter;
        }

        public virtual void AssertWasCalled(Func<IFakeObjectCall, bool> callPredicate, string callDescription, Func<int, bool> repeatPredicate, string repeatDescription)
        {
            var repeat = this.calls.Count(callPredicate);

            if (!repeatPredicate(repeat))
            {
                var message = CreateExceptionMessage(callDescription, repeatDescription, repeat);

                throw new ExpectationException(message);
            }
        }

        private string CreateExceptionMessage(string callDescription, string repeatDescription, int repeat)
        {
            var messageWriter = new StringWriter(CultureInfo.InvariantCulture);

            messageWriter.WriteLine();
            AppendCallDescription(callDescription, messageWriter);
            this.AppendExpectation(repeatDescription, repeat, messageWriter);
            this.AppendCallList(messageWriter);
            messageWriter.WriteLine();

            return messageWriter.GetStringBuilder().ToString();
        }

        private static void AppendCallDescription(string callDescription, StringWriter writer)
        {
            writer.WriteLine();
            writer.WriteLine("  Assertion failed for the following call:");
            writer.WriteLine("    '{0}'", callDescription);
        }

        private void AppendExpectation(string repeatDescription, int repeat, StringWriter writer)
        {
            writer.Write("  Expected to find it {0} ", repeatDescription);

            if (this.calls.Any())
            {
                writer.WriteLine("but found it #{0} times among the calls:", repeat);
            }
            else
            {
                writer.WriteLine("but no calls were made to the fake object.");
            }
        }

        private void AppendCallList(StringWriter writer)
        {
            this.callWriter.WriteCalls(4, this.calls, writer);
        }
    }
}