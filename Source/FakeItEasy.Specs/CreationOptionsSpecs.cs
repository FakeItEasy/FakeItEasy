namespace FakeItEasy.Specs
{
    using System;
    using FakeItEasy.Core;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Machine.Specifications;

    public class when_faking_a_class_which_calls_configured_virtual_member_in_constructor
    {
        static MakesVirtualCallInConstructor fake;

        Because of = () => fake = A.Fake<MakesVirtualCallInConstructor>(options => options.ConfigureFake(
            f => A.CallTo(() => f.VirtualMethod(A<string>._))
                .Returns("configured value in fake options")));

        private It should_return_the_configured_value_during_the_constructor =
            () => fake.VirtualMethodValueDuringConstructorCall.Should().Be("configured value in fake options");

        private It should_return_the_configured_value_after_the_constructor =
            () => fake.VirtualMethod(null).Should().Be("configured value in fake options");
    }

    public class when_configuring_a_method_called_by_a_constructor_that_is_also_configured_by_a_fake_configurator
    {
        static RobotRunsAmokEvent fake = null;

        Because of = () => fake = A.Fake<RobotRunsAmokEvent>(
            options => options.ConfigureFake(
                f => A.CallTo(() => f.CalculateTimestamp()).Returns(new DateTime(2000, 1, 1, 0, 0, 0))));

        It should_use_the_configured_behavior_from_the_fake_options =
            () => fake.Timestamp.Should().Be(new DateTime(2000, 1, 1, 0, 0, 0));
    }

    public class when_configuring_a_method_called_by_a_constructor_to_call_base_method
    {
        static MakesVirtualCallInConstructor fake;

        Because of = () => fake = A.Fake<MakesVirtualCallInConstructor>(options => options.CallsBaseMethods());

        It should_call_base_method_during_constructor =
            () => fake.VirtualMethodValueDuringConstructorCall.Should().Be("implementation value");

        It should_call_base_method_after_constructor =
            () => fake.VirtualMethod(null).Should().Be("implementation value");
    }

    public class when_configuring_a_method_called_by_a_constructor_to_be_strict
    {
        static Exception exception;

        Because of = () => exception = Record.Exception(() => A.Fake<MakesVirtualCallInConstructor>(options => options.Strict()));

        private It should_throw_an_exception_during_the_constructor = () =>
        {
            var expectedMessage = new[]
            {
                string.Empty,
                "  Failed to create fake of type \"FakeItEasy.Specs.MakesVirtualCallInConstructor\".",
                string.Empty,
                "  Below is a list of reasons for failure per attempted constructor:",
                "    No constructor arguments failed:",
                "      No usable default constructor was found on the type FakeItEasy.Specs.MakesVirtualCallInConstructor.",
                "      An exception of type FakeItEasy.ExpectationException was caught during this call. Its message was:",
                "      Call to non configured method \"VirtualMethod\" of strict fake.",
                "*"
            }.AsTextBlock();

            exception
                .Should()
                .BeAnExceptionOfType<FakeCreationException>()
                .WithMessage(
                    expectedMessage);
        };
    }

    // This spec proves that we can configure methods of a strict fake and it applies to calls in the constructor (the 
    // counterpart to the previous scenario with an un-configured strict fake):
    public class when_configuring_a_method_called_by_a_constructor_to_be_strict_and_adding_configuration
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

    public class when_wrapping_an_object_in_a_fake_that_calls_virtual_member_in_constructor
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

    public class when_wrapping_an_object_and_then_configuring_a_method_called_by_constructor
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

    public class when_configuring_a_method_called_by_constructor_and_then_wrapping_an_object
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

    public class when_wrapping_an_object_with_a_fake_that_has_a_fake_configurator
    {
        static RobotRunsAmokEvent fake;

        Because of = () =>
            fake = A.Fake<RobotRunsAmokEvent>(
                options => options.Wrapping(new RobotRunsAmokEvent()));

        private It should_delegate_to_the_wrapped_object =
            () => fake.Timestamp.Should().Be(DomainEvent.DefaultTimestamp);
    }
}