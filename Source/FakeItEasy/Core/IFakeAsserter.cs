namespace FakeItEasy.Core
{
    using System;

    internal interface IFakeAsserter
    {
        void AssertWasCalled(Func<IFakeObjectCall, bool> callPredicate, string callDescription, Func<int, bool> repeatPredicate, string repeatDescription);
    }
}