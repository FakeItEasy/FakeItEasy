namespace FakeItEasy.Specs
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using FluentAssertions;
    using Xbehave;

    public static class UnconfiguredPropertySpecs
    {
        public interface IHaveInterestingProperties
        {
            IHaveInterestingProperties FakeableProperty { get; }

            UnfakeableClass UnfakeableProperty { get; }

            [SuppressMessage("Microsoft.Design", "CA1023:IndexersShouldNotBeMultidimensional", Justification = "Required for testing.")]
            [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Required for testing.")]
            List<string> this[int index1, bool index2] { get; set; }
        }

        [Scenario]
        [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Required for testing.")]
        public static void SettingIndexedProperty(
            IHaveInterestingProperties subject,
            List<string> firstGetResultForOriginalIndexes,
            List<string> firstGetResultForDifferentIndexes)
        {
            "Given a Fake with an indexed property"
                .x(() => subject = A.Fake<IHaveInterestingProperties>());

            "When I set the value of the property"
                .x(() => subject[17, true] = new List<string> { "hippo", "lemur" });

            "And I call the getter with the original indexes"
                .x(() => firstGetResultForOriginalIndexes = subject[17, true]);

            "And I call the getter with different indexes"
                .x(() => firstGetResultForDifferentIndexes = subject[-183, true]);

            "Then the property returns the supplied value when called using the original indexes"
                .x(() => firstGetResultForOriginalIndexes.Should().BeEquivalentTo("hippo", "lemur"));

            "And the property returns the same instance when called again with those indexes"
                .x(() => subject[17, true].Should()
                    .BeSameAs(firstGetResultForOriginalIndexes, "property getters should return the same object every time"));

            "And the property returns the default value when called using the different indexes"
                .x(() => firstGetResultForDifferentIndexes.Should().BeEmpty());

            "And the property returns the same instance when called again with those indexes"
                .x(() => subject[-183, true].Should().BeSameAs(firstGetResultForDifferentIndexes));
        }

        [Scenario]
        [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Required for testing.")]
        public static void SettingIndexedPropertyForDifferentIndexes(
            IHaveInterestingProperties subject,
            List<string> getResultForFirstIndexes,
            List<string> getResultForSecondIndexes)
        {
            "Given a Fake with an indexed property"
                .x(() => subject = A.Fake<IHaveInterestingProperties>());

            "When I set the value of the property"
                .x(() => subject[17, true] = new List<string> { "hippo", "lemur" });

            "And I set the value of the property using different indexes"
                .x(() => subject[17, false] = new List<string> { "corgi", "chicken" });

            "And I call the getter with the original indexes"
                .x(() => getResultForFirstIndexes = subject[17, true]);

            "And I call the getter with the second indexes"
                .x(() => getResultForSecondIndexes = subject[17, false]);

            "Then the property returns the correct value when called with the original indexes"
                .x(() => getResultForFirstIndexes.Should().BeEquivalentTo("hippo", "lemur"));

            "And the property returns the correct value when called with the second indexes"
                .x(() => getResultForSecondIndexes.Should().BeEquivalentTo("corgi", "chicken"));
        }

        [Scenario]
        public static void GettingUnconfiguredFakeableProperty(
            IHaveInterestingProperties subject,
            IHaveInterestingProperties firstGetResult,
            IHaveInterestingProperties secondGetResult)
        {
            "Given a Fake with a property"
                .x(() => subject = A.Fake<IHaveInterestingProperties>());

            "When I get the value of the property"
                .x(() => firstGetResult = subject.FakeableProperty);

            "And I get the value of the property again"
                .x(() => secondGetResult = subject.FakeableProperty);

            "Then the property does not return null the first time"
                .x(() => firstGetResult.Should().NotBeNull());

            "And the property returns the same instance when called again"
                .x(() => secondGetResult.Should().BeSameAs(firstGetResult));
        }

        [Scenario]
        public static void GettingUnconfiguredPropertyOfUnfakeableType(
            IHaveInterestingProperties subject,
            UnfakeableClass firstGetResult,
            UnfakeableClass secondGetResult)
        {
            "Given a Fake with a property whose type is unfakeable"
                .x(() => subject = A.Fake<IHaveInterestingProperties>());

            "And the property type can be made into a Dummy"
                .x(() => { }); // see UnfakeableClass

            "When I get the value of the property"
                .x(() => firstGetResult = subject.UnfakeableProperty);

            "And I get the value of the property again"
                .x(() => secondGetResult = subject.UnfakeableProperty);

            "Then the property does not return null if a Dummy can be made"
                .x(() => firstGetResult.Should().NotBeNull());

            "And the property returns the same instance when called again"
                .x(() => secondGetResult.Should().BeSameAs(firstGetResult));
        }

        public sealed class UnfakeableClass
        {
            private static int nextId;
            private readonly int id;

            public UnfakeableClass()
            {
                this.id = ++nextId;
            }

            public override string ToString()
            {
                return string.Concat("UnfakeableClass ", this.id);
            }
        }
    }
}
