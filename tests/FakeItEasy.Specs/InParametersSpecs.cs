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

        public delegate void DelegateWithInParam(in int i);

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
        public void SettingInParamInterface(IInParam fake, Exception exception)
        {
            "Given a faked interface with a method that takes an 'in' parameter"
                .x(() => fake = A.Fake<IInParam>());

            "And a call to this method is configured to set a new value for the parameter"
                .x(() => A.CallTo(() => fake.Foo(A<int>._)).AssignsOutAndRefParameters(19));

            "When the method is called"
                .x(() => exception = Record.Exception(() => fake.Foo(A.Dummy<int>())));

            "Then it throws an exception"
                .x(() => exception.Should().BeAnExceptionOfType<InvalidOperationException>());

            "And the exception message indicates that the out and ref parameter counts don't agree"
                .x(() => exception.Message.Should()
                    .Be("The number of values for out and ref parameters specified does not match the number of out and ref parameters in the call."));
        }

        [Scenario]
        public void SettingInParamDelegate(DelegateWithInParam fake, Exception exception)
        {
            "Given a faked delegate with a method that takes an 'in' parameter"
                .x(() => fake = A.Fake<DelegateWithInParam>());

            "And a call to this method is configured to set a new value for the parameter"
                .x(() => A.CallTo(() => fake.Invoke(A<int>._)).AssignsOutAndRefParameters(19));

            "When the method is called"
                .x(() => exception = Record.Exception(() => fake.Invoke(A.Dummy<int>())));

            "Then it throws an exception"
                .x(() => exception.Should().BeAnExceptionOfType<InvalidOperationException>());

            "And the exception message indicates that the out and ref parameter counts don't agree"
                .x(() => exception.Message.Should()
                    .Be("The number of values for out and ref parameters specified does not match the number of out and ref parameters in the call."));
        }

#if LACKS_FAKEABLE_GENERIC_IN_PARAMETERS
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
#else
        [Scenario]
        public void FakingGenericInParam(IGenericInParam<bool> fake, int argument, int result)
        {
            "Given a faked generic interface with a method that takes an 'in' parameter"
                .x(() => fake = A.Fake<IGenericInParam<bool>>());

            "And a call to this method is configured to return a value"
                .x(() => A.CallTo(() => fake.Foo(in argument)).Returns(42));

            "When the method is called"
                .x(() => result = fake.Foo(in argument));

            "Then it returns the configured value"
                .x(() => result.Should().Be(42));
        }
#endif
    }
}
