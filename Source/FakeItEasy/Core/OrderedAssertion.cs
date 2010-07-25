namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class OrderedAssertion
    {
        public static IDisposable OrderedAssertions(this IEnumerable<ICompletedFakeObjectCall> calls)
        {
            var result = new AsserterResetter() { ResetTo = CurrentAsserterFactory };
            
            var asserter = new OrderedFakeAsserter(calls.Cast<IFakeObjectCall>(), ServiceLocator.Current.Resolve<CallWriter>());
            CurrentAsserterFactory = x => asserter;
            
            return result;
        }

        private class AsserterResetter
            : IDisposable
        {
            public FakeAsserter.Factory ResetTo;

            public void Dispose()
            {
                OrderedAssertion.CurrentAsserterFactory = this.ResetTo;
            }
        }


        internal static FakeAsserter.Factory CurrentAsserterFactory = x => new FakeAsserter(x, ServiceLocator.Current.Resolve<CallWriter>());
    }
}
