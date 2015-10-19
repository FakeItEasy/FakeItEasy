namespace FakeItEasy.Specs
{
    using System;
    using Core;
    using FluentAssertions;
    using Xbehave;

    public static class FakeScopeSpecs
    {
        [Scenario]
        public static void CreatingFakeInsideScope(
            IFakeObjectContainer fakeObjectContainer,
            IFakeScope scope,
            MakesVirtualCallInConstructor fake)
        {
            "given an object container"
                .x(() =>
                {
                    fakeObjectContainer = A.Fake<IFakeObjectContainer>();
                    A.CallTo(() => fakeObjectContainer.ConfigureFake(A<Type>._, A<object>._))
                        .Invokes(
                            (Type t, object options) =>
                                A.CallTo(options).WithReturnType<string>().Returns("configured value in fake scope"));
                });

            "and a fake scope using that container"
                .x(() => scope = Fake.CreateScope(fakeObjectContainer));

            "when a fake is created inside the scope"
                .x(() => fake = A.Fake<MakesVirtualCallInConstructor>());

            "then the object container should configure the fake"
                .x(() => A.CallTo(() => fakeObjectContainer.ConfigureFake(typeof(MakesVirtualCallInConstructor), fake))
                    .MustHaveHappened());

            "and the object container's configuration should be used during the constructor"
                .x(() => fake.VirtualMethodValueDuringConstructorCall.Should().Be("configured value in fake scope"));

            "and the object container's configuration should be used after the constructor"
                .x(() => fake.VirtualMethod("call after constructor").Should().Be("configured value in fake scope"));

            "and the object container's configuration should not be used outside the scope"
                .x(() =>
                {
                    scope.Dispose();
                    fake.VirtualMethod("call outside scope").Should().Be(string.Empty);
                });
        }
    }
}