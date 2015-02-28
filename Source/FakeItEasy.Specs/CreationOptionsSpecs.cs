namespace FakeItEasy.Specs
{
    using System;
    using FakeItEasy.Core;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Machine.Specifications;

    public class when_ConfigureFake_is_used_to_configure_a_method
    {
        private static MakesVirtualCallInConstructor fake;

        Because of = () => fake = A.Fake<MakesVirtualCallInConstructor>(options => options.ConfigureFake(
            f => A.CallTo(() => f.VirtualMethod(A<string>._))
                .Returns("configured value in fake options")));

        It should_return_the_configured_value_during_the_constructor =
            () => fake.VirtualMethodValueDuringConstructorCall.Should().Be("configured value in fake options");

        It should_return_the_configured_value_after_the_constructor =
            () => fake.VirtualMethod(null).Should().Be("configured value in fake options");
    }

    public class when_ConfigureFake_is_used_to_configure_a_method_also_configured_by_a_FakeConfigurator
    {
        static RobotRunsAmokEvent fake;

        Because of = () => fake = A.Fake<RobotRunsAmokEvent>(
            options => options.ConfigureFake(
                f => A.CallTo(() => f.CalculateTimestamp()).Returns(new DateTime(2000, 1, 1, 0, 0, 0))));

        It should_use_the_configured_behavior_from_the_ConfigureFake =
            () => fake.Timestamp.Should().Be(new DateTime(2000, 1, 1, 0, 0, 0));
    }

    public class when_CallsBaseMethods_is_used_to_configure_a_fake
    {
        static MakesVirtualCallInConstructor fake;

        Because of = () => fake = A.Fake<MakesVirtualCallInConstructor>(options => options.CallsBaseMethods());

        It should_call_base_method_during_the_constructor =
            () => fake.VirtualMethodValueDuringConstructorCall.Should().Be("implementation value");

        It should_call_base_method_after_the_constructor =
            () => fake.VirtualMethod(null).Should().Be("implementation value");
    }

    public class when_Strict_is_used_to_configure_a_fake
    {
        private static MakesVirtualCallInConstructor fake;

        Because of = () => fake = A.Fake<MakesVirtualCallInConstructor>(options => options.Strict());

        It should_throw_an_exception_from_a_method_call_during_the_constructor = () =>
        {
            fake.ExceptionFromVirtualMethodCallInConstructor
                .Should()
                .BeAnExceptionOfType<ExpectationException>()
                .WithMessage("Call to non configured method \"VirtualMethod\" of strict fake.");
        };

        It should_throw_an_exception_from_a_method_call_after_the_constructor = () =>
        {
            Record.Exception(() => fake.VirtualMethod("call outside constructor"))
                .Should()
                .BeAnExceptionOfType<ExpectationException>()
                .WithMessage("Call to non configured method \"VirtualMethod\" of strict fake.");
        };
    }

    public class when_Strict_followed_by_ConfigureFake_are_used_to_configure_a_fake
    {
        static MakesVirtualCallInConstructor fake;

        Because of = () =>
            fake = A.Fake<MakesVirtualCallInConstructor>(options => options
                .Strict()
                .ConfigureFake(f => A.CallTo(() => f.VirtualMethod(A<string>._))
                    .Returns("configured value of strict fake")));

        It should_return_the_configured_value_during_the_constructor =
            () => fake.VirtualMethodValueDuringConstructorCall.Should().Be("configured value of strict fake");

        It should_return_the_configured_value_after_the_constructor =
            () => fake.VirtualMethod(null).Should().Be("configured value of strict fake");
    }

    public class when_Wrapping_is_used_to_configure_a_fake
    {
        static MakesVirtualCallInConstructor fake;

        Because of = () =>
            fake = A.Fake<MakesVirtualCallInConstructor>(
                    options => options.Wrapping(new MakesVirtualCallInConstructor()));

        It should_delegate_to_the_wrapped_instance_during_the_constructor =
            () => fake.VirtualMethodValueDuringConstructorCall.Should().Be("implementation value");

        It should_delegate_to_the_wrapped_instance_after_the_constructor =
            () => fake.VirtualMethod(null).Should().Be("implementation value");
    }

    public class when_Wrapping_followed_by_ConfigureFake_are_used_to_configure_a_fake
    {
        static MakesVirtualCallInConstructor fake;

        Because of = () =>
            fake = A.Fake<MakesVirtualCallInConstructor>(options => options
                .Wrapping(new MakesVirtualCallInConstructor())
                .ConfigureFake(f => A.CallTo(() => f.VirtualMethod(A<string>._))
                    .Returns("configured in test")));

        It should_use_the_configured_behavior_during_the_constructor =
            () => fake.VirtualMethodValueDuringConstructorCall.Should().Be("configured in test");

        It should_use_the_configured_behavior_after_the_constructor =
            () => fake.VirtualMethod(null).Should().Be("configured in test");
    }

    public class when_ConfigureFake_followed_by_Wrapping_are_used_to_configure_a_fake
    {
        static MakesVirtualCallInConstructor fake;

        Because of = () =>
            fake = A.Fake<MakesVirtualCallInConstructor>(options => options
                    .ConfigureFake(f => A.CallTo(() => f.VirtualMethod(A<string>._))
                        .Returns("configured in test"))
                    .Wrapping(new MakesVirtualCallInConstructor()));

        It should_use_the_configured_behavior_during_the_constructor =
            () => fake.VirtualMethodValueDuringConstructorCall.Should().Be("configured in test");

        It should_use_the_configured_behavior_after_the_constructor =
            () => fake.VirtualMethod(null).Should().Be("configured in test");
    }

    public class when_Wrapping_is_used_to_configure_a_fake_that_has_a_FakeConfigurator
    {
        static RobotRunsAmokEvent fake;

        Because of = () =>
            fake = A.Fake<RobotRunsAmokEvent>(
                options => options.Wrapping(new RobotRunsAmokEvent()));

        It should_delegate_to_the_wrapped_object =
            () => fake.Timestamp.Should().Be(DomainEvent.DefaultTimestamp);
    }
}