namespace FakeItEasy.Specs
{
    using System.Diagnostics.CodeAnalysis;
    using FluentAssertions;
    using Machine.Specifications;

    public class when_faking_a_class_with_a_virtual_method
    {
        static MakesVirtualCallInConstructor fake;

        Because of = () => fake = A.Fake<MakesVirtualCallInConstructor>();

        It should_return_a_default_value_when_the_method_is_called_during_the_constructor =
            () => fake.VirtualMethodValueDuringConstructorCall.Should().Be(string.Empty);

        It should_return_a_default_value_when_the_method_is_called_after_the_constructor =
            () => fake.VirtualMethod("call after constructor").Should().Be(string.Empty);

        It should_record_the_method_call_during_the_constructor = () =>
            A.CallTo(() => fake.VirtualMethod("call in constructor")).MustHaveHappened();

        It should_record_the_method_call_after_the_constructor = () =>
            A.CallTo(() => fake.VirtualMethod("call after constructor")).MustHaveHappened();
    }

    public class when_faking_a_class_with_virtual_properties
    {
        static FakedClass fake;

        Because of = () => fake = A.Fake<FakedClass>();

        It should_return_a_default_value_when_a_reference_type_property_is_called_during_the_constructor = () =>
        {
            fake.StringPropertyValueDuringConstructorCall.Should().Be(string.Empty);
        };

        It should_return_a_default_value_when_a_value_type_property_is_called_during_the_constructor = () =>
        {
            // The test for the default value of a value type is a regression test for https://github.com/FakeItEasy/FakeItEasy/issues/368:
            fake.ValueTypePropertyValueDuringConstructorCall.Should().Be(0);
        };

        It should_return_the_value_assigned_during_constructor_when_a_reference_type_property_is_gotten_after_the_constructor = () =>
        {
            fake.StringProperty.Should().Be("value set in constructor");
        };

        It should_return_the_value_assigned_during_constructor_when_a_value_type_property_is_gotten_after_the_constructor = () =>
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
}
