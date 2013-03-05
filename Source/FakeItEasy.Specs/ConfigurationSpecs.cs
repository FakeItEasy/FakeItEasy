﻿namespace FakeItEasy.Specs
{
    using Machine.Specifications;
    using Tests;

    public class ConfigSpecifications<T>
    {
        Establish context = () => Fake = A.Fake<T>();

        protected static T Fake { get; set; }
    }

    public class when_configuring_callback
        : ConfigSpecifications<IFoo>
    {
        private static bool wasCalled;

        Because of = () =>
        {
            A.CallTo(() => Fake.Bar()).Invokes(x => wasCalled = true);
            Fake.Bar();
        };

        It should_invoke_the_callback = () => wasCalled.ShouldBeTrue();
    }

    public class when_configuring_multiple_callbacks
        : ConfigSpecifications<IFoo>
    {
        static bool firstWasCalled;
        static bool secondWasCalled;
        static int returnValue;

        Because of = () =>
        {
            A.CallTo(() => Fake.Baz())
                .Invokes(x => firstWasCalled = true)
                .Invokes(x => secondWasCalled = true)
                .Returns(10);

            returnValue = Fake.Baz();
        };

        It should_call_the_first_callback = () => firstWasCalled.ShouldBeTrue();
        It should_call_the_second_callback = () => secondWasCalled.ShouldBeTrue();
        It should_return_the_configured_value = () => returnValue.ShouldEqual(10);
    }

    public class when_configuring_to_call_base_method
        : ConfigSpecifications<BaseClass>
    {
        static int returnValue;
        static bool callbackWasInvoked;

        Because of = () =>
        {
            A.CallTo(() => Fake.ReturnSomething()).Invokes(x => callbackWasInvoked = true).CallsBaseMethod();
            returnValue = Fake.ReturnSomething();
        };

        It shuld_have_called_the_base_method = () => Fake.WasCalled.ShouldBeTrue();
        It should_return_value_from_base_method = () => returnValue.ShouldEqual(10);
        It should_invoke_the_callback = () => callbackWasInvoked.ShouldBeTrue();
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
