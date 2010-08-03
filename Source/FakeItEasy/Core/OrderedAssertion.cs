namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class OrderedAssertion
    {
        private static FakeAsserter.Factory currentAsserterFactoryField = x => new FakeAsserter(x, ServiceLocator.Current.Resolve<CallWriter>());
        private static bool orderedAssertionsScopeIsOpened;

        public static IDisposable OrderedAssertions(this IEnumerable<ICompletedFakeObjectCall> calls)
        {
            if (orderedAssertionsScopeIsOpened)
            {
                throw new InvalidOperationException(ExceptionMessages.OrderedAssertionsAlreadyOpen);
            }
            
            orderedAssertionsScopeIsOpened = true;

            var orderedFactory = 
                new OrderedFakeAsserterFactory(
                    x => new FakeAsserter(x, ServiceLocator.Current.Resolve<CallWriter>()),
                    new OrderedFakeAsserter(calls.Cast<IFakeObjectCall>(), ServiceLocator.Current.Resolve<CallWriter>())
                 );

            var resetter = new AsserterResetter { ResetTo = currentAsserterFactoryField };

            currentAsserterFactoryField = orderedFactory.CreateAsserter;

            return resetter;
        }

        private class AsserterResetter
            : IDisposable
        {
            public FakeAsserter.Factory ResetTo;

            public void Dispose()
            {
                OrderedAssertion.currentAsserterFactoryField = ResetTo;
                OrderedAssertion.orderedAssertionsScopeIsOpened = false;
            }
        }

        internal static FakeAsserter.Factory CurrentAsserterFactory
        {
            get
            {
                return currentAsserterFactoryField;
            }
        }
    }
}
