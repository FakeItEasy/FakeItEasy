namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;
    using FakeAsserterFactory = System.Func<System.Collections.Generic.IEnumerable<IFakeObjectCall>, FakeAsserter>;

    internal class OrderedFakeAsserterFactory
    {
        private readonly FakeAsserterFactory asserterFactory;
        private readonly OrderedFakeAsserter orderedAsserter;

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
            public FakeAsserter Asserter { get; set; }

            public OrderedFakeAsserter OrderedAsserter { get; set; }

            public void AssertWasCalled(Func<IFakeObjectCall, bool> callPredicate, string callDescription, Func<int, bool> repeatPredicate, string repeatDescription)
            {
                this.Asserter.AssertWasCalled(callPredicate, callDescription, repeatPredicate, repeatDescription);
                this.OrderedAsserter.AssertWasCalled(callPredicate, callDescription, repeatPredicate, repeatDescription);
            }
        }
    }
}