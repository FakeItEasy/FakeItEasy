namespace FakeItEasy.Specs
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using FluentAssertions;
    using Machine.Specifications;

    public interface IHaveAnInterestingProperty
    {
        [SuppressMessage("Microsoft.Design",
            "CA1023:IndexersShouldNotBeMultidimensional", Justification = "Required for testing.")]
        [SuppressMessage("Microsoft.Design",
            "CA1002:DoNotExposeGenericLists", Justification = "Required for testing.")]
        List<string> this[int index1, bool index2] { get; set; }
    }

    public class when_setting_the_value_of_an_indexed_property
    {
        private static IHaveAnInterestingProperty subject;

        private Establish context = () =>
        {
            subject = A.Fake<IHaveAnInterestingProperty>();
        };

        private Because of = () => subject[17, true] = new List<string> { "hippo", "lemur" };

        private It should_return_the_value_to_the_getter_with_same_indexes =
            () => subject[17, true].Should().BeEquivalentTo("hippo", "lemur");

        private It should_return_the_same_instance_each_time_the_getter_is_called_with_those_indexes =
            () =>
            {
                var firstResult = subject[17, true];
                var secondResult = subject[17, true];
                ReferenceEquals(firstResult, secondResult)
                    .Should().BeTrue("property getters should return the same object every time");
            };

        private It should_return_the_default_value_to_getters_with_different_indexes =
            () =>
            {
                var result = subject[-183, true];
                result.Should().BeEmpty();
            };

        private It should_return_the_same_instance_each_time_the_getter_is_called_with_other_indexes =
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
        private static IHaveAnInterestingProperty subject;

        private Establish context = () =>
        {
            subject = A.Fake<IHaveAnInterestingProperty>();
        };

        private Because of = () =>
        {
            subject[17, true] = new List<string> { "hippo", "lemur" };
            subject[17, false] = new List<string> { "corgi", "chicken" };
        };

        private It should_return_the_correct_value_for_the_first_indexes =
            () => subject[17, true].Should().BeEquivalentTo("hippo", "lemur");

        private It should_return_the_correct_value_for_the_second_indexes =
            () => subject[17, false].Should().BeEquivalentTo("corgi", "chicken");
    }
}
