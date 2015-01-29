namespace FakeItEasy.Specs
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using FakeItEasy.Core;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Machine.Specifications;

    public class MakesVirtualCallInConstructor
    {
        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "This anti-pattern is part of the the tested scenario.")]
        public MakesVirtualCallInConstructor()
        {
            this.VirtualMethodValueDuringConstructorCall = this.VirtualMethod("call in constructor");
        }

        public string VirtualMethodValueDuringConstructorCall { get; private set; }

        public virtual string VirtualMethod(string parameter)
        {
            return "implementation value";
        }
    }

    public class when_faking_a_class_which_calls_virtual_member_in_constructor
    {
        static MakesVirtualCallInConstructor fake;

        Because of = () => fake = A.Fake<MakesVirtualCallInConstructor>();

        It should_use_default_behavior_during_the_constructor =
            () => fake.VirtualMethodValueDuringConstructorCall.Should().Be(string.Empty);

        It should_use_default_behaviour_after_the_constructor =
            () => fake.VirtualMethod(null).Should().Be(string.Empty);

        It should_record_the_virtual_method_call_during_the_constructor = () => 
            A.CallTo(() => fake.VirtualMethod("call in constructor")).MustHaveHappened();
    }

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

    public class when_configuring_a_method_called_by_a_constructor_from_within_a_scope
    {
        static IFakeObjectContainer fakeObjectContainer;
        static MakesVirtualCallInConstructor fake;
        static string virtualMethodValueInsideOfScope;
        static string virtualMethodValueOutsideOfScope;

        Establish context = () =>
        {
            fakeObjectContainer = A.Fake<IFakeObjectContainer>();
            A.CallTo(() => fakeObjectContainer.ConfigureFake(A<Type>._, A<object>._))
                .Invokes((Type t, object options) => A.CallTo(options).WithReturnType<string>().Returns("configured value in fake scope"));
        };

        Because of = () =>
        {
            using (Fake.CreateScope(fakeObjectContainer))
            {
                fake = A.Fake<MakesVirtualCallInConstructor>();
                virtualMethodValueInsideOfScope = fake.VirtualMethod(null);
            }

            virtualMethodValueOutsideOfScope = fake.VirtualMethod(null);
        };

        It should_call_ConfigureFake_of_the_fake_scope = () => 
            A.CallTo(() => fakeObjectContainer.ConfigureFake(typeof(MakesVirtualCallInConstructor), fake)).MustHaveHappened();

        It should_return_the_configured_value_within_the_scope_during_the_constructor =
            () => fake.VirtualMethodValueDuringConstructorCall.Should().Be("configured value in fake scope");

        It should_return_the_configured_value_within_the_scope_after_the_constructor =
            () => virtualMethodValueInsideOfScope.Should().Be("configured value in fake scope");

        It should_return_default_value_outside_the_scope = () => 
            virtualMethodValueOutsideOfScope.Should().Be(string.Empty);
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

    public class when_faking_a_class_which_calls_virtual_properties_in_constructor
    {
        static FakedClass fake;

        Because of = () => fake = A.Fake<FakedClass>();

        It should_provide_a_default_reference_type_value_during_the_constructor = () =>
        {
            fake.StringPropertyValueDuringConstructorCall.Should().Be(string.Empty);
        };

        It should_provide_a_default_value_type_value_during_the_constructor = () =>
        {
            // The test for the default value of a value type is a regression test for https://github.com/FakeItEasy/FakeItEasy/issues/368:
            fake.ValueTypePropertyValueDuringConstructorCall.Should().Be(0);
        };

        It should_save_a_reference_type_value_assigned_during_the_constructor = () =>
        {
            fake.StringProperty.Should().Be("value set in constructor");
        };

        It should_save_a_value_type_value_assigned_during_the_constructor = () =>
        {
            fake.ValueTypeProperty.Should().Be(123456);
        };

        public class FakedClass
        {
            [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "This anti-pattern is part of the the tested scenario.")]
            public FakedClass()
            {
                this.StringPropertyValueDuringConstructorCall = this.StringProperty;
                this.ValueTypePropertyValueDuringConstructorCall = this.ValueTypeProperty;

                this.StringProperty = "value set in constructor";
                this.ValueTypeProperty = 123456;
            }

            public virtual string StringProperty { get; set; }

            public string StringPropertyValueDuringConstructorCall { get; private set; }

            public virtual int ValueTypeProperty { get; set; }

            public int ValueTypePropertyValueDuringConstructorCall { get; private set; }
        }
    }

    // This spec proves that we can cope with throwing constructors (e.g. ensures that FakeManagers won't be reused):
    public class when_faking_a_class_whose_first_constructor_fails
    {
        static Action<FakedClass> onFakeConfiguration;
        static FakedClass fake;

        Establish context = () =>
        {
            onFakeConfiguration = A.Fake<Action<FakedClass>>();
        };

        Because of = () => fake = A.Fake<FakedClass>(options => options.ConfigureFake(onFakeConfiguration));

        It should_instantiate_the_fake_using_the_second_constructor = () =>
            fake.SecondConstructorCalled.Should().BeTrue();

        It should_use_a_fake_manager_which_did_not_receive_the_first_constructor_call = () =>
            fake.DefaultConstructorCalled.Should().BeFalse("because the default constructor was called on a *different* fake object");

        It should_call_fake_configuration_actions_for_each_constructor = () =>
            A.CallTo(() => onFakeConfiguration(A<FakedClass>._)).MustHaveHappened(Repeated.Exactly.Twice);

        public class FakedClass
        {
            [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "This anti-pattern is part of the the tested scenario.")]
            public FakedClass()
            {
                this.DefaultConstructorCalled = true;

                throw new InvalidOperationException();
            }

            [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "This anti-pattern is part of the the tested scenario.")]
            [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "someInterface", Justification = "This is just a dummy argument.")]
            public FakedClass(IDisposable someInterface)
            {
                this.SecondConstructorCalled = true;
            }

            public virtual bool DefaultConstructorCalled { get; set; }

            public virtual bool SecondConstructorCalled { get; set; }
        }
    }
}