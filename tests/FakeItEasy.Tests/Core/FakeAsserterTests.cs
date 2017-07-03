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
        private readonly List<ICompletedFakeObjectCall> calls;
        private readonly CallWriter callWriter;

        public FakeAsserterTests()
        {
            this.calls = new List<ICompletedFakeObjectCall>();
            this.callWriter = A.Fake<CallWriter>();
        }

        [Fact]
        public void AssertWasCalled_should_pass_when_the_repeatPredicate_returns_true_for_the_number_of_matching_calls()
        {
            this.StubCalls(2);

            var asserter = this.CreateAsserter();

            asserter.AssertWasCalled(x => true, outputWriter => { }, Repeated.Exactly.Twice);
        }

        [Fact]
        public void AssertWasCalled_should_fail_when_the_repeatPredicate_returns_false_for_the_number_of_matching_calls()
        {
            this.StubCalls(2);

            var asserter = this.CreateAsserter();

            var exception = Record.Exception(() => asserter.AssertWasCalled(x => true, outputWriter => { }, Repeated.Exactly.Times(5)));
            exception.Should().BeAnExceptionOfType<ExpectationException>();
        }

        [Fact]
        public void AssertWasCalled_should_pass_the_number_of_matching_calls_to_the_repeatPredicate()
        {
            this.StubCalls(4);

            var repeatedConstraint = A.Fake<Repeated>();
            A.CallTo(() => repeatedConstraint.Matches(A<int>._)).Returns(true);

            var asserter = this.CreateAsserter();

            asserter.AssertWasCalled(x => this.calls.IndexOf(x) == 0, outputWriter => { }, repeatedConstraint);

            A.CallTo(() => repeatedConstraint.Matches(1)).MustHaveHappened();
        }

        [Fact]
        public void Exception_message_should_start_with_call_specification()
        {
            var asserter = this.CreateAsserter();
            var exception =
                Record.Exception(() => asserter.AssertWasCalled(x => true, outputWriter => outputWriter.Write("IFoo.Bar(1)"), A.Dummy<Repeated>()));

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
                asserter.AssertWasCalled(x => false, outputWriter => { }, Repeated.Exactly.Twice));

            var expectedMessage =
@"
  Expected to find it exactly twice but found it #0 times among the calls:";

            exception.Should().BeAnExceptionOfType<ExpectationException>()
                .And.Message.Should().Contain(expectedMessage);
        }

        [Fact]
        public void Exception_message_should_call_the_call_writer_to_append_calls_list()
        {
            this.StubCalls(2);

            var asserter = this.CreateAsserter();
            var exception = Record.Exception(() => asserter.AssertWasCalled(x => false, outputWriter => { }, A.Dummy<Repeated>()));

            exception.Should().BeAnExceptionOfType<ExpectationException>();
            A.CallTo(() => this.callWriter.WriteCalls(A<IEnumerable<IFakeObjectCall>>.That.IsThisSequence(this.calls), A<IOutputWriter>._)).MustHaveHappened();
        }

        [Fact]
        public void Exception_message_should_write_that_no_calls_were_made_when_calls_is_empty()
        {
            this.calls.Clear();

            var asserter = this.CreateAsserter();

            var exception = Record.Exception(() =>
                asserter.AssertWasCalled(x => false, outputWriter => { }, Repeated.Exactly.Twice));

            var expectedMessage =
@"
  Expected to find it exactly twice but no calls were made to the fake object.";

            exception.Should().BeAnExceptionOfType<ExpectationException>()
                .And.Message.Should().Contain(expectedMessage);
        }

        [Fact]
        public void Exception_message_should_end_with_blank_line()
        {
            var asserter = this.CreateAsserter();

            var exception = Record.Exception(() =>
                asserter.AssertWasCalled(x => false, outputWriter => { }, A.Dummy<Repeated>()));

            exception.Should().BeAnExceptionOfType<ExpectationException>()
                .And.Message.Should().EndWith(string.Concat(Environment.NewLine, Environment.NewLine));
        }

        [Fact]
        public void Exception_message_should_start_with_blank_line()
        {
            var asserter = this.CreateAsserter();

            var exception = Record.Exception(() =>
                asserter.AssertWasCalled(x => false, outputWriter => { }, A.Dummy<Repeated>()));

            exception.Should().BeAnExceptionOfType<ExpectationException>()
                .And.Message.Should().StartWith(Environment.NewLine);
        }

        [Fact]
        public void Exception_message_should_not_contain_matching_call_when_call_is_recorded_after_checking_count()
        {
            var asserter = this.CreateAsserter();

            var repeatConstraint = A.Fake<Repeated>();

            // This will cause the call to be recorded after checking the count
            A.CallTo(() => repeatConstraint.Matches(A<int>._))
                .Invokes(() => this.StubCalls(1))
                .Returns(false);

            var exception = Record.Exception(() =>
                asserter.AssertWasCalled(x => true, outputWriter => { }, repeatConstraint));

            exception.Should().BeAnExceptionOfType<ExpectationException>()
                .And.Message.Should().Contain("no calls were made to the fake object.");
        }

        private FakeAsserter CreateAsserter()
        {
            return new FakeAsserter(this.calls, this.callWriter);
        }

        private void StubCalls(int numberOfCalls)
        {
            for (int i = 0; i < numberOfCalls; i++)
            {
                var call = A.Fake<ICompletedFakeObjectCall>();
                SequenceNumberManager.RecordSequenceNumber(call);
                this.calls.Add(call);
            }
        }
    }
}
