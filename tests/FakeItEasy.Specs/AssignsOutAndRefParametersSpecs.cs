namespace FakeItEasy.Specs
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xbehave;

    public delegate void VoidDelegateWithOutAndRefParameters(int x, ref int y, out int z);

    public delegate int NonVoidDelegateWithOutAndRefParameters(int x, ref int y, out int z);

    public interface IHaveAnOut
    {
        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "0#", Justification = "Required for testing.")]
        void MightReturnAKnownValue(out string andThisIsWhoReallyDidIt);
    }

    public static class AssignsOutAndRefParametersSpecs
    {
        [Scenario]
        public static void AssignOutAndRefParametersLazilyUsingFunc(
            IHaveAnOut subject,
            string outValue)
        {
            var condition = "someone_else";
            var knownOutput = "you";

            "establish"
                .x(() => subject = A.Fake<IHaveAnOut>());

            "when configuring a fake to assign out and ref parameters lazily using func"
                .x(() => A.CallTo(() => subject.MightReturnAKnownValue(out outValue))
                    .WithAnyArguments()
                    .AssignsOutAndRefParametersLazily((string value) => new object[]
                    {
                        value == condition ? knownOutput : "me"
                    }));

            "it should assign the conditional value to the out field"
                .x(() =>
                    {
                        string value = condition;
                        subject.MightReturnAKnownValue(out value);
                        value.Should().BeEquivalentTo(knownOutput);
                    });
        }

        [Scenario]
        public static void AssignOutAndRefParametersLazilyUsingCall(
            IHaveAnOut subject,
            string outValue)
        {
            var condition = "someone_else";
            var knownOutput = "you";

            "establish"
                .x(() => subject = A.Fake<IHaveAnOut>());

            "when configuring a fake to assign out and ref parameters lazily using call"
                .x(() => A.CallTo(() => subject.MightReturnAKnownValue(out outValue))
                    .WithAnyArguments()
                    .AssignsOutAndRefParametersLazily((call) => new object[]
                    {
                        call.Arguments.Get<string>(0) == condition ? knownOutput : "me"
                    }));

            "it should assign the conditional value to the out field"
                .x(() =>
                    {
                        string value = condition;
                        subject.MightReturnAKnownValue(out value);
                        value.Should().BeEquivalentTo(knownOutput);
                    });
        }

        [Scenario]
        public static void AssignOutAndRefParameters(
            IHaveAnOut subject,
            string outValue)
        {
            var condition = "someone_else";
            var knownOutput = "you";

            "establish"
                .x(() => subject = A.Fake<IHaveAnOut>());

            "when configuring a fake to assign out and ref parameters"
                .x(() => A.CallTo(() => subject.MightReturnAKnownValue(out outValue))
                             .WithAnyArguments()
                             .AssignsOutAndRefParameters(knownOutput));

            "it should assign the conditional value to the out field"
                .x(() =>
                    {
                        string value = condition;
                        subject.MightReturnAKnownValue(out value);
                        value.Should().BeEquivalentTo(knownOutput);
                    });
        }

        [Scenario]
        public static void MultipleAssignOutAndRefParameters(
            IHaveAnOut subject,
            string outValue,
            Exception exception)
        {
            "establish"
                .x(() => subject = A.Fake<IHaveAnOut>());

            "when configuring a fake to assign out and ref parameters multiple times"
                .x(() =>
                {
                    var callSpec =
                        A.CallTo(() => subject.MightReturnAKnownValue(out outValue))
                            .WithAnyArguments();

                    callSpec.AssignsOutAndRefParameters(new object[] { "test1" });

                    exception = Record.Exception(() => callSpec.AssignsOutAndRefParameters(new object[] { "test2" }));
                });

            "it should throw an invalid operation exception"
                .x(() => exception.Should().BeAnExceptionOfType<InvalidOperationException>());
        }

        [Scenario]
        public static void AssignOutAndRefParameterForVoidDelegate(
            VoidDelegateWithOutAndRefParameters subject,
            int refValue,
            int outValue)
        {
            "Given a faked delegate with void return type and ref and out parameters"
                .x(() => subject = A.Fake<VoidDelegateWithOutAndRefParameters>());

            "When the faked delegate is configured to assign the out and ref parameters"
                .x(() => A.CallTo(() => subject(1, ref refValue, out outValue)).AssignsOutAndRefParameters(42, 99));

            "And I call the faked delegate"
                .x(() => subject(1, ref refValue, out outValue));

            "Then the ref parameter is set to the specified value"
                .x(() => refValue.Should().Be(42));

            "And the out parameter is set to the specified value"
                .x(() => outValue.Should().Be(99));
        }

        [Scenario]
        public static void AssignOutAndRefParameterForNonVoidDelegate(
            NonVoidDelegateWithOutAndRefParameters subject,
            int refValue,
            int outValue,
            int result)
        {
            "Given a faked delegate with a non-void return type and ref and out parameters"
                .x(() => subject = A.Fake<NonVoidDelegateWithOutAndRefParameters>());

            "When the faked delegate is configured to assign the out and ref parameters"
                .x(() => A.CallTo(() => subject(1, ref refValue, out outValue)).Returns(123).AssignsOutAndRefParameters(42, 99));

            "And I call the faked delegate"
                .x(() => result = subject(1, ref refValue, out outValue));

            "Then it returns the specified value"
                .x(() => result.Should().Be(123));

            "And the ref parameter is set to the specified value"
                .x(() => refValue.Should().Be(42));

            "And the out parameter is set to the specified value"
                .x(() => outValue.Should().Be(99));
        }
    }
}
