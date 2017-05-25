namespace FakeItEasy.Core
{
    using System;

    internal interface IFakeAsserter
    {
        void AssertWasCalled(Func<IFakeObjectCall, bool> callPredicate, Action<IOutputWriter> callDescriber, Func<int, bool> repeatPredicate, string repeatDescription);
    }
}
