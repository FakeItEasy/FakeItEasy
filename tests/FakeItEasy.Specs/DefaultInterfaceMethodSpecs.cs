namespace FakeItEasy.Specs;

using FluentAssertions;
using LambdaTale;

#if !LACKS_DEFAULT_INTERFACE_METHODS
public static class DefaultInterfaceMethodSpecs
{
    public interface IHaveDefaultMethod
    {
        int Foo() => 42;
    }

    [Scenario]
    public static void FakeInterfaceWithDefaultMethod(IHaveDefaultMethod fake, int result)
    {
        "Given a faked interface with a default method"
            .x(() => fake = A.Fake<IHaveDefaultMethod>());

        "When the default interface method is called"
            .x(() => result = fake.Foo());

        "Then it should return the default value"
            .x(() => result.Should().Be(0));
    }

    [Scenario]
    public static void FakeInterfaceWithDefaultMethodWithCallsBaseMethods(IHaveDefaultMethod fake, int result)
    {
        "Given a faked interface with a default method configured to call base methods"
            .x(() => fake = A.Fake<IHaveDefaultMethod>(o => o.CallsBaseMethods()));

        "When the default interface method is called"
            .x(() => result = fake.Foo());

        "Then it should return the value from the default implementation"
            .x(() => result.Should().Be(42));
    }
}
#endif
