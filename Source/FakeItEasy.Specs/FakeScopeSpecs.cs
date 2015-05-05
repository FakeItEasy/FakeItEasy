namespace FakeItEasy.Specs
{
    using System;
    using Core;
    using FluentAssertions;
    using Machine.Specifications;

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
}
