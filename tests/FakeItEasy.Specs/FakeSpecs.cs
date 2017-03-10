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

            void AnotherMethod(string text);
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
                    fake.AnotherMethod("houseboat");
                });

            "When I use the static Fake class to get the calls made on the Fake"
                .x(() => completedCalls = Fake.GetCalls(fake));

            "Then the calls made to the Fake will be returned"
                .x(() =>
                    completedCalls.Select(call => call.Method.Name)
                        .Should()
                        .Equal("AMethod", "AnotherMethod", "AnotherMethod"));
        }

        [Scenario]
        public static void MatchingCallsWithNoMatches(
            IFoo fake,
            IEnumerable<ICompletedFakeObjectCall> completedCalls,
            IEnumerable<ICompletedFakeObjectCall> matchedCalls)
        {
            "Given a Fake"
                .x(() => fake = A.Fake<IFoo>());

            "And I make several calls to the Fake"
                .x(() =>
                {
                    fake.AMethod();
                    fake.AnotherMethod("houseboat");
                });

            "And I use the static Fake class to get the calls made on the Fake"
                .x(() => completedCalls = Fake.GetCalls(fake));

            "When I use Matching to find calls to a method with no matches"
                .x(() => matchedCalls = completedCalls.Matching<IFoo>(c => c.AnotherMethod("hovercraft")));

            "Then it finds no calls"
                .x(() => matchedCalls.Should().BeEmpty());
        }

        [Scenario]
        public static void MatchingCallsWithMatches(
            IFoo fake,
            IEnumerable<ICompletedFakeObjectCall> completedCalls,
            IEnumerable<ICompletedFakeObjectCall> matchedCalls)
        {
            "Given a Fake"
                .x(() => fake = A.Fake<IFoo>());

            "And I make several calls to the Fake"
                .x(() =>
                {
                    fake.AMethod();
                    fake.AnotherMethod();
                    fake.AnotherMethod("houseboat");
                });

            "And I use the static Fake class to get the calls made on the Fake"
                .x(() => completedCalls = Fake.GetCalls(fake));

            "When I use Matching to find calls to a method with a match"
                .x(() => matchedCalls = completedCalls.Matching<IFoo>(c => c.AnotherMethod("houseboat")));

            "Then it finds the matching call"
                .x(() => matchedCalls.Select(c => c.Method.Name).Should().Equal("AnotherMethod"));
        }

        [Scenario]
        public static void ClearRecordedCalls(IFoo fake)
        {
            "Given a Fake"
                .x(() => fake = A.Fake<IFoo>());

            "And I make several calls to the Fake"
                .x(() =>
                {
                    fake.AMethod();
                    fake.AnotherMethod();
                    fake.AnotherMethod("houseboat");
                });

            "When I clear the recorded calls"
                .x(() => Fake.ClearRecordedCalls(fake));

            "Then the recorded call list is empty"
                .x(() => Fake.GetCalls(fake).Should().BeEmpty());
        }
    }
}
