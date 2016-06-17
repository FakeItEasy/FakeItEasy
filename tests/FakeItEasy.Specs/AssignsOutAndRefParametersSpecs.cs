namespace FakeItEasy.Specs
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using FakeItEasy.Configuration;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xbehave;
    using Xunit;

    [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "WithOut", Justification = "That's two words, not one")]
    [SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Required for testing.")]
    [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Required for testing.")]
    public delegate void VoidDelegateWithOutAndRefParameters(
        int byValueParameter, ref int byRefParameter, out int outParameter);

    [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "WithOut", Justification = "That's two words, not one")]
    [SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Required for testing.")]
    [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Required for testing.")]
    public delegate int NonVoidDelegateWithOutAndRefParameters(
        int byValueParameter, ref int byRefParameter, out int outParameter);

    public interface IHaveARef
    {
        [SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "0#", Justification = "Required for testing.")]
        void MightReturnAKnownValue(ref string andThisIsWhoReallyDidIt);
    }

    public static class AssignsOutAndRefParametersSpecs
    {
        private const string Condition = "someone_else";
        private const string KnownOutput = "you";

        [Scenario]
        public static void AssignOutAndRefParametersLazilyUsingFunc(IHaveARef subject, string outValue)
        {
            "Given a fake with a method that has a ref parameter"
                .x(() => subject = A.Fake<IHaveARef>());

            "When the fake is configured to assign out and ref parameters lazily using func"
                .x(() => A.CallTo(() => subject.MightReturnAKnownValue(ref outValue))
                    .WithAnyArguments()
                    .AssignsOutAndRefParametersLazily((string value) => new object[]
                    {
                        value == Condition ? KnownOutput : "me"
                    }));

            "And the configured method is called with matching arguments"
                .x(() =>
                {
                    outValue = Condition;
                    subject.MightReturnAKnownValue(ref outValue);
                });

            "Then it assigns the conditional value to the ref field"
                .x(() => outValue.Should().BeEquivalentTo(KnownOutput));
        }

        [Scenario]
        public static void AssignOutAndRefParametersLazilyUsingCall(IHaveARef subject, string outValue)
        {
            "Given a fake with a method that has a ref parameter"
                .x(() => subject = A.Fake<IHaveARef>());

            "When the fake is configured to assign out and ref parameters lazily using call"
                .x(() => A.CallTo(() => subject.MightReturnAKnownValue(ref outValue))
                    .WithAnyArguments()
                    .AssignsOutAndRefParametersLazily(call => new object[]
                    {
                        call.Arguments.Get<string>(0) == Condition ? KnownOutput : "me"
                    }));

            "And the configured method is called with matching arguments"
                .x(() =>
                {
                    outValue = Condition;
                    subject.MightReturnAKnownValue(ref outValue);
                });

            "Then it assigns the conditional value to the ref field"
                .x(() => outValue.Should().BeEquivalentTo(KnownOutput));
        }

        [Scenario]
        public static void AssignOutAndRefParameters(IHaveARef subject, string outValue)
        {
            "Given a fake with a method that has a ref parameter"
                .x(() => subject = A.Fake<IHaveARef>());

            "When the fake is configured to assign out and ref parameters unconditionally"
                .x(() => A.CallTo(() => subject.MightReturnAKnownValue(ref outValue))
                    .WithAnyArguments()
                    .AssignsOutAndRefParameters(KnownOutput));

            "And the configured method is called"
                .x(() => subject.MightReturnAKnownValue(ref outValue));

            "Then it assigns the configured value to the ref field"
                .x(() => outValue.Should().BeEquivalentTo(KnownOutput));
        }

        [Scenario]
        public static void MultipleAssignOutAndRefParameters(
            IHaveARef subject, string outValue, IVoidConfiguration callSpec, Exception exception)
        {
            "Given a fake with a method that has a ref parameter"
                .x(() => subject = A.Fake<IHaveARef>());

            "And a call specification on that fake"
                .x(() => callSpec = A.CallTo(() => subject.MightReturnAKnownValue(ref outValue)).WithAnyArguments());

            "And the call specification is configured to assign out and ref parameters"
                .x(() => callSpec.AssignsOutAndRefParameters("test1"));

            "When the fake is configured to assign out and ref parameters again"
                .x(() => exception = Record.Exception(() => callSpec.AssignsOutAndRefParameters("test2")));

            "Then it throws an invalid operation exception"
                .x(() => exception.Should().BeAnExceptionOfType<InvalidOperationException>());
        }

        [Scenario]
        public static void AssignOutAndRefParameterForVoidDelegate(
            VoidDelegateWithOutAndRefParameters subject, int refValue, int outValue)
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
            NonVoidDelegateWithOutAndRefParameters subject, int refValue, int outValue, int result)
        {
            "Given a faked delegate with a non-void return type and ref and out parameters"
                .x(() => subject = A.Fake<NonVoidDelegateWithOutAndRefParameters>());

            "When the faked delegate is configured to assign the out and ref parameters"
                .x(() => A.CallTo(() => subject(1, ref refValue, out outValue))
                    .Returns(123).AssignsOutAndRefParameters(42, 99));

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
