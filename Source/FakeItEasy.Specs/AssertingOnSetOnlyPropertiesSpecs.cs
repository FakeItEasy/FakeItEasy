namespace FakeItEasy.Specs
{
    using System.Diagnostics.CodeAnalysis;
    using Machine.Specifications;

    public class when_asserting_on_set_only_properties
    {
        private static ISetOnly setOnly;

        Establish context = () => setOnly = A.Fake<ISetOnly>();

        Because of = () =>
            {
                setOnly.MyProperty = 1;
                setOnly.MyProperty2 = false;
            };

        It should_be_able_to_assert_with_argument_constraint =
            () => A.CallTo(setOnly).Where(x => x.Method.Name == "set_MyProperty")
                .WhenArgumentsMatch(x => x.Get<int>(0) == 1).MustHaveHappened();

        public interface ISetOnly
        {
            [SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly", Justification = "Required for testing.")]
            int MyProperty { set; }

            [SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly", Justification = "Required for testing.")]
            bool MyProperty2 { set; }
        }
    }
}