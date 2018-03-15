namespace FakeItEasy.Specs
{
    using System;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xbehave;
    using Xunit;

    /// <summary>
    /// Characterization tests, representing the current capabilities of the code, not the desired state.
    /// If these tests start failing, update them and fix the "What can be faked" documentation page.
    /// </summary>
    public static class RefReturnSpecs
    {
        public interface IRefReturn
        {
            ref int RefReturn();
        }

        [Scenario]
        public static void UnconfiguredInterfaceWithRefReturn(IRefReturn fake, int result, Exception exception)
        {
            "Given a fake with a method that returns a 'ref' value"
                .x(() => fake = A.Fake<IRefReturn>());

            "When I call the method"
                .x(() => exception = Record.Exception(() => result = fake.RefReturn()));

            "Then it throws"
                .x(() => exception.Should().BeAnExceptionOfType<NullReferenceException>());
        }

        [Scenario]
        public static void ConfiguredInterfaceWithRefReturn(IRefReturn fake, int result, Exception exception)
        {
            "Given a fake with a method that returns a 'ref' value"
                .x(() => fake = A.Fake<IRefReturn>());

            // Note: can't call a ref returning method in an expression
            "And I configure the method to return a given value"
                .x(() => A.CallTo(fake)
                    .Where(call => call.Method.Name == "RefReturn")
                    .WithNonVoidReturnType()
                    .Returns(42));

            "When I call the method"
                .x(() => exception = Record.Exception(() => result = fake.RefReturn()));

            "Then it throws"
                .x(() => exception.Should().BeAnExceptionOfType<NullReferenceException>());
        }
    }
}
