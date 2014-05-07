namespace FakeItEasy.Specs
{
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

        private Because of = () => subject[17, true] = new[] { "hippo", "lemur" };

        private It should_return_the_value_to_the_getter_with_same_indices =
            () => subject[17, true].Should().BeEquivalentTo("hippo", "lemur");

        private It should_return_the_same_instance_each_time_the_getter_is_called_with_those_indices =
            () => ReferenceEquals(subject[17, true], subject[17, true])
                .Should().BeTrue("property getters should return the same object every time");

        private It should_return_the_same_instance_each_time_the_getter_is_called_with_other_indices =
            () => ReferenceEquals(subject[18, false], subject[18, false])
                .Should().BeTrue("property getters should return the same object every time");

        public interface IFoo
        {
            [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Required for testing.")]
            [SuppressMessage("Microsoft.Design", "CA1023:IndexersShouldNotBeMultidimensional", Justification = "Required for testing.")]
            string[] this[int index1, bool index2] { get; set; }
        }
    }
}
