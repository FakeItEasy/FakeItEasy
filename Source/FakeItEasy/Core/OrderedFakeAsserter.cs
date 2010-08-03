namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    internal class OrderedFakeAsserter
        : IFakeAsserter
    {
        private CallWriter callWriter;
        private Queue<IFakeObjectCall> calls;
        private IEnumerable<IFakeObjectCall> originalCallList;
        private List<AssertedCall> assertedCalls;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedFakeAsserter"/> class.
        /// </summary>
        /// <param name="calls">The calls.</param>
        /// <param name="callWriter">The call writer.</param>
        public OrderedFakeAsserter(IEnumerable<IFakeObjectCall> calls, CallWriter callWriter)
        {
            this.originalCallList = calls;
            this.calls = new Queue<IFakeObjectCall>(calls);
            this.callWriter = callWriter;

            this.assertedCalls = new List<AssertedCall>();
        }

        /// <summary>
        /// Asserts the was called.
        /// </summary>
        /// <param name="callPredicate">The call predicate.</param>
        /// <param name="callDescription">The call description.</param>
        /// <param name="repeatPredicate">The repeat predicate.</param>
        /// <param name="repeatDescription">The repeat description.</param>
        public virtual void AssertWasCalled(Func<IFakeObjectCall, bool> callPredicate, string callDescription, Func<int, bool> repeatPredicate, string repeatDescription)
        {
            this.assertedCalls.Add(new AssertedCall
            {
                CallDescription = callDescription,
                RepeatDescription = repeatDescription
            });

            this.RemoveCallsToSatisfyRepeatPredicate(callPredicate, repeatPredicate);
        }

        private void RemoveCallsToSatisfyRepeatPredicate(Func<IFakeObjectCall, bool> callPredicate, Func<int, bool> repeatPredicate)
        {
            int numberOfCallsFound = 0;

            while (!repeatPredicate(numberOfCallsFound))
            {
                if (this.calls.Count == 0)
                {
                    this.ThrowExceptionWhenAssertionFailed();
                }

                var currentCall = this.calls.Dequeue();

                if (callPredicate(currentCall))
                {
                    numberOfCallsFound++;
                }
            }
        }

        private void ThrowExceptionWhenAssertionFailed()
        {
            var message = new StringWriter();

            message.WriteLine(string.Empty);
            message.WriteLine(string.Empty);
            message.WriteLine("  Assertion failed for the following calls:");

            foreach (var call in this.assertedCalls)
            {
                message.Write("    '");
                message.Write(call.CallDescription);
                message.Write("' ");
                message.Write("repeated ");
                message.WriteLine(call.RepeatDescription);
            }

            message.WriteLine("  The calls where found but not in the correct order among the calls:");
            
            this.callWriter.WriteCalls(4, this.originalCallList, message);
            
            throw new ExpectationException(message.GetStringBuilder().ToString());
        }

        private struct AssertedCall
        {
            public string CallDescription;
            public string RepeatDescription;
        }
    }
}