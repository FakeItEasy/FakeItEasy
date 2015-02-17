namespace FakeItEasy.Specs
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using FluentAssertions;
    using Machine.Specifications;

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

    public class when_setting_the_value_of_an_indexed_property
    {
        private static IHaveInterestingProperties subject;

        Establish context = () =>
        {
            subject = A.Fake<IHaveInterestingProperties>();
        };

        Because of = () => subject[17, true] = new List<string> { "hippo", "lemur" };

        It should_return_the_value_to_the_getter_with_same_indexes =
            () => subject[17, true].Should().BeEquivalentTo("hippo", "lemur");

        It should_return_the_same_instance_each_time_the_getter_is_called_with_those_indexes =
            () =>
            {
                var firstResult = subject[17, true];
                var secondResult = subject[17, true];
                ReferenceEquals(firstResult, secondResult)
                    .Should().BeTrue("property getters should return the same object every time");
            };

        It should_return_the_default_value_to_getters_with_different_indexes =
            () =>
            {
                var result = subject[-183, true];
                result.Should().BeEmpty();
            };

        It should_return_the_same_instance_each_time_the_getter_is_called_with_other_indexes =
            () =>
            {
                var firstResult = subject[18, false];
                var secondResult = subject[18, false];
                ReferenceEquals(firstResult, secondResult)
                    .Should().BeTrue("property getters should return the same object every time");
            };
    }

    public class when_setting_the_value_of_an_indexed_property_for_different_indexes
    {
        private static IHaveInterestingProperties subject;

        Establish context = () =>
        {
            subject = A.Fake<IHaveInterestingProperties>();
        };

        Because of = () =>
        {
            subject[17, true] = new List<string> { "hippo", "lemur" };
            subject[17, false] = new List<string> { "corgi", "chicken" };
        };

        It should_return_the_correct_value_for_the_first_indexes =
            () => subject[17, true].Should().BeEquivalentTo("hippo", "lemur");

        It should_return_the_correct_value_for_the_second_indexes =
            () => subject[17, false].Should().BeEquivalentTo("corgi", "chicken");
    }

    public class when_getting_the_value_of_an_unconfigured_fakeable_property
    {
        private static IHaveInterestingProperties subject;
        private static IHaveInterestingProperties firstValue;
        private static IHaveInterestingProperties secondValue;

        Establish context = () =>
        {
            subject = A.Fake<IHaveInterestingProperties>();
        };

        Because of = () =>
        {
            firstValue = subject.FakeableProperty;
            secondValue = subject.FakeableProperty;
        };

        It should_not_return_null =
            () => firstValue.Should().NotBeNull();

        It should_return_the_same_instance_on_a_subsequent_get =
            () => secondValue.Should().BeSameAs(firstValue);
    }

    public class when_getting_the_value_of_an_unconfigured_unfakeable_property
    {
        private static IHaveInterestingProperties subject;
        private static UnfakeableClass firstValue;
        private static UnfakeableClass secondValue;

        Establish context = () =>
        {
            subject = A.Fake<IHaveInterestingProperties>();
        };

        Because of = () =>
        {
            firstValue = subject.UnfakeableProperty;
            secondValue = subject.UnfakeableProperty;
        };

        It should_not_return_null_if_dummy_can_be_made =
            () => firstValue.Should().NotBeNull();

        It should_return_the_same_instance_on_a_subsequent_get =
            () => secondValue.Should().BeSameAs(firstValue);
    }
}
