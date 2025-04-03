namespace FakeItEasy.Specs;

using System;
using FakeItEasy.Configuration;
using FakeItEasy.Tests.TestHelpers;
using FluentAssertions;
using LambdaTale;
using Xunit;

#if !LACKS_DEFAULT_INTERFACE_METHODS
public static class DefaultInterfaceMethodSpecs
{
    public interface IHaveDefaultMethods
    {
        int Foo() => 42;

        sealed int Bar() => 123;
    }

    [Scenario]
    public static void FakeInterfaceWithDefaultMethod(IHaveDefaultMethods fake, int result)
    {
        "Given a faked interface with a default method"
            .x(() => fake = A.Fake<IHaveDefaultMethods>());

        "When the default interface method is called"
            .x(() => result = fake.Foo());

        "Then it should return the default value"
            .x(() => result.Should().Be(0));
    }

    [Scenario]
    public static void FakeInterfaceWithDefaultMethodWithCallsBaseMethods(IHaveDefaultMethods fake, int result)
    {
        "Given a faked interface with a default method configured to call base methods"
            .x(() => fake = A.Fake<IHaveDefaultMethods>(o => o.CallsBaseMethods()));

        "When the default interface method is called"
            .x(() => result = fake.Foo());

        "Then it should return the value from the default implementation"
            .x(() => result.Should().Be(42));
    }

    [Scenario]
    public static void FakeInterfaceWithDefaultMethodOverrideBehavior(IHaveDefaultMethods fake, int result)
    {
        "Given a faked interface with a default method"
            .x(() => fake = A.Fake<IHaveDefaultMethods>());

        "When the default method is configured to return something else"
            .x(() => A.CallTo(() => fake.Foo()).Returns(99));

        "And the default interface method is called"
            .x(() => result = fake.Foo());

        "Then it should return the configured value"
            .x(() => result.Should().Be(99));
    }

    [Scenario]
    public static void FakeInterfaceWithSealedDefaultMethod(IHaveDefaultMethods fake, int result)
    {
        "Given a faked interface with a sealed default method"
            .x(() => fake = A.Fake<IHaveDefaultMethods>());

        "When the default interface method is called"
            .x(() => result = fake.Bar());

        "Then it should return the value from the default implementation"
            .x(() => result.Should().Be(123));
    }

    [Scenario]
    public static void FakeInterfaceWithSealedDefaultMethodOverrideBehavior(IHaveDefaultMethods fake, Exception? exception)
    {
        "Given a faked interface with a sealed default method"
            .x(() => fake = A.Fake<IHaveDefaultMethods>());

        "When the sealed default method is configured to return something else"
            .x(() => exception = Record.Exception(() => A.CallTo(() => fake.Bar()).Returns(99)));

        "Then it throws a fake configuration exception"
            .x(() => exception.Should().BeAnExceptionOfType<FakeConfigurationException>()
                .And.Message.Should().Contain("Non-virtual or sealed members can not be intercepted. Only interface members and non-sealed virtual, overriding, and abstract members can be intercepted."));
    }
}
#endif
