namespace FakeItEasy.Tests.Core
{
    using System;
    using System.Collections.Generic;
    using FakeItEasy.Core;
    using NUnit.Framework;

    [TestFixture]
    public class OrderedFakeAsserterTests
    {
        private CallWriter callWriter;
        
        [SetUp]
        public void Setup()
        {
            this.callWriter = A.Fake<CallWriter>();
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

        [Test]
        [SetCulture("en-US")]
        public void Should_throw_exception_with_correct_message()
        {
            // Arrange
            var calls = A.CollectionOfFake<IFakeObjectCall>(2);
            calls.Add(calls[0]);

            var orderedAsserter = this.CreateAsserter(calls);

            Func<IFakeObjectCall, bool> secondCallPredicate = x => object.ReferenceEquals(calls[1], x);
            Func<IFakeObjectCall, bool> firstCallPredicate = x => object.ReferenceEquals(calls[0], x);

            A.CallTo(() => this.callWriter.WriteCalls(A<IEnumerable<IFakeObjectCall>>.That.IsThisSequence(calls), A<IOutputWriter>._))
                .Invokes(x => x.Arguments.Get<IOutputWriter>("writer").Write("list of calls"));

            // Act
            orderedAsserter.AssertWasCalled(firstCallPredicate, "first call description", x => x == 2, "first repeat description");

            // Assert
            var expectedMessage = @"

  Assertion failed for the following calls:
    'first call description' repeated first repeat description
    'second call description' repeated second repeat description
  The calls where found but not in the correct order among the calls:
    list of calls";

            Assert.That(
                () => orderedAsserter.AssertWasCalled(secondCallPredicate, "second call description", x => x == 1, "second repeat description"),
                Throws.Exception.With.Message.EqualTo(expectedMessage));
        }

        private OrderedFakeAsserter CreateAsserter(IEnumerable<IFakeObjectCall> calls)
        {
            return new OrderedFakeAsserter(calls, this.callWriter);
        }
    }
}
