namespace FakeItEasy.Tests.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using FakeItEasy.Core;
    using NUnit.Framework;

    [TestFixture]
    public class FakeAsserterTests
    {
        private List<IFakeObjectCall> calls;
        private CallWriter callWriter;

        [SetUp]
        public void SetUp()
        {
            this.calls = new List<IFakeObjectCall>();
            this.callWriter = A.Fake<CallWriter>();
        }

        private FakeAsserter CreateAsserter()
        {
            return new FakeAsserter(this.calls, this.callWriter);
        }

        private void StubCalls(int numberOfCalls)
        {
            for (int i = 0; i < numberOfCalls; i++)
            {
                this.calls.Add(A.Fake<IFakeObjectCall>());
            }
        }

        private string GetExceptionMessage(Action failingAssertion)
        {
            try
            {
                failingAssertion();
            }
            catch (ExpectationException ex)
            {
                return ex.Message;
            }

            throw new AssertionException("No ExpectationException was thrown.");
        }

        [Test]
        public void AssertWasCalled_should_pass_when_the_repeatPredicate_returns_true_for_the_number_of_matching_calls()
        {
            this.StubCalls(2);

            var asserter = this.CreateAsserter();

            asserter.AssertWasCalled(x => true, "", x => x == 2, "");
        }

        [Test]
        public void AssertWasCalled_should_fail_when_the_repeatPredicate_returns_false_fro_the_number_of_matching_calls()
        {
            this.StubCalls(2);

            var asserter = this.CreateAsserter();

            Assert.Throws<ExpectationException>(() => asserter.AssertWasCalled(x => true, "", x => false, ""));
        }

        [Test]
        public void AssertWasCalled_should_pass_the_number_of_matching_calls_to_the_repeatPredicate()
        {
            int? numberPassedToRepeatPredicate = null;

            this.StubCalls(4);

            var asserter = this.CreateAsserter();

            asserter.AssertWasCalled(x => this.calls.IndexOf(x) == 0, "",  x => { numberPassedToRepeatPredicate = x; return true; }, "");

            Assert.That(numberPassedToRepeatPredicate, Is.EqualTo(1));
        }

        [Test]
        public void Exception_message_should_start_with_call_specification()
        {
            var asserter = this.CreateAsserter();

            var message = this.GetExceptionMessage(() =>
                asserter.AssertWasCalled(x => true, @"IFoo.Bar(1)", x => false, ""));

            Assert.That(message, Is.StringStarting(@"

  Assertion failed for the following call:
    'IFoo.Bar(1)'"));
        }

        [Test]
        public void Exception_message_should_write_repeat_expectation()
        {
            this.StubCalls(2);

            var asserter = this.CreateAsserter();

            var message = this.GetExceptionMessage(() =>
                asserter.AssertWasCalled(x => false, "", x => x == 2, "#2 times"));

            Assert.That(message, Is.StringContaining(@"
  Expected to find it #2 times but found it #0 times among the calls:"));
        }

        [Test]
        public void Exception_message_should_call_the_call_writer_to_append_calls_list()
        {
            this.StubCalls(2);

            var asserter = this.CreateAsserter();

            A.CallTo(() => this.callWriter.WriteCalls(4, A<IEnumerable<IFakeObjectCall>>.That.IsThisSequence(this.calls.Cast<IFakeObjectCall>()).Argument, A<TextWriter>.Ignored))
                .Invokes(x => x.Arguments.Get<TextWriter>("writer").Write("foo"));

            var message = this.GetExceptionMessage(() =>
                asserter.AssertWasCalled(x => false, "", x => false, ""));

            Assert.That(message, Is.StringContaining(@"
foo"));
        }

        [Test]
        public void Exception_message_should_write_that_no_calls_were_made_when_calls_is_empty()
        {
            this.calls.Clear();

            var asserter = this.CreateAsserter();

            var message = this.GetExceptionMessage(() =>
                asserter.AssertWasCalled(x => false, "", x => x == 2, "#2 times"));

            Assert.That(message, Is.StringContaining(@"
  Expected to find it #2 times but no calls were made to the fake object."));
        }

        [Test]
        public void Exception_message_should_end_with_blank_line()
        {
            var asserter = this.CreateAsserter();

            var message = this.GetExceptionMessage(() =>
                asserter.AssertWasCalled(x => false, "", x => false, ""));

            Assert.That(message, Is.StringEnding(string.Concat(Environment.NewLine, Environment.NewLine)));
        }

        [Test]
        public void Exception_message_should_start_with_blank_line()
        {
            var asserter = this.CreateAsserter();

            var message = this.GetExceptionMessage(() =>
                asserter.AssertWasCalled(x => false, "", x => false, ""));

            Assert.That(message, Is.StringStarting(Environment.NewLine));
        }
    }
}