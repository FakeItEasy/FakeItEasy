namespace FakeItEasy.Tests.Core
{
    using System.Collections.Generic;
    using FakeItEasy.Core;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xunit;

    public class FakeAsserterTests
    {
        private readonly List<ICompletedFakeObjectCall> calls;
        private readonly CallWriter callWriter;
        private readonly StringBuilderOutputWriter.Factory outputWriterFactory;

        public FakeAsserterTests()
        {
            this.calls = new List<ICompletedFakeObjectCall>();
            this.callWriter = A.Fake<CallWriter>();
            this.outputWriterFactory = A.Fake<StringBuilderOutputWriter.Factory>();
        }

        [Fact]
        public void Exception_message_should_not_contain_matching_call_when_call_is_recorded_after_checking_count()
        {
            var asserter = this.CreateAsserter();

            // This will cause the call to be recorded after checking the count
            var callCountConstraintThatSimulatesALateArrivingCall = new CallCountConstraint(n => { this.StubCalls(1); return false; }, "no match");

            var exception = Record.Exception(() =>
                asserter.AssertWasCalled(x => true, outputWriter => { }, callCountConstraintThatSimulatesALateArrivingCall));

            exception.Should().BeAnExceptionOfType<ExpectationException>()
                .And.Message.Should().Contain("no calls were made to the fake object.");
        }

        private FakeAsserter CreateAsserter()
        {
            return new FakeAsserter(this.calls, this.callWriter, this.outputWriterFactory);
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
