namespace FakeItEasy.Specs
{
    using System;
    using FluentAssertions;
    using Xbehave;

    public static class AnyCallConfigurationSpecs
    {
        public interface IFoo
        {
            T Bar<T>() where T : class;

            void Baz();
        }

        [Scenario]
        public static void WithNonVoidReturnType(IFoo fake)
        {
            "Given a fake with a generic method"
                .x(() => fake = A.Fake<IFoo>());

            "When the fake's generic method is configured to return null for any non-void return type"
                .x(() => A.CallTo(fake).Where(call => call.Method.Name == "Bar").WithNonVoidReturnType().Returns(null));

            "Then the configured method returns null when called with generic argument String"
                .x(() => fake.Bar<string>().Should().BeNull());

            "And the configured method returns null when called with generic argument IFoo"
                .x(() => fake.Bar<IFoo>().Should().BeNull());
        }

        [Scenario]
        public static void WithNonVoidReturnTypeAndVoidMethod(
            IFoo fake,
            bool methodWasIntercepted)
        {
            "Given a fake with a void method"
                .x(() => fake = A.Fake<IFoo>());

            "When the fake is configured to set a flag for any non-void return type"
                .x(() => A.CallTo(fake).WithNonVoidReturnType().Invokes(() => methodWasIntercepted = true));

            "And the void method is called"
                .x(() => fake.Baz());

            "Then the flag is not set"
                .x(() => methodWasIntercepted.Should().BeFalse());
        }

        [Scenario]
        public static void WithReturnType(
            IFoo fake,
            string returnValue)
        {
            "Given a fake with a generic method"
                .x(() => fake = A.Fake<IFoo>());

            "When the fake's generic method is configured to return a specific value for a given return type"
                .x(() => A.CallTo(fake).Where(call => call.Method.Name == "Bar").WithReturnType<string>().Returns(returnValue = "hello world"));

            "Then the configured method returns the specified value when called with generic argument String"
                .x(() => fake.Bar<string>().Should().Be(returnValue));

            "And the configured method returns a dummy when called with generic argument IFoo"
                .x(() => fake.Bar<IFoo>().Should().NotBeNull().And.BeAssignableTo<IFoo>());
        }
    }
}
