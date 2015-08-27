namespace FakeItEasy.Specs
{
    using System.Diagnostics.CodeAnalysis;
    using FluentAssertions;
    using Xbehave;

    public interface IHaveAnOut
    {
        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "0#", Justification = "Required for testing.")]
        void MightReturnAKnownValue(out string andThisIsWhoReallyDidIt);
    }

    public class AssignsOutAndRefParameters
    {
        [Scenario]
        public void when_configuring_a_fake_to_assign_out_and_ref_parameters_lazily_using_func(
            IHaveAnOut subject,
            string outValue)
        {
            string condition = "someone_else";
            string knownOutput = "you";

            "establish"
                .x(() => subject = A.Fake<IHaveAnOut>());

            "when configuring a fake to assign out and ref parameters lazily using func"
                .x(() => A.CallTo(() => subject.MightReturnAKnownValue(out outValue))
                             .WithAnyArguments()
                             .AssignsOutAndRefParametersLazily((string value) => new object[] { value == condition ? knownOutput : "me" }));

            "it should assign the conditional value to the out field"
                .x(() =>
                    {
                        string value = condition;
                        subject.MightReturnAKnownValue(out value);
                        value.Should().BeEquivalentTo(knownOutput);
                    });
        }    

        [Scenario]
        public void when_configuring_a_fake_to_assign_out_and_ref_parameters_lazily_using_call(
            IHaveAnOut subject,
            string outValue)
        {
            string condition = "someone_else";
            string knownOutput = "you";

            "establish"
                .x(() => subject = A.Fake<IHaveAnOut>());

            "when configuring a fake to assign out and ref parameters lazily using call"
                .x(() => A.CallTo(() => subject.MightReturnAKnownValue(out outValue))
                             .WithAnyArguments()
                             .AssignsOutAndRefParametersLazily((call) => new object[] { call.Arguments.Get<string>(0) == condition ? knownOutput : "me" }));

            "it should assign the conditional value to the out field"
                .x(() =>
                    {
                        string value = condition;
                        subject.MightReturnAKnownValue(out value);
                        value.Should().BeEquivalentTo(knownOutput);
                    });
        }

        [Scenario]
        public void when_configuring_a_fake_to_assign_out_and_ref_parameters(
            IHaveAnOut subject,
            string outValue)
        {
            string condition = "someone_else";
            string knownOutput = "you";

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
    }
}
