namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;

    internal class OrderedFakeAsserter
        : IFakeAsserter
    {
        private IFakeAsserter asserter;
        private Queue<IFakeObjectCall> calls;

        public OrderedFakeAsserter(IEnumerable<IFakeObjectCall> calls, IFakeAsserter asserter)
        {
            this.asserter = asserter;
            this.calls = new Queue<IFakeObjectCall>(calls);
        }

        public void AssertWasCalled(Func<IFakeObjectCall, bool> callPredicate, string callDescription, Func<int, bool> repeatPredicate, string repeatDescription)
        {
            this.asserter.AssertWasCalled(callPredicate, callDescription, repeatPredicate, repeatDescription);

            this.RemoveCallsToSatisfyRepeatPredicate(callPredicate, repeatPredicate);
        }

        private void RemoveCallsToSatisfyRepeatPredicate(Func<IFakeObjectCall, bool> callPredicate, Func<int, bool> repeatPredicate)
        {
            int numberOfCallsFound = 0;

            while (!repeatPredicate(numberOfCallsFound))
            {
                if (this.calls.Count == 0)
                {
                    throw new ExpectationException();
                }

                var currentCall = this.calls.Dequeue();

                if (callPredicate(currentCall))
                {
                    numberOfCallsFound++;
                }
            }
        }
    }
}
