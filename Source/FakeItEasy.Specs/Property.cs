namespace FakeItEasy.Specs
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using FluentAssertions;
    using Xbehave;

    public interface IHaveInterestingProperties
    {
        IHaveInterestingProperties FakeableProperty { get; }

        UnfakeableClass UnfakeableProperty { get; }

        [SuppressMessage("Microsoft.Design",
            "CA1023:IndexersShouldNotBeMultidimensional", Justification = "Required for testing.")]
        [SuppressMessage("Microsoft.Design",
            "CA1002:DoNotExposeGenericLists", Justification = "Required for testing.")]
        List<string> this[int index1, bool index2] { get; set; }
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

    public class Property
    {
        [Scenario]
        public void when_setting_the_value_of_an_indexed_property(
            IHaveInterestingProperties subject)
        {
            "establish"
                .x(() => subject = A.Fake<IHaveInterestingProperties>());

            "when setting the value of an indexed property"
                .x(() => subject[17, true] = new List<string> { "hippo", "lemur" });

            "it should return the value to the getter with same indexes"
                .x(() => subject[17, true].Should().BeEquivalentTo("hippo", "lemur"));

            "it should return the same instance each time the getter is called with those indexes"
                .x(() =>
                    {
                        var firstResult = subject[17, true];
                        var secondResult = subject[17, true];
                        ReferenceEquals(firstResult, secondResult)
                            .Should().BeTrue("property getters should return the same object every time");
                    });

            "it should return the default value to getters with different indexes"
                .x(() =>
                    {
                        var result = subject[-183, true];
                        result.Should().BeEmpty();
                    });

            "it should return the same instance each time the getter is called with other indexes"
                .x(() =>
                    {
                        var firstResult = subject[18, false];
                        var secondResult = subject[18, false];
                        ReferenceEquals(firstResult, secondResult)
                            .Should().BeTrue("property getters should return the same object every time");
                    });
        }

        [Scenario]
        public void when_setting_the_value_of_an_indexed_property_for_different_indexes(
            IHaveInterestingProperties subject)
        {
            "establish"
                .x(() => subject = A.Fake<IHaveInterestingProperties>());

            "when setting the value of an indexed property for different indexes"
                .x(() =>
                    {
                        subject[17, true] = new List<string> { "hippo", "lemur" };
                        subject[17, false] = new List<string> { "corgi", "chicken" };
                    });

            "it should return the correct value for the first indexes"
                .x(() => subject[17, true].Should().BeEquivalentTo("hippo", "lemur"));

            "it should return the correct value for the second indexes"
                .x(() => subject[17, false].Should().BeEquivalentTo("corgi", "chicken"));
        }

        [Scenario]
        public void when_getting_the_value_of_an_unconfigured_fakeable_property(
            IHaveInterestingProperties subject, 
            IHaveInterestingProperties firstValue, 
            IHaveInterestingProperties secondValue)
        {
            "establish"
                .x(() => subject = A.Fake<IHaveInterestingProperties>());

            "when getting the value of an unconfigured fakeable property"
                .x(() =>
                    {
                        firstValue = subject.FakeableProperty;
                        secondValue = subject.FakeableProperty;
                    });

            "it should not return null"
                .x(() => firstValue.Should().NotBeNull());

            "it should return the same instance on a subsequent get"
                .x(() => secondValue.Should().BeSameAs(firstValue));
        }

        [Scenario]
        public void when_getting_the_value_of_an_unconfigured_unfakeable_property(
            IHaveInterestingProperties subject, 
            UnfakeableClass firstValue, 
            UnfakeableClass secondValue)
        {
            "establish"
                .x(() => subject = A.Fake<IHaveInterestingProperties>());

            "when getting the value of an unconfigured unfakeable property"
                .x(() =>
                    {
                        firstValue = subject.UnfakeableProperty;
                        secondValue = subject.UnfakeableProperty;
                    });

            "it should not return null if dummy can be made"
                .x(() => firstValue.Should().NotBeNull());

            "it should return the same instance on a subsequent get"
                .x(() => secondValue.Should().BeSameAs(firstValue));
        }
    }
}
