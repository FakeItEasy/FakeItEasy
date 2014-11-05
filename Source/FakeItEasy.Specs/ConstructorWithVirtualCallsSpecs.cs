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
}