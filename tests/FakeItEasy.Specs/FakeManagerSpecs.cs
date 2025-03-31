namespace FakeItEasy.Specs
{
    using System.Collections.Generic;
    using FakeItEasy.Core;
    using FluentAssertions;
    using LambdaTale;

    public static class FakeManagerSpecs
    {
        public interface IFoo
        {
            void AMethod();

            void AnotherMethod();
        }

        [Scenario]
        public static void CanEnumerateRulesWhileConfiguringFake(
            IFoo fake,
            FakeManager manager,
            IEnumerable<IFakeObjectCallRule> rules,
            IEnumerator<IFakeObjectCallRule> enumerator)
        {
            "Given a Fake"
                .x(() => fake = A.Fake<IFoo>());

            "And I configure a method call on the Fake"
                .x(() => A.CallTo(() => fake.AMethod()).DoesNothing());

            "And I configure another method call on the Fake"
                .x(() => A.CallTo(() => fake.AMethod()).DoesNothing());

            "And I get the Fake's manager"
                .x(() => manager = Fake.GetFakeManager(fake));

            "When I fetch the rules from the manager"
                .x(() => rules = manager.Rules);

            "And I get the rules' enumerator"
                .x(() => enumerator = rules.GetEnumerator());

            "And I step over the first rule"
                .x(() => enumerator.MoveNext());

            "And I configure the Fake with another rule"
                .x(() => A.CallTo(() => fake.AnotherMethod()).DoesNothing());

            "And I step over the next rule"
                .x(() => enumerator.MoveNext());

            "Then I come to the end of the list of rules"
                .x(() => enumerator.MoveNext().Should().BeFalse());
        }
    }
}
