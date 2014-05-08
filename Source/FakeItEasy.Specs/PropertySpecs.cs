namespace FakeItEasy.Specs
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using FluentAssertions;
    using Machine.Specifications;

    public class when_setting_the_value_of_an_indexed_property
    {
        private static IFoo subject;

        private Establish context = () =>
        {
            subject = A.Fake<IFoo>();
        };

        private Because of = () => subject[17, true] = new List<string> { "hippo", "lemur" };

        private It should_return_the_value_to_the_getter_with_same_indices =
            () => subject[17, true].Should().BeEquivalentTo("hippo", "lemur");

        private It should_return_the_same_instance_each_time_the_getter_is_called_with_those_indices =
            () =>
            {
                var firstResult = subject[17, true];
                var secondResult = subject[17, true];
                ReferenceEquals(firstResult, secondResult)
                    .Should().BeTrue("property getters should return the same object every time");
            };

        private It should_return_the_default_value_to_getters_with_different_indices =
            () =>
            {
                var result = subject[-183, true];
                result.Should().BeEmpty();
            };

        private It should_return_the_same_instance_each_time_the_getter_is_called_with_other_indices =
            () =>
            {
                var firstResult = subject[18, false];
                var secondResult = subject[18, false];
                ReferenceEquals(firstResult, secondResult)
                    .Should().BeTrue("property getters should return the same object every time");
            };

        public interface IFoo
        {
            [SuppressMessage("Microsoft.Design",
                "CA1023:IndexersShouldNotBeMultidimensional", Justification = "Required for testing.")]
            List<string> this[int index1, bool index2] { get; set; }
        }
    }
}
