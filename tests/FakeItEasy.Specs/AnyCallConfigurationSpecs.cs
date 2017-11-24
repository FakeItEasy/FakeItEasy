namespace FakeItEasy.Specs
{
    using System;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xbehave;
    using Xunit;

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

            "And the fake is configured to set a flag for any non-void return type"
                .x(() => A.CallTo(fake).WithNonVoidReturnType().Invokes(() => methodWasIntercepted = true));

            "When the void method is called"
                .x(() => fake.Baz());

            "Then the flag is not set"
                .x(() => methodWasIntercepted.Should().BeFalse());
        }

        [Scenario]
        public static void WithNonVoidReturnTypeDescription(
            IFoo fake,
            Exception exception)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "When an assertion is made that a non-void method was called"
                .x(() => exception = Record.Exception(() => A.CallTo(fake).WithNonVoidReturnType().MustHaveHappened()));

            "Then an exception is thrown"
                .x(() => exception.Should().BeAnExceptionOfType<ExpectationException>()
                .WithMessage("*Any call with non-void return type to the fake object.*"));
        }

        [Scenario]
        public static void WithVoidReturnType(IFoo fake, bool methodWasIntercepted)
        {
            "Given a fake with a void method"
                .x(() => fake = A.Fake<IFoo>());

            "And the fake is configured to set a flag for any void method"
                .x(() => A.CallTo(fake).WithVoidReturnType().Invokes(() => methodWasIntercepted = true));

            "When a void method is called on the fake"
                .x(() => fake.Baz());

            "Then the flag is set"
                .x(() => methodWasIntercepted.Should().BeTrue());
        }

        [Scenario]
        public static void WithVoidReturnTypeAndNonVoidMethod(
            IFoo fake,
            bool methodWasIntercepted)
        {
            "Given a fake with void and non-void methods"
                .x(() => fake = A.Fake<IFoo>());

            "And the fake is configured to set a flag for any void method"
                .x(() => A.CallTo(fake).WithVoidReturnType().Invokes(() => methodWasIntercepted = true));

            "When a non-void method is called"
                .x(() => fake.Bar<string>());

            "Then the flag is not set"
                .x(() => methodWasIntercepted.Should().BeFalse());
        }

        [Scenario]
        public static void WithVoidReturnTypeDescription(
            IFoo fake,
            Exception exception)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "When an assertion is made that a void method was called"
                .x(() => exception = Record.Exception(() => A.CallTo(fake).WithVoidReturnType().MustHaveHappened()));

            "Then an exception is thrown"
                .x(() => exception.Should().BeAnExceptionOfType<ExpectationException>()
                .WithMessage("*Any call with void return type to the fake object.*"));
        }

        [Scenario]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = nameof(IFoo), Justification = "It's an identifier")]
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

        [Scenario]
        public static void WithReturnTypeDescription(
            IFoo fake,
            Exception exception)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "When an assertion is made that a method with a particular return type was called"
                .x(() => exception = Record.Exception(() => A.CallTo(fake).WithReturnType<Guid>().MustHaveHappened()));

            "Then an exception is thrown"
                .x(() => exception.Should().BeAnExceptionOfType<ExpectationException>()
                .WithMessage("*Any call with return type System.Guid to the fake object.*"));
        }

        [Scenario]
        public static void AnyCallDescription(
            IFoo fake,
            Exception exception)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "When an assertion is made that any method was called"
                .x(() => exception = Record.Exception(() => A.CallTo(fake).MustHaveHappened()));

            "Then an exception is thrown"
                .x(() => exception.Should().BeAnExceptionOfType<ExpectationException>()
                .WithMessage("*Any call made to the fake object.*"));
        }
    }
}
