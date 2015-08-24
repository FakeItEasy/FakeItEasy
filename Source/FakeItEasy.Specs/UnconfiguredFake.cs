namespace FakeItEasy.Specs
{
    using System.Diagnostics.CodeAnalysis;
    using FluentAssertions;
    using Xbehave;

    public class UnconfiguredFake
    {
        [Scenario]
        public void when_faking_a_class_with_a_virtual_method(
            MakesVirtualCallInConstructor fake)
        {
            "when faking a class with a virtual method"._(() =>
            {
                fake = A.Fake<MakesVirtualCallInConstructor>();
            });

            "it should return a default value when the method is called during the constructor"._(() =>
            {
                fake.VirtualMethodValueDuringConstructorCall.Should().Be(string.Empty);
            });

            "it should return a default value when the method is called after the constructor"._(() =>
            {
                fake.VirtualMethod("call after constructor").Should().Be(string.Empty);
            });

            "it should record the method call during the constructor"._(() =>
            {
                A.CallTo(() => fake.VirtualMethod("call in constructor")).MustHaveHappened();
            });

            "it should record the method call after the constructor"._(() =>
            {
                A.CallTo(() => fake.VirtualMethod("call after constructor")).MustHaveHappened();
            });
        }

        [Scenario]
        public void when_faking_a_class_with_virtual_properties(
            FakedClass fake)
        {
            "when faking a class with a virtual method"._(() =>
            {
                fake = A.Fake<FakedClass>();
            });

            "it should return a default value when a reference type property is called during the constructor"._(() =>
            {
                fake.StringPropertyValueDuringConstructorCall.Should().Be(string.Empty);
            });

            "it should return a default value when a value type property is called during the constructor"._(() =>
            {
                // The test for the default value of a value type is a regression test for https://github.com/FakeItEasy/FakeItEasy/issues/368:
                fake.ValueTypePropertyValueDuringConstructorCall.Should().Be(0);
            });

            "it should return the value assigned during constructor when a reference type property is gotten after the constructor"._(() =>
            {
                fake.StringProperty.Should().Be("value set in constructor");
            });

            "it should return the value assigned during constructor when a value type property is gotten after the constructor"._(() =>
            {
                fake.ValueTypeProperty.Should().Be(123456);
            });
        }

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
