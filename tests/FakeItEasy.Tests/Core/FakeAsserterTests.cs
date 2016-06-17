namespace FakeItEasy.Tests.Core
{
    using System;
    using System.Collections.Generic;
    using FakeItEasy.Core;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xunit;

    public class FakeAsserterTests
    {
        private readonly List<IFakeObjectCall> calls;
        private readonly CallWriter callWriter;

        public FakeAsserterTests()
        {
            this.calls = new List<IFakeObjectCall>();
            this.callWriter = A.Fake<CallWriter>();
        }

        [Fact]
        public void AssertWasCalled_should_pass_when_the_repeatPredicate_returns_true_for_the_number_of_matching_calls()
        {
            this.StubCalls(2);

            var asserter = this.CreateAsserter();

            asserter.AssertWasCalled(x => true, string.Empty, x => x == 2, string.Empty);
        }

        [Fact]
        public void AssertWasCalled_should_fail_when_the_repeatPredicate_returns_false_fro_the_number_of_matching_calls()
        {
            this.StubCalls(2);

            var asserter = this.CreateAsserter();

            var exception = Record.Exception(() => asserter.AssertWasCalled(x => true, string.Empty, x => false, string.Empty));
            exception.Should().BeAnExceptionOfType<ExpectationException>();
        }

        [Fact]
        public void AssertWasCalled_should_pass_the_number_of_matching_calls_to_the_repeatPredicate()
        {
            int? numberPassedToRepeatPredicate = null;

            this.StubCalls(4);

            var asserter = this.CreateAsserter();

            asserter.AssertWasCalled(x => this.calls.IndexOf(x) == 0, string.Empty, x => { numberPassedToRepeatPredicate = x; return true; }, string.Empty);

            numberPassedToRepeatPredicate.Should().Be(1);
        }

        [Fact]
        public void Exception_message_should_start_with_call_specification()
        {
            var asserter = this.CreateAsserter();
            var exception =
                Record.Exception(() => asserter.AssertWasCalled(x => true, @"IFoo.Bar(1)", x => false, string.Empty));

            var expectedMessage =
@"

  Assertion failed for the following call:
    IFoo.Bar(1)";

            exception.Should().BeAnExceptionOfType<ExpectationException>()
                .And.Message.Should().StartWith(expectedMessage);
        }

        [Fact]
        public void Exception_message_should_write_repeat_expectation()
        {
            this.StubCalls(2);

            var asserter = this.CreateAsserter();
            var exception = Record.Exception(() =>
                asserter.AssertWasCalled(x => false, string.Empty, x => x == 2, "#2 times"));

            var expectedMessage =
@"
  Expected to find it #2 times but found it #0 times among the calls:";

            exception.Should().BeAnExceptionOfType<ExpectationException>()
                .And.Message.Should().Contain(expectedMessage);
        }

        [Fact]
        public void Exception_message_should_call_the_call_writer_to_append_calls_list()
        {
            this.StubCalls(2);

            var asserter = this.CreateAsserter();
            var exception = Record.Exception(() => asserter.AssertWasCalled(x => false, string.Empty, x => false, string.Empty));

            exception.Should().BeAnExceptionOfType<ExpectationException>();
            A.CallTo(() => this.callWriter.WriteCalls(A<IEnumerable<IFakeObjectCall>>.That.IsThisSequence(this.calls), A<IOutputWriter>._)).MustHaveHappened();
        }

        [Fact]
        public void Exception_message_should_write_that_no_calls_were_made_when_calls_is_empty()
        {
            this.calls.Clear();

            var asserter = this.CreateAsserter();

            var exception = Record.Exception(() =>
                asserter.AssertWasCalled(x => false, string.Empty, x => x == 2, "#2 times"));

            var expectedMessage =
@"
  Expected to find it #2 times but no calls were made to the fake object.";

            exception.Should().BeAnExceptionOfType<ExpectationException>()
                .And.Message.Should().Contain(expectedMessage);
        }

        [Fact]
        public void Exception_message_should_end_with_blank_line()
        {
            var asserter = this.CreateAsserter();

            var exception = Record.Exception(() =>
                asserter.AssertWasCalled(x => false, string.Empty, x => false, string.Empty));

            exception.Should().BeAnExceptionOfType<ExpectationException>()
                .And.Message.Should().EndWith(string.Concat(Environment.NewLine, Environment.NewLine));
        }

        [Fact]
        public void Exception_message_should_start_with_blank_line()
        {
            var asserter = this.CreateAsserter();

            var exception = Record.Exception(() =>
                asserter.AssertWasCalled(x => false, string.Empty, x => false, string.Empty));

            exception.Should().BeAnExceptionOfType<ExpectationException>()
                .And.Message.Should().StartWith(Environment.NewLine);
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
    }
}
