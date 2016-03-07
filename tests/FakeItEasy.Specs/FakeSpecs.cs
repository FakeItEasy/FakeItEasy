namespace FakeItEasy.Specs
{
    using System.Collections.Generic;
    using System.Linq;
    using FakeItEasy.Core;
    using FluentAssertions;
    using Xbehave;

    public static class FakeSpecs
    {
        public interface IFoo
        {
            void AMethod();

            void AnotherMethod();
        }

        [Scenario]
        public static void NonGenericCallsSuccess(
            IFoo fake,
            IEnumerable<ICompletedFakeObjectCall> completedCalls)
        {
            "Given a Fake"
                .x(() => fake = A.Fake<IFoo>());

            "And I make several calls to the Fake"
                .x(() =>
                {
                    fake.AMethod();
                    fake.AnotherMethod();
                    fake.AnotherMethod();
                });

            "When I use the static Fake class to get the calls made on the Fake"
                .x(() => completedCalls = Fake.GetCalls(fake));

            "Then the calls made to the Fake will be returned"
                .x(() =>
                    completedCalls.Select(call => call.Method.Name)
                        .Should()
                        .Equal("AMethod", "AnotherMethod", "AnotherMethod"));
        }
    }
}
