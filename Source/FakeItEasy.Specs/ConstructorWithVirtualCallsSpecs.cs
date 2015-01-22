namespace FakeItEasy.Specs
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using FakeItEasy.Core;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Machine.Specifications;

    public class SimpleVirtualCallInConstructor
    {
        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "This anti-pattern is part of the the tested scenario.")]
        public SimpleVirtualCallInConstructor()
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
        static SimpleVirtualCallInConstructor fake;

        Because of = () => fake = A.Fake<SimpleVirtualCallInConstructor>();

        It should_have_the_same_default_behavior_inside_and_outside_of_the_constructor_call = () =>
        {
            fake.VirtualMethodValueDuringConstructorCall.Should().Be(string.Empty);
            fake.VirtualMethod(null).Should().Be(string.Empty);
        };

        It should_record_the_virtual_method_call_during_the_constructor = () => 
            A.CallTo(() => fake.VirtualMethod("call in constructor")).MustHaveHappened();
    }

    public class when_faking_a_class_which_calls_virtual_member_in_constructor_with_changed_configuration_before_the_constructor
    {
        static SimpleVirtualCallInConstructor fake;

        Because of = () => 
            fake = A.Fake<SimpleVirtualCallInConstructor>(
                    o => o.OnFakeConfiguration(
                        f => A.CallTo(() => f.VirtualMethod(A<string>._)).Returns("configured value in fake options")));

        It should_return_the_configured_value_inside_and_outside_of_the_constructor_call = () =>
        {
            fake.VirtualMethodValueDuringConstructorCall.Should().Be("configured value in fake options");
            fake.VirtualMethod(null).Should().Be("configured value in fake options");
        };
    }

    public class when_faking_a_class_which_calls_virtual_member_in_constructor_within_fake_scope_which_configures_the_fake
    {
        static IFakeObjectContainer fakeObjectContainer;
        static SimpleVirtualCallInConstructor fake;
        static string virtualMethodValueInsideOfScope;
        static string virtualMethodValueOutsideOfScope;

        Establish context = () =>
        {
            fakeObjectContainer = A.Fake<IFakeObjectContainer>();
            A.CallTo(() => fakeObjectContainer.ConfigureFake(A<Type>._, A<object>._))
                .Invokes((Type t, object o) => A.CallTo(o).WithReturnType<string>().Returns("configured value in fake scope"));
        };

        Because of = () =>
        {
            using (Fake.CreateScope(fakeObjectContainer))
            {
                fake = A.Fake<SimpleVirtualCallInConstructor>();
                virtualMethodValueInsideOfScope = fake.VirtualMethod(null);
            }

            virtualMethodValueOutsideOfScope = fake.VirtualMethod(null);
        };

        It should_call_ConfigureFake_of_the_fake_scope = () => 
            A.CallTo(() => fakeObjectContainer.ConfigureFake(typeof(SimpleVirtualCallInConstructor), fake)).MustHaveHappened();

        It should_return_the_configured_value_from_the_scope_inside_and_outside_of_the_constructor_call = () =>
        {
            fake.VirtualMethodValueDuringConstructorCall.Should().Be("configured value in fake scope");
            virtualMethodValueInsideOfScope.Should().Be("configured value in fake scope");
        };

        It should_return_default_value_outside_of_scope = () => 
            virtualMethodValueOutsideOfScope.Should().Be(string.Empty);
    }

    public class when_faking_a_class_which_calls_virtual_member_in_constructor_with_CallsBaseMethods_option
    {
        static SimpleVirtualCallInConstructor fake;

        Because of = () => fake = A.Fake<SimpleVirtualCallInConstructor>(o => o.CallsBaseMethods());

        It should_call_base_method_inside_and_outside_of_the_constructor_call = () =>
        {
            fake.VirtualMethodValueDuringConstructorCall.Should().Be("implementation value");
            fake.VirtualMethod(null).Should().Be("implementation value");
        };
    }

    public class when_faking_a_class_which_calls_virtual_member_in_constructor_with_Strict_option
    {
        static Exception exception;

        Because of = () => exception = Catch.Exception(() => A.Fake<SimpleVirtualCallInConstructor>(o => o.Strict()));

        It should_throw_an_exception_during_the_constructor_call_because_of_the_unconfigured_call = () => 
            exception
                .Should()
                .BeAnExceptionOfType<FakeCreationException>()
                .WithMessage(
                    "\r\n  Failed to create fake of type \"FakeItEasy.Specs.SimpleVirtualCallInConstructor\".\r\n\r\n" +
                    "  Below is a list of reasons for failure per attempted constructor:\r\n" +
                    "    No constructor arguments failed:\r\n" +
                    "      No usable default constructor was found on the type FakeItEasy.Specs.SimpleVirtualCallInConstructor.\r\n" +
                    "      An exception of type FakeItEasy.ExpectationException was caught during this call. Its message was:\r\n" +
                    "      Call to non configured method \"VirtualMethod\" of strict fake.\r\n" + 
                    "*");
    }

    // This spec proves that we can configure methods of a strict fake and it applies to calls in the constructor (the 
    // counterpart to the previous scenario with an un-configured strict fake):
    public class when_faking_a_class_which_calls_virtual_member_in_constructor_with_Strict_option_plus_overridden_configuration
    {
        static SimpleVirtualCallInConstructor fake;

        Because of = () =>
            fake = A.Fake<SimpleVirtualCallInConstructor>(
                    o =>
                    {
                        o.Strict();
                        o.OnFakeConfiguration(f => A.CallTo(() => f.VirtualMethod(A<string>._)).Returns("configured value of strict fake"));
                    });

        It should_return_the_configured_value_inside_and_outside_of_the_constructor_call = () =>
        {
            fake.VirtualMethodValueDuringConstructorCall.Should().Be("configured value of strict fake");
            fake.VirtualMethod(null).Should().Be("configured value of strict fake");
        };
    }

    public class when_faking_a_class_which_calls_virtual_member_in_constructor_with_Wrapping_fake_option
    {
        static SimpleVirtualCallInConstructor fake;

        Because of = () =>
            fake = A.Fake<SimpleVirtualCallInConstructor>(
                    o => o.Wrapping(new SimpleVirtualCallInConstructor()));

        It should_delegate_to_the_wrapped_instance_inside_and_outside_of_the_constructor_call = () =>
        {
            fake.VirtualMethodValueDuringConstructorCall.Should().Be("implementation value");
            fake.VirtualMethod(null).Should().Be("implementation value");
        };
    }

    public class when_faking_a_class_which_calls_virtual_property_members_in_constructor
    {
        static FakedClass fake;

        Because of = () => fake = A.Fake<FakedClass>();

        It should_provide_default_values_during_constructor_execution = () =>
        {
            fake.StringPropertyValueDuringConstructorCall.Should().Be(string.Empty);

            // The test for the default value of a value type is a regression test for https://github.com/FakeItEasy/FakeItEasy/issues/368:
            fake.ValueTypePropertyValueDuringConstructorCall.Should().Be(0);
            fake.InterfacePropertyMethodResultDuringConstructorCall.Should().Be(string.Empty);
        };

        It should_save_the_values_assigned_during_constructor_execution = () =>
        {
            fake.StringProperty.Should().Be("value set in constructor");
            fake.ValueTypeProperty.Should().Be(123456);
            fake.InterfaceProperty.InnerProperty.Should().Be("value set in constructor");
        };

        public class FakedClass
        {
            [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "This anti-pattern is part of the the tested scenario.")]
            public FakedClass()
            {
                this.StringPropertyValueDuringConstructorCall = this.StringProperty;
                this.ValueTypePropertyValueDuringConstructorCall = this.ValueTypeProperty;
                this.InterfacePropertyMethodResultDuringConstructorCall = this.InterfaceProperty.InnerProperty;

                this.StringProperty = "value set in constructor";
                this.ValueTypeProperty = 123456;
                this.InterfaceProperty.InnerProperty = "value set in constructor";
            }

            public interface IInterface
            {
                string InnerProperty { get; set; }
            }

            public virtual string StringProperty { get; set; }

            public string StringPropertyValueDuringConstructorCall { get; private set; }

            public virtual int ValueTypeProperty { get; set; }

            public int ValueTypePropertyValueDuringConstructorCall { get; private set; }

            public virtual IInterface InterfaceProperty { get; set; }

            public string InterfacePropertyMethodResultDuringConstructorCall { get; private set; }
        }
    }

    // This spec proves that we can cope with throwing constructors (e.g. ensures that FakeManagers won't be reused):
    public class when_faking_a_class_with_a_failing_default_constructor_which_calls_virtual_member
    {
        static Action<FakedClass> onFakeConfiguration;
        static Action<FakedClass> onFakeCreated;
        static FakedClass fake;

        Establish context = () =>
        {
            onFakeConfiguration = A.Fake<Action<FakedClass>>();
            onFakeCreated = A.Fake<Action<FakedClass>>();
        };

        Because of = () =>
            fake = A.Fake<FakedClass>(
                    o =>
                    {
                        o.OnFakeConfiguration(onFakeConfiguration);
                        o.OnFakeCreated(onFakeCreated);
                    });

        It should_have_instantiated_the_fake_with_the_second_constructor = () =>
            fake.SecondConstructorCalled.Should().BeTrue();

        It should_use_a_fake_manager_which_did_not_receive_the_default_constructor_call = () =>
            fake.DefaultConstructorCalled.Should().BeFalse("because the default constructor was called on a *different* fake object");

        It should_call_OnFakeConfiguration_for_both_fakes = () =>
            A.CallTo(() => onFakeConfiguration(A<FakedClass>._)).MustHaveHappened(Repeated.Exactly.Twice);

        It should_call_OnFakeCreated_only_for_the_second_one = () =>
            A.CallTo(() => onFakeCreated(fake)).MustHaveHappened(Repeated.Exactly.Once);

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