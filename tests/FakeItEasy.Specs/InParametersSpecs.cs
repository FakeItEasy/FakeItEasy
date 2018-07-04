namespace FakeItEasy.Specs
{
    using System;
    using FakeItEasy.Core;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xbehave;
    using Xunit;

    public class InParameterSpecs
    {
        public interface IInParam
        {
            int Foo(in int x);
        }

        public interface IGenericInParam<T>
        {
            int Foo(in int x);
        }

        [Scenario]
        public void FakingInParam(IInParam fake, int argument, int result)
        {
            "Given a fake with a method that takes an 'in' parameter"
                .x(() => fake = A.Fake<IInParam>());

            "And a call to this method is configured configured to return a value"
                .x(() => A.CallTo(() => fake.Foo(in argument)).Returns(42));

            "When the method is called"
                .x(() => result = fake.Foo(in argument));

            "Then it returns the configured value"
                .x(() => result.Should().Be(42));
        }

        [Scenario]
        public void SettingInParamInterface(IInParam fake, int argument)
        {
            "Given a faked interface with a method that takes an 'in' parameter"
                .x(() => fake = A.Fake<IInParam>());

            "And the 'in' argument has a known value"
                .x(() => argument = 17);

            "And a call to this method is configured to set a new value for the parameter"
                .x(() => A.CallTo(() => fake.Foo(A<int>._)).AssignsOutAndRefParameters(19));

            "When the method is called"
                .x(() => fake.Foo(argument));

            "Then the 'in' argument retains its original value"
                .x(() => argument.Should().Be(17));
        }

        // A characterization test, representing the current capabilities of the code, not the desired state.
        // If it start failing, update it and fix the "What can be faked" documentation page.
        //
        // https://github.com/FakeItEasy/FakeItEasy/issues/1382 tracks this failing.
        [Scenario]
        public void FakingGenericInParam(Exception exception)
        {
            "Given an interface with a method that takes a generic 'in' parameter"
                .See<IGenericInParam<bool>>();

            "When I create a fake of the interface"
                .x(() => exception = Record.Exception(() => A.Fake<IGenericInParam<bool>>()));

            "Then it throws"
                .x(() => exception.Should().BeAnExceptionOfType<FakeCreationException>());
        }
    }
}
