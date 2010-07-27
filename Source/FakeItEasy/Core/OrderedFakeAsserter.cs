namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class OrderedFakeAsserter
        : IFakeAsserter
    {
        void IFakeAsserter.AssertWasCalled(Func<IFakeObjectCall, bool> callPredicate, string callDescription, Func<int, bool> repeatPredicate, string repeatDescription)
        {
            throw new NotImplementedException();
        }
    }
}
