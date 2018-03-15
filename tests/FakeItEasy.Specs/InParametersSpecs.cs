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

#if FEATURE_IN_PARAM
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

// Below are characterization tests, representing the current capabilities of the code, not the desired state.
// If these tests start failing, update them and fix the "What can be faked" documentation page.
//
// See https://github.com/castleproject/Core/issues/339 for more details about the limitation.
#else
        [Scenario]
        public void FakingInParam(Exception exception)
        {
            "Given an interface with a method that takes an 'in' parameter"
                .See<IInParam>();

            "When I create a fake of the interface"
                .x(() => exception = Record.Exception(() => A.Fake<IInParam>()));

            "Then it throws"
                .x(() => exception.Should().BeAnExceptionOfType<FakeCreationException>());
        }
#endif

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
