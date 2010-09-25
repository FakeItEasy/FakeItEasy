namespace FakeItEasy.Tests.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FakeItEasy.Core;
    using NUnit.Framework;

    [TestFixture]
    public class OrderedFakeAsserterFactoryTests
    {
        [Test]
        public void Should_call_asserter_from_asserter_factory()
        {
            // Arrange
            var innerAsserter = A.Fake<FakeAsserter>();
            Func<IEnumerable<IFakeObjectCall>, FakeAsserter> innerFactory = x => 
            {
                return innerAsserter;
            };

            var factory = new OrderedFakeAsserterFactory(innerFactory, A.Fake<OrderedFakeAsserter>());

            Func<IFakeObjectCall, bool> callPredicate = x => true;
            Func<int, bool> repeatPredicate = x => true;
            
            // Act
            var asserter = factory.CreateAsserter(Enumerable.Empty<IFakeObjectCall>());
            asserter.AssertWasCalled(callPredicate, "call description", repeatPredicate, "repeat description");

            // Assert
            A.CallTo(() => innerAsserter.AssertWasCalled(callPredicate, "call description", repeatPredicate, "repeat description")).MustHaveHappened();
        }

        [Test]
        public void Should_call_ordered_asserter()
        {
            // Arrange
            var innerAsserter = A.Fake<FakeAsserter>();
            var orderedAsserter = A.Fake<OrderedFakeAsserter>();

            var factory = new OrderedFakeAsserterFactory(x => innerAsserter, orderedAsserter);

            Func<IFakeObjectCall, bool> callPredicate = x => true;
            Func<int, bool> repeatPredicate = x => true;

            // Act
            var asserter = factory.CreateAsserter(Enumerable.Empty<IFakeObjectCall>());
            asserter.AssertWasCalled(callPredicate, "call description", repeatPredicate, "repeat description");

            // Assert
            A.CallTo(() => orderedAsserter.AssertWasCalled(callPredicate, "call description", repeatPredicate, "repeat description")).MustHaveHappened();
        }
    }
}
