namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;
    using FakeAsserterFactory = System.Func<System.Collections.Generic.IEnumerable<FakeItEasy.Core.IFakeObjectCall>, FakeItEasy.Core.FakeAsserter>;

    internal class OrderedFakeAsserterFactory
    {
        private FakeAsserterFactory asserterFactory;
        private OrderedFakeAsserter orderedAsserter;

        public OrderedFakeAsserterFactory(FakeAsserterFactory asserterFactory, OrderedFakeAsserter orderedAsserter)
        {
            this.asserterFactory = asserterFactory;
            this.orderedAsserter = orderedAsserter;
        }

        public IFakeAsserter CreateAsserter(IEnumerable<IFakeObjectCall> calls)
        {
            return new CompositeAsserter 
            {
                Asserter = this.asserterFactory(calls),
                OrderedAsserter = this.orderedAsserter
            };
        }

        private class CompositeAsserter
            : IFakeAsserter
        {
            public FakeAsserter Asserter;
            public OrderedFakeAsserter OrderedAsserter;

            public void AssertWasCalled(Func<IFakeObjectCall, bool> callPredicate, string callDescription, Func<int, bool> repeatPredicate, string repeatDescription)
            {
                this.Asserter.AssertWasCalled(callPredicate, callDescription, repeatPredicate, repeatDescription);
                this.OrderedAsserter.AssertWasCalled(callPredicate, callDescription, repeatPredicate, repeatDescription);
            }
        }

    }
}
