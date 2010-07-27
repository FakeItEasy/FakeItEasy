namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class OrderedFakeAsserter
            : FakeAsserter
    {
        public OrderedFakeAsserter(IEnumerable<IFakeObjectCall> calls, CallWriter callWriter)
            : base(calls, callWriter)
        {
        }

        public override void AssertWasCalled(Func<IFakeObjectCall, bool> callPredicate, string callDescription, Func<int, bool> repeatPredicate, string repeatDescription)
        {
#if DEBUG
            throw new NotImplementedException();
#else
#error "Must be implemented"
#endif
        }

    }
}
