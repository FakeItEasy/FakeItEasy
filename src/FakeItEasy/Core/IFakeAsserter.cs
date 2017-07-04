namespace FakeItEasy.Core
{
    using System;

    internal interface IFakeAsserter
    {
        void AssertWasCalled(Func<ICompletedFakeObjectCall, bool> callPredicate, Action<IOutputWriter> callDescriber, Repeated repeatConstraint);
    }
}
