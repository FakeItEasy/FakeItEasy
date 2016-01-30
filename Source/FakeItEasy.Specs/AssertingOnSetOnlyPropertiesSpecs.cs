namespace FakeItEasy.Specs
{
    using System.Diagnostics.CodeAnalysis;
    using Xbehave;

    public static class AssertingOnSetOnlyPropertiesSpecs
    {
        public interface ISetOnly
        {
            [SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly", Justification = "Required for testing.")]
            int MyProperty { set; }

            [SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly", Justification = "Required for testing.")]
            bool MyProperty2 { set; }
        }

        [Scenario]
        public static void SetOnlyProperties(
            ISetOnly setOnly)
        {
            "establish"
                .x(() => setOnly = A.Fake<ISetOnly>());

            "when assertion on set only properties"
                .x(() =>
                {
                    setOnly.MyProperty = 1;
                    setOnly.MyProperty2 = false;
                });

            "it should be able to assert with argument constraint"
                .x(() => A.CallTo(setOnly).Where(x => x.Method.Name == "set_MyProperty")
                             .WhenArgumentsMatch(x => x.Get<int>(0) == 1).MustHaveHappened());
        }
    }
}
