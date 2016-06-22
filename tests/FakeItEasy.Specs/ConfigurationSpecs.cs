namespace FakeItEasy.Specs
{
    using System;
    using FakeItEasy.Configuration;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xbehave;
    using Xunit;

    public static class ConfigurationSpecs
    {
        public interface IFoo
        {
            void Bar();

            int Baz();
        }

        [Scenario]
        public static void Callback(
            IFoo fake,
            bool wasCalled)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And I configure a method to invoke an action"
                .x(() => A.CallTo(() => fake.Bar()).Invokes(x => wasCalled = true));

            "When I call the method"
                .x(() => fake.Bar());

            "Then it invokes the action"
                .x(() => wasCalled.Should().BeTrue());
        }

        [Scenario]
        public static void MultipleCallbacks(
            IFoo fake,
            bool firstWasCalled,
            bool secondWasCalled,
            int returnValue)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And I configure a method to invoke two actions and return a value"
                .x(() =>
                    A.CallTo(() => fake.Baz())
                        .Invokes(x => firstWasCalled = true)
                        .Invokes(x => secondWasCalled = true)
                        .Returns(10));

            "When I call the method"
                .x(() => returnValue = fake.Baz());

            "Then it calls the first callback"
                .x(() => firstWasCalled.Should().BeTrue());

            "And it calls the first callback"
                .x(() => secondWasCalled.Should().BeTrue());

            "And it returns the configured value"
                .x(() => returnValue.Should().Be(10));
        }

        [Scenario]
        public static void CallBaseMethod(
            BaseClass fake,
            int returnValue,
            bool callbackWasInvoked)
        {
            "Given a fake"
                .x(() => fake = A.Fake<BaseClass>());

            "And I configure a method to invoke an action and call the base method"
                .x(() =>
                    A.CallTo(() => fake.ReturnSomething())
                        .Invokes(x => callbackWasInvoked = true)
                        .CallsBaseMethod());

            "When I call the method"
                .x(() => returnValue = fake.ReturnSomething());

            "Then it calls the base method"
                .x(() => fake.WasCalled.Should().BeTrue());

            "And it returns the value from base method"
                .x(() => returnValue.Should().Be(10));

            "And it invokes the callback"
                .x(() => callbackWasInvoked.Should().BeTrue());
        }

        [Scenario]
        public static void MultipleReturns(
            IFoo fake,
            IReturnValueArgumentValidationConfiguration<int> configuration,
            Exception exception)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And I configure the return value for the method"
                .x(() =>
                {
                    configuration = A.CallTo(() => fake.Baz());
                    configuration.Returns(42);
                });

            "When I use the same configuration object to set the return value again"
                .x(() => exception = Record.Exception(() => configuration.Returns(0)));

            "Then it throws an invalid operation exception"
                .x(() => exception.Should().BeAnExceptionOfType<InvalidOperationException>());
        }

        [Scenario]
        public static void ReturnThenThrow(
            IFoo fake,
            IReturnValueArgumentValidationConfiguration<int> configuration,
            Exception exception)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And I configure the return value for the method"
                .x(() =>
                {
                    configuration = A.CallTo(() => fake.Baz());
                    configuration.Returns(42);
                });

            "When I use the same configuration object to have the method throw an exception"
                .x(() => exception = Record.Exception(() => configuration.Throws<Exception>()));

            "Then it throws an invalid operation exception"
                .x(() => exception.Should().BeAnExceptionOfType<InvalidOperationException>());
        }

        [Scenario]
        public static void ReturnThenCallsBaseMethod(
            IFoo fake,
            IReturnValueArgumentValidationConfiguration<int> configuration,
            Exception exception)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And I configure the return value for the method"
                .x(() =>
                {
                    configuration = A.CallTo(() => fake.Baz());
                    configuration.Returns(42);
                });

            "When I use the same configuration object to have the method call the base method"
                .x(() => exception = Record.Exception(() => configuration.CallsBaseMethod()));

            "Then it throws an invalid operation exception"
                .x(() => exception.Should().BeAnExceptionOfType<InvalidOperationException>());
        }

        [Scenario]
        public static void MultipleThrows(
            IFoo fake,
            IReturnValueArgumentValidationConfiguration<int> configuration,
            Exception exception)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And I configure the return method to throw an exception"
                .x(() =>
                {
                    configuration = A.CallTo(() => fake.Baz());
                    configuration.Throws<ArgumentNullException>();
                });

            "When I use the same configuration object to have the method throw an exception again"
                .x(() => exception = Record.Exception(() => configuration.Throws<ArgumentException>()));

            "Then it throws an invalid operation exception"
                .x(() => exception.Should().BeAnExceptionOfType<InvalidOperationException>());
        }

        public class BaseClass
        {
            public bool WasCalled { get; private set; }

            public virtual void DoSomething()
            {
                this.WasCalled = true;
            }

            public virtual int ReturnSomething()
            {
                this.WasCalled = true;
                return 10;
            }
        }
    }
}
