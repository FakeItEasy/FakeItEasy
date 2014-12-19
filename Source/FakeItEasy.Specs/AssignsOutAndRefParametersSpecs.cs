namespace FakeItEasy.Specs
{
    using System.Diagnostics.CodeAnalysis;
    using FluentAssertions;
    using Machine.Specifications;

    public interface IHaveAnOut
    {
        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "0#", Justification = "Required for testing.")]
        void MightReturnAKnownValue(out string andThisIsWhoReallyDidIt);
    }

    public class when_configuring_a_fake_to_assign_out_and_ref_parameters_lazily_using_func
    {
        private static IHaveAnOut subject;
        private static string outValue;
        private static string condition = "someone_else";
        private static string knownOutput = "you";

        private Establish context = () =>
        {
            subject = A.Fake<IHaveAnOut>();
        };

        private Because of = () =>
        {
            A.CallTo(() => subject.MightReturnAKnownValue(out outValue))
                .WithAnyArguments()
                .AssignsOutAndRefParametersLazily((string value) => new object[] { value == condition ? knownOutput : "me" });
        };

        private It should_assign_the_conditional_value_to_the_out_field = () =>
        {
            string value = condition;
            subject.MightReturnAKnownValue(out value);
            value.Should().BeEquivalentTo(knownOutput);
        };
    }

    public class when_configuring_a_fake_to_assign_out_and_ref_parameters_lazily_using_call
    {
        private static IHaveAnOut subject;
        private static string outValue;
        private static string condition = "someone_else";
        private static string knownOutput = "you";

        private Establish context = () =>
        {
            subject = A.Fake<IHaveAnOut>();
        };

        private Because of = () =>
        {
            A.CallTo(() => subject.MightReturnAKnownValue(out outValue))
                .WithAnyArguments()
                .AssignsOutAndRefParametersLazily((call) => new object[] { call.Arguments.Get<string>(0) == condition ? knownOutput : "me" });
        };

        private It should_assign_the_conditional_value_to_the_out_field = () =>
        {
            string value = condition;
            subject.MightReturnAKnownValue(out value);
            value.Should().BeEquivalentTo(knownOutput);
        };
    }

    public class when_configuring_a_fake_to_assign_out_and_ref_parameters
    {
        private static IHaveAnOut subject;
        private static string outValue;
        private static string condition = "someone_else";
        private static string knownOutput = "you";

        private Establish context = () =>
        {
            subject = A.Fake<IHaveAnOut>();
        };

        private Because of = () =>
        {
            A.CallTo(() => subject.MightReturnAKnownValue(out outValue))
                .WithAnyArguments()
                .AssignsOutAndRefParameters(knownOutput);
        };

        private It should_assign_the_conditional_value_to_the_out_field = () =>
        {
            string value = condition;
            subject.MightReturnAKnownValue(out value);
            value.Should().BeEquivalentTo(knownOutput);
        };
    }
}
