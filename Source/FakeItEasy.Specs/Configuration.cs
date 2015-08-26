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
            "establish".x(() => fake = A.Fake<IFoo>());

            "when configuring callback".x(() =>
            {
                A.CallTo(() => fake.Bar()).Invokes(x => wasCalled = true);
                fake.Bar();
            });

            "it should invoke the callback".x(() =>
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
            "establish".x(() => fake = A.Fake<IFoo>());

            "when configuring callback".x(() =>
            {
                A.CallTo(() => fake.Baz())
                    .Invokes(x => firstWasCalled = true)
                    .Invokes(x => secondWasCalled = true)
                    .Returns(10);

                returnValue = fake.Baz();
            });

            "it should call the first callback".x(() =>
            {
                firstWasCalled.Should().BeTrue();
            });

            "it should call the second callback".x(() =>
            {
                secondWasCalled.Should().BeTrue();
            });

            "it should return the configured value".x(() =>
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
            "establish".x(() => fake = A.Fake<BaseClass>());

            "when configuring callback".x(() =>
            {
                A.CallTo(() => fake.ReturnSomething()).Invokes(x => callbackWasInvoked = true).CallsBaseMethod();
                returnValue = fake.ReturnSomething();
            });

            "it shuld have called the base method".x(() =>
            {
                fake.WasCalled.Should().BeTrue();
            });

            "it should return value from base method".x(() =>
            {
                returnValue.Should().Be(10);
            });

            "it should invoke the callback".x(() =>
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
