namespace FakeItEasy
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using FakeItEasy.Core;

    /// <summary>
    /// Provides functionality for making ordered assertions on fakes.
    /// </summary>
    public static class OrderedAssertion
    {
        private static FakeAsserter.Factory currentAsserterFactoryField = x => new FakeAsserter(x, ServiceLocator.Current.Resolve<CallWriter>());
        private static bool orderedAssertionsScopeIsOpened;

        internal static FakeAsserter.Factory CurrentAsserterFactory
        {
            get { return currentAsserterFactoryField; }
        }

        /// <summary>
        /// Creates a scope that changes the behavior on asserts so that all asserts within
        /// the scope must be to calls in the specified collection of calls. Calls must have happened
        /// in the order that the asserts are specified or the asserts will fail.
        /// </summary>
        /// <param name="calls">The calls to assert among.</param>
        /// <returns>A disposable used to close the scope.</returns>
        public static IDisposable OrderedAssertions(this IEnumerable<ICompletedFakeObjectCall> calls)
        {
            AssertThatScopeIsNotOpen();

            orderedAssertionsScopeIsOpened = true;

            var orderedFactory = CreateOrderedAsserterFactory(calls);

            return SetOrderedFactoryAsCurrentAndGetResetter(orderedFactory);
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Factory method.")]
        private static IDisposable SetOrderedFactoryAsCurrentAndGetResetter(OrderedFakeAsserterFactory orderedFactory)
        {
            var resetter = new AsserterResetter { ResetTo = currentAsserterFactoryField };

            currentAsserterFactoryField = orderedFactory.CreateAsserter;

            return resetter;
        }

        private static OrderedFakeAsserterFactory CreateOrderedAsserterFactory(IEnumerable<ICompletedFakeObjectCall> calls)
        {
            return new OrderedFakeAsserterFactory(
                x => new FakeAsserter(x, ServiceLocator.Current.Resolve<CallWriter>()),
                new OrderedFakeAsserter(calls.Cast<IFakeObjectCall>(), ServiceLocator.Current.Resolve<CallWriter>()));
        }

        private static void AssertThatScopeIsNotOpen()
        {
            if (orderedAssertionsScopeIsOpened)
            {
                throw new InvalidOperationException(ExceptionMessages.OrderedAssertionsAlreadyOpen);
            }
        }

        private class AsserterResetter
            : IDisposable
        {
            public FakeAsserter.Factory ResetTo { get; set; }

            [SuppressMessage("Microsoft.Usage", "CA1816:CallGCSuppressFinalizeCorrectly", Justification = "There is no finalizer.")]
            public void Dispose()
            {
                currentAsserterFactoryField = this.ResetTo;
                orderedAssertionsScopeIsOpened = false;
            }
        }
    }
}