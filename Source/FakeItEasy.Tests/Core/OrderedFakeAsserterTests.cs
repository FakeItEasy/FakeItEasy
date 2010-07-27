namespace FakeItEasy.Tests.Core
{
    using System;
    using FakeItEasy.Core;
    using NUnit.Framework;
    using System.Linq;
    using System.Collections.Generic;

    [TestFixture]
    public class OrderedFakeAsserterTests
    {
        private IFakeAsserter innerAsserter;

        [SetUp]
        public void SetUp()
        {
            this.innerAsserter = A.Fake<IFakeAsserter>();
        }

        private OrderedFakeAsserter CreateAsserter(IEnumerable<IFakeObjectCall> calls)
        {
            return new OrderedFakeAsserter(calls, this.innerAsserter);
        }

        [Test]
        public void Should_call_fake_asserter()
        {
            // Arrange
            var orderedAsserter = this.CreateAsserter(Enumerable.Empty<IFakeObjectCall>());

            Func<IFakeObjectCall, bool> callPredicate = x => true;
            Func<int, bool> repeatPredicate = x => true;

            // Act
            orderedAsserter.AssertWasCalled(callPredicate, "call description", repeatPredicate, "repeat description");
            
            // Assert
            A.CallTo(() => 
                this.innerAsserter.AssertWasCalled(callPredicate, "call description", repeatPredicate, "repeat description"))
                .MustHaveHappened();
        }

        [Test]
        public void Should_fail_when_calls_did_not_happen_in_order()
        {
            // Arrange
            var calls = A.CollectionOfFake<IFakeObjectCall>(2);

            var orderedAsserter = this.CreateAsserter(calls);

            Func<IFakeObjectCall, bool> secondCallPredicate = x => object.ReferenceEquals(calls[1], x);
            Func<IFakeObjectCall, bool> firstCallPredicate = x => object.ReferenceEquals(calls[0], x);

            // Act
            orderedAsserter.AssertWasCalled(secondCallPredicate, "foo", x => x == 1, "foo");
            
            // Assert
            Assert.Throws<ExpectationException>(() =>
                orderedAsserter.AssertWasCalled(firstCallPredicate, "foo", x => x == 1, "foo"));
        }

        [Test]
        public void Should_not_fail_when_calls_happened_in_order()
        {
            // Arrange
            var calls = A.CollectionOfFake<IFakeObjectCall>(2);

            var orderedAsserter = this.CreateAsserter(calls);

            Func<IFakeObjectCall, bool> secondCallPredicate = x => object.ReferenceEquals(calls[1], x);
            Func<IFakeObjectCall, bool> firstCallPredicate = x => object.ReferenceEquals(calls[0], x);

            // Act
            orderedAsserter.AssertWasCalled(firstCallPredicate, "foo", x => x == 1, "foo");

            // Assert
            Assert.DoesNotThrow(() =>
                orderedAsserter.AssertWasCalled(secondCallPredicate, "foo", x => x == 1, "foo"));
        }

        [Test]
        public void Should_fail_when_calls_happened_in_order_but_repeat_is_not_correct()
        {
            // Arrange
            var calls = A.CollectionOfFake<IFakeObjectCall>(2);
            calls.Add(calls[0]);

            var orderedAsserter = this.CreateAsserter(calls);

            Func<IFakeObjectCall, bool> secondCallPredicate = x => object.ReferenceEquals(calls[1], x);
            Func<IFakeObjectCall, bool> firstCallPredicate = x => object.ReferenceEquals(calls[0], x);

            // Act
            orderedAsserter.AssertWasCalled(firstCallPredicate, "foo", x => x == 2, "foo");

            // Assert
            Assert.Throws<ExpectationException>(() =>
                orderedAsserter.AssertWasCalled(secondCallPredicate, "foo", x => x == 1, "foo"));
        }
    }
}
