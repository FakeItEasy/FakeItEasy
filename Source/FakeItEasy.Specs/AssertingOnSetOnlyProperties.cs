namespace FakeItEasy.Specs
{
    using System.Diagnostics.CodeAnalysis;
    using Xbehave;

    public  class AssertingOnSetOnlyProperties
    {
        [Scenario]
        public void when_asserting_on_set_only_properties(
            ISetOnly setOnly)
        {
            "establish"._(() =>
            {
                setOnly = A.Fake<ISetOnly>();
            });

            "when assertion on set only properties"._(() =>
            { 
                setOnly.MyProperty = 1;
                setOnly.MyProperty2 = false;
            });

            "it should be able to assert with argument constraint"._(() =>
            {
                A.CallTo(setOnly).Where(x => x.Method.Name == "set_MyProperty")
                    .WhenArgumentsMatch(x => x.Get<int>(0) == 1).MustHaveHappened();
            });
        }

        public interface ISetOnly
        {
            [SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly", Justification = "Required for testing.")]
            int MyProperty { set; }

            [SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly", Justification = "Required for testing.")]
            bool MyProperty2 { set; }
        }
    }
}