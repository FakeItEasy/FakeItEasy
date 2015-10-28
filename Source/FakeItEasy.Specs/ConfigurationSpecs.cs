namespace FakeItEasy.Specs
{
    using System;
    using FakeItEasy.Configuration;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xbehave;

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
            "establish"
                .x(() => fake = A.Fake<IFoo>());

            "when configuring callback"
                .x(() =>
                    {
                        A.CallTo(() => fake.Bar()).Invokes(x => wasCalled = true);
                        fake.Bar();
                    });

            "it should invoke the callback"
                .x(() => wasCalled.Should().BeTrue());
        }

        [Scenario]
        public static void MultipleCallbacks(
            IFoo fake,
            bool firstWasCalled,
            bool secondWasCalled,
            int returnValue)
        {
            "establish"
                .x(() => fake = A.Fake<IFoo>());

            "when configuring multiple callback"
                .x(() =>
                    {
                        A.CallTo(() => fake.Baz())
                            .Invokes(x => firstWasCalled = true)
                            .Invokes(x => secondWasCalled = true)
                            .Returns(10);

                        returnValue = fake.Baz();
                    });

            "it should call the first callback"
                .x(() => firstWasCalled.Should().BeTrue());

            "it should call the second callback"
                .x(() => secondWasCalled.Should().BeTrue());

            "it should return the configured value"
                .x(() => returnValue.Should().Be(10));
        }

        [Scenario]
        public static void CallBaseMethod(
            BaseClass fake,
            int returnValue,
            bool callbackWasInvoked)
        {
            "establish"
                .x(() => fake = A.Fake<BaseClass>());

            "when configuring to call base method"
                .x(() =>
                    {
                        A.CallTo(() => fake.ReturnSomething()).Invokes(x => callbackWasInvoked = true).CallsBaseMethod();
                        returnValue = fake.ReturnSomething();
                    });

            "it should have called the base method"
                .x(() => fake.WasCalled.Should().BeTrue());

            "it should return value from base method"
                .x(() => returnValue.Should().Be(10));

            "it should invoke the callback"
                .x(() => callbackWasInvoked.Should().BeTrue());
        }

        [Scenario]
        public static void MultipleReturns(
            IFoo fake,
            IReturnValueArgumentValidationConfiguration<int> configuration,
            Exception exception)
        {
            "establish"
                .x(() => fake = A.Fake<IFoo>());

            "when configuring multiple returns on the same configuration"
                .x(() =>
                {
                    configuration = A.CallTo(() => fake.Baz());
                    configuration.Returns(42);
                    exception = Record.Exception(() => configuration.Returns(0));
                });

            "it should throw an InvalidOperationException"
                .x(() => exception.Should().BeAnExceptionOfType<InvalidOperationException>());
        }

        [Scenario]
        public static void ReturnThenThrow(
            IFoo fake,
            IReturnValueArgumentValidationConfiguration<int> configuration,
            Exception exception)
        {
            "establish"
                .x(() => fake = A.Fake<IFoo>());

            "when configuring a return then a throw on the same configuration"
                .x(() =>
                {
                    configuration = A.CallTo(() => fake.Baz());
                    configuration.Returns(42);
                    exception = Record.Exception(() => configuration.Throws(new Exception()));
                });

            "it should throw an InvalidOperationException"
                .x(() => exception.Should().BeAnExceptionOfType<InvalidOperationException>());
        }

        [Scenario]
        public static void ReturnThenCallsBaseMethod(
            IFoo fake,
            IReturnValueArgumentValidationConfiguration<int> configuration,
            Exception exception)
        {
            "establish"
                .x(() => fake = A.Fake<IFoo>());

            "when configuring a return then base method call on the same configuration"
                .x(() =>
                {
                    configuration = A.CallTo(() => fake.Baz());
                    configuration.Returns(42);
                    exception = Record.Exception(() => configuration.CallsBaseMethod());
                });

            "it should throw an InvalidOperationException"
                .x(() => exception.Should().BeAnExceptionOfType<InvalidOperationException>());
        }

        [Scenario]
        public static void MultipleThrows(
            IFoo fake,
            IReturnValueArgumentValidationConfiguration<int> configuration,
            Exception exception)
        {
            "establish"
                .x(() => fake = A.Fake<IFoo>());

            "when configuring a return then a throw on the same configuration"
                .x(() =>
                {
                    configuration = A.CallTo(() => fake.Baz());
                    configuration.Throws(new ArgumentNullException());
                    exception = Record.Exception(() => configuration.Throws(new ArgumentException()));
                });

            "it should throw an InvalidOperationException"
                .x(() => exception.Should().BeAnExceptionOfType<InvalidOperationException>());
        }

        public class BaseClass
        {
            public bool WasCalled { get; set; }

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
