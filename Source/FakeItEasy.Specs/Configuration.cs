namespace FakeItEasy.Specs
{
    using FluentAssertions;
    using Xbehave;

    public class Configuration
    {
        public interface IFoo
        {
            void Bar();

            int Baz();
        }

        [Scenario]
        public void when_configuring_callback(
            IFoo fake,
            bool wasCalled)
        {
            "establish"._(() => fake = A.Fake<IFoo>());

            "when configuring callback"._(() =>
            {
                A.CallTo(() => fake.Bar()).Invokes(x => wasCalled = true);
                fake.Bar();
            });

            "it should invoke the callback"._(() =>
            {
                wasCalled.Should().BeTrue();
            });
        }

        [Scenario]
        public void when_configuring_multiple_callbacks(
            IFoo fake,
            bool firstWasCalled,
            bool secondWasCalled,
            int returnValue)
        {
            "establish"._(() => fake = A.Fake<IFoo>());

            "when configuring callback"._(() =>
            {
                A.CallTo(() => fake.Baz())
                    .Invokes(x => firstWasCalled = true)
                    .Invokes(x => secondWasCalled = true)
                    .Returns(10);

                returnValue = fake.Baz();
            });

            "it should call the first callback"._(() =>
            {
                firstWasCalled.Should().BeTrue();
            });

            "it should call the second callback"._(() =>
            {
                secondWasCalled.Should().BeTrue();
            });

            "it should return the configured value"._(() =>
            {
                returnValue.Should().Be(10);
            });
        }

        [Scenario]
        public void when_configuring_to_call_base_method(
            BaseClass fake,
            int returnValue,
            bool callbackWasInvoked)
        {
            "establish"._(() => fake = A.Fake<BaseClass>());

            "when configuring callback"._(() =>
            {
                A.CallTo(() => fake.ReturnSomething()).Invokes(x => callbackWasInvoked = true).CallsBaseMethod();
                returnValue = fake.ReturnSomething();
            });

            "it shuld have called the base method"._(() =>
            {
                fake.WasCalled.Should().BeTrue();
            });

            "it should return value from base method"._(() =>
            {
                returnValue.Should().Be(10);
            });

            "it should invoke the callback"._(() =>
            {
                callbackWasInvoked.Should().BeTrue();
            });
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
