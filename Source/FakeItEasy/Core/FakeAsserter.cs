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
        
        public FakeAsserter(IEnumerable<IFakeObjectCall> calls, CallWriter callWriter)
        {
            this.Calls = calls;
            this.callWriter = callWriter;
        }

        public delegate IFakeAsserter Factory(IEnumerable<IFakeObjectCall> calls);

        protected IEnumerable<IFakeObjectCall> Calls { get; set; }

        public virtual void AssertWasCalled(Func<IFakeObjectCall, bool> callPredicate, string callDescription, Func<int, bool> repeatPredicate, string repeatDescription)
        {
            var repeat = this.Calls.Count(callPredicate);

            if (!repeatPredicate(repeat))
            {
                var message = this.CreateExceptionMessage(callDescription, repeatDescription, repeat);

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

        private string CreateExceptionMessage(string callDescription, string repeatDescription, int repeat)
        {
            var outputWriter = new StringBuilderOutputWriter();
            outputWriter.WriteLine();

            using (outputWriter.Indent())
            {
                AppendCallDescription(callDescription, outputWriter);
                this.AppendExpectation(repeatDescription, repeat, outputWriter);
                this.AppendCallList(outputWriter);
                outputWriter.WriteLine();
            }

            return outputWriter.Builder.ToString();
        }

        private void AppendExpectation(string repeatDescription, int repeat, IOutputWriter writer)
        {
            writer.Write("Expected to find it {0} ", repeatDescription);

            if (this.Calls.Any())
            {
                writer.Write("but found it #{0} times among the calls:", repeat);
            }
            else
            {
                writer.Write("but no calls were made to the fake object.");
            }

            writer.WriteLine();
        }

        private void AppendCallList(IOutputWriter writer)
        {
            using (writer.Indent())
            {
                this.callWriter.WriteCalls(this.Calls, writer);
            }
        }
    }
}