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
                .x(() => fakeObjectContainer = CreateFakeObjectContainer("configured value in fake scope"));

            "and a fake scope using that container"
                .x(context => scope = Fake.CreateScope(fakeObjectContainer).Using(context));

            "when a fake is created inside the scope"
                .x(() => fake = A.Fake<MakesVirtualCallInConstructor>());

            "then the object container should configure the fake"
                .x(() => A.CallTo(() => fakeObjectContainer.ConfigureFake(typeof(MakesVirtualCallInConstructor), fake))
                    .MustHaveHappened());

            "and the object container's configuration should be used during the constructor"
                .x(() => fake.VirtualMethodValueDuringConstructorCall.Should().Be("configured value in fake scope"));

            "and the object container's configuration should be used after the constructor"
                .x(() => fake.VirtualMethod("call after constructor").Should().Be("configured value in fake scope"));
        }

        [Scenario]
        public static void UsingFakeOutsideOfScope(
            IFakeObjectContainer fakeObjectContainer,
            MakesVirtualCallInConstructor fake,
            string virtualMethodValueOutsideOfScope)
        {
            "given an object container"
                .x(() => fakeObjectContainer = CreateFakeObjectContainer("configured value in fake scope"));

            "and a fake created within a scope using that container"
                .x(() =>
                {
                    using (Fake.CreateScope(fakeObjectContainer))
                    {
                        fake = A.Fake<MakesVirtualCallInConstructor>();
                    }
                });

            "when the fake is accessed outside the scope"
                .x(() => virtualMethodValueOutsideOfScope = fake.VirtualMethod("call outside scope"));

            "then the object container's configuration should not be used"
                .x(() => virtualMethodValueOutsideOfScope.Should().Be(string.Empty));
        }

        private static IFakeObjectContainer CreateFakeObjectContainer(string stringMethodValue)
        {
            var fakeObjectContainer = A.Fake<IFakeObjectContainer>();
            A.CallTo(() => fakeObjectContainer.ConfigureFake(A<Type>._, A<object>._))
                .Invokes(
                    (Type t, object options) =>
                        A.CallTo(options).WithReturnType<string>().Returns(stringMethodValue));
            return fakeObjectContainer;
        }
    }
}