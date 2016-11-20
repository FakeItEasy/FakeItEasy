namespace FakeItEasy.Specs
{
    using System.Diagnostics.CodeAnalysis;
    using FluentAssertions;
    using Xbehave;

    public static class UnconfiguredFakeSpecs
    {
        public interface IFoo
        {
            bool IsADummy { get; }
        }

        [Scenario]
        public static void VirtualMethodCalledDuringConstruction(
            MakesVirtualCallInConstructor fake)
        {
            "Given a type with a default constructor"
                .x(() => { }); // see MakesVirtualCallInConstructor

            "And the constructor calls a virtual method"
                .x(() => { }); // see MakesVirtualCallInConstructor.ctor()

            "And the method returns a non-default value"
               .x(() => { }); // see MakesVirtualCallInConstructor.VirtualMethod()

            "When I create a fake of the type"
                .x(() => fake = A.Fake<MakesVirtualCallInConstructor>());

            "Then the method will return a default value"
                .x(() => fake.VirtualMethodValueDuringConstructorCall.Should().Be(string.Empty));

            "And the method call will be recorded"
                .x(() => A.CallTo(() => fake.VirtualMethod("call in constructor")).MustHaveHappened());
        }

        [Scenario]
        public static void VirtualMethodCalledAfterConstruction(
            MakesVirtualCallInConstructor fake,
            string result)
        {
            "Given a type with a virtual method"
                .x(() => { }); // see MakesVirtualCallInConstructor

            "And the method returns a non-default value"
                .x(() => { }); // see MakesVirtualCallInConstructor.VirtualMethod

            "And a fake of that type"
                .x(() => fake = A.Fake<MakesVirtualCallInConstructor>());

            "When I call the method"
                .x(() => result = fake.VirtualMethod("call after constructor"));

            "Then it will return a default value"
                .x(() => result.Should().Be(string.Empty));

            "And the method call will be recorded"
                .x(() => A.CallTo(() => fake.VirtualMethod("call after constructor")).MustHaveHappened());
        }

        [Scenario]
        public static void VirtualPropertiesCalledDuringConstruction(
            FakedClass fake)
        {
            "Given a type with a default constructor"
                .x(() => { }); // see FakedClass

            "And the constructor calls some virtual properties"
                .x(() => { }); // see FakedClass.ctor()

            "And the properties return non-default values"
               .x(() => { }); // see FakedClass.StringProperty, FakedClass.ValueTypeProperty

            "When I create a fake of the type"
                .x(() => fake = A.Fake<FakedClass>());

            "Then the reference-type property will return a default value"
                .x(() => fake.StringPropertyValueDuringConstructorCall.Should().Be(string.Empty));

            "And the value-type property will return a default value"
                .x(() => fake.ValueTypePropertyValueDuringConstructorCall.Should().Be(0));
        }

        [Scenario]
        public static void VirtualReferenceTypeProperty(
            FakedClass fake,
            string result)
        {
            "Given a type with a default constructor"
                .x(() => { }); // see FakedClass

            "And the constructor assigns a value to a virtual reference-type property"
                .x(() => { }); // see FakedClass.ctor()

            "And a fake of that type"
                .x(() => fake = A.Fake<FakedClass>());

            "When I fetch the property value"
                .x(() => result = fake.StringProperty);

            "Then it will be the value assigned during construction"
                .x(() => result.Should().Be("value set in constructor"));
        }

        [Scenario]
        public static void VirtualValueTypeProperty(
            FakedClass fake,
            int result)
        {
            "Given a type with a default constructor"
                .x(() => { }); // see FakedClass

            "And the constructor assigns a value to a virtual value-type property"
                .x(() => { }); // see FakedClass.ctor()

            "And a fake of that type"
                .x(() => fake = A.Fake<FakedClass>());

            "When I fetch the property value"
                .x(() => result = fake.ValueTypeProperty);

            "Then it will be the value assigned during construction"
                .x(() => result.Should().Be(123456));
        }

        [Scenario]
        public static void FakeableProperty(
            FakedClass fake,
            IFoo result)
        {
            "Given a type with a virtual fakeable-type property"
                .x(() => { }); // see FakedClasss

            "And a fake of that type"
                .x(() => fake = A.Fake<FakedClass>());

            "When I get the property value"
                .x(() => result = fake.FakeableProperty);

            "Then the value will be a Dummy"
                .x(() => result.IsADummy.Should().BeTrue("because the property value should be a Dummy"));
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

            public string StringPropertyValueDuringConstructorCall { get; }

            public virtual int ValueTypeProperty { get; set; }

            public int ValueTypePropertyValueDuringConstructorCall { get; }

            public virtual IFoo FakeableProperty { get; set; }
        }

        public class Foo : IFoo
        {
            public bool IsADummy { get; set; }
        }

        public class FooFactory : DummyFactory<IFoo>
        {
            protected override IFoo Create()
            {
                return new Foo { IsADummy = true };
            }
        }
    }
}
