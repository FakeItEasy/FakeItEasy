namespace FakeItEasy.Specs
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using FakeItEasy.Configuration;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xbehave;
    using Xunit;

    public static class AssignsOutAndRefParametersSpecs
    {
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "WithOut", Justification = "That's two words, not one")]
        public delegate void VoidDelegateWithOutAndRefParameters(
            int byValueParameter, ref int byRefParameter, out int outParameter);

        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "WithOut", Justification = "That's two words, not one")]
        public delegate Foo NonVoidDelegateWithOutAndRefParameters(
            int byValueParameter, ref int byRefParameter, out int outParameter);

        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "WithOut", Justification = "That's two words, not one")]
        public delegate void VoidDelegateWithRefParameter(ref string byRefParameter);

        public interface IHaveARef
        {
            void MightReturnAKnownValue(ref string andThisIsWhoReallyDidIt);
        }

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
        public static void SpecifyReturnsAndAssignOutAndRefParameterForNonVoidDelegate(
            NonVoidDelegateWithOutAndRefParameters subject, int refValue, int outValue, Foo expectedResult, Foo result)
        {
            "Given a faked delegate with a non-void return type and ref and out parameters"
                .x(() => subject = A.Fake<NonVoidDelegateWithOutAndRefParameters>());

            "When the faked delegate is configured to return a value and assign the out and ref parameters"
                .x(() =>
                {
                    expectedResult = new Foo();
                    A.CallTo(() => subject(1, ref refValue, out outValue)).Returns(expectedResult).AssignsOutAndRefParameters(42, 99);
                });

            "And I call the faked delegate"
                .x(() => result = subject(1, ref refValue, out outValue));

            "Then it returns the specified value"
                .x(() => result.Should().BeSameAs(expectedResult));

            "And the ref parameter is set to the specified value"
                .x(() => refValue.Should().Be(42));

            "And the out parameter is set to the specified value"
                .x(() => outValue.Should().Be(99));
        }

        [Scenario]
        public static void AssignOutAndRefParameterForNonVoidDelegate(
            NonVoidDelegateWithOutAndRefParameters subject, int refValue, int outValue, Foo result)
        {
            "Given a faked delegate with a non-void return type and ref and out parameters"
                .x(() => subject = A.Fake<NonVoidDelegateWithOutAndRefParameters>());

            "When the faked delegate is configured to assign the out and ref parameters without setting the return value"
                .x(() => A.CallTo(() => subject(1, ref refValue, out outValue))
                    .AssignsOutAndRefParameters(43, 100));

            "And I call the faked delegate"
                .x(() => result = subject(1, ref refValue, out outValue));

            "Then it returns a Dummy value"
                .x(() => result.Should().BeSameAs(FooFactory.Instance));

            "And the ref parameter is set to the specified value"
                .x(() => refValue.Should().Be(43));

            "And the out parameter is set to the specified value"
                .x(() => outValue.Should().Be(100));
        }

        [Scenario]
        public static void AssignOutAndRefParameterRecordedCall(
            IHaveARef subject, string refValue)
        {
            "Given a fake with a method that has a ref parameter"
                .x(() => subject = A.Fake<IHaveARef>());

            "When the fake is configured to assign out and ref parameters when argument matches condition"
                .x(() =>
                {
                    refValue = Condition;
                    A.CallTo(() => subject.MightReturnAKnownValue(ref refValue))
                        .AssignsOutAndRefParameters(KnownOutput);
                });

            "And the configured method is called"
                .x(() => subject.MightReturnAKnownValue(ref refValue));

            "Then the assertion that a call with the expected value has happened succeeds"
                .x(() =>
                {
                    string expectedValue = Condition;
                    A.CallTo(() => subject.MightReturnAKnownValue(ref expectedValue)).MustHaveHappened();
                });

            "And the call records the updated arguments"
                .x(() =>
                {
                    Fake.GetCalls(subject).First().ArgumentsAfterCall[0].Should().Be(KnownOutput);
                });
        }

        [Scenario]
        public static void AssignOutAndRefParameterRecordedCallForVoidDelegate(
            VoidDelegateWithRefParameter subject, string refValue)
        {
            "Given a fake with a method that has a ref parameter"
                .x(() => subject = A.Fake<VoidDelegateWithRefParameter>());

            "When the fake is configured to assign out and ref parameters when argument matches condition"
                .x(() =>
                {
                    refValue = Condition;
                    A.CallTo(() => subject.Invoke(ref refValue))
                        .AssignsOutAndRefParameters(KnownOutput);
                });

            "And the configured method is called"
                .x(() => subject.Invoke(ref refValue));

            "Then the assertion that a call with the expected value has happened succeeds"
                .x(() =>
                {
                    string expectedValue = Condition;
                    A.CallTo(() => subject.Invoke(ref expectedValue)).MustHaveHappened();
                });

            "And the call records the updated arguments"
                .x(() =>
                {
                    Fake.GetCalls(subject).First().ArgumentsAfterCall[0].Should().Be(KnownOutput);
                });
        }

        public class Foo
        {
        }

        public class FooFactory : DummyFactory<Foo>
        {
            public static Foo Instance { get; } = new Foo();

            protected override Foo Create() => Instance;
        }
    }
}
