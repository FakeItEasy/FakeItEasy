namespace FakeItEasy.Specs
{
    using FluentAssertions;
    using Xbehave;

    public static class ReadWritePropertySpecs
    {
        public interface IFoo
        {
            int Bar { get; set; }
        }

        [Scenario]
        public static void AssignNonConfiguredProperty(IFoo foo)
        {
            "Given a fake"
                .x(() => foo = A.Fake<IFoo>());

            "When I assign a value to a read-write property"
                .x(() => foo.Bar = 123);

            "Then the property returns the assigned value"
                .x(() => foo.Bar.Should().Be(123));
        }

        [Scenario]
        public static void AssignPropertyWithConfiguredGetter(IFoo foo)
        {
            "Given a fake"
                .x(() => foo = A.Fake<IFoo>());

            "And the getter of a read-write property of the fake is explicitly configured to return a value"
                .x(() => A.CallTo(() => foo.Bar).Returns(42));

            "When I assign another value to that property"
                .x(() => foo.Bar = 123);

            "Then the assignment has no effect and the property returns the explicitly configured value"
                .x(() => foo.Bar.Should().Be(42));
        }

        [Scenario]
        public static void GetPropertyWithConfiguredSetter(IFoo foo, int value)
        {
            "Given a fake"
                .x(() => foo = A.Fake<IFoo>());

            "And the setter of a read-write property of the fake is explicitly configured to do nothing"
                .x(() => A.CallToSet(() => foo.Bar).DoesNothing());

            "When I get the value of that property"
                .x(() => value = foo.Bar);

            "And I assign another value to that property"
                .x(() => foo.Bar = 123);

            "Then the assignment has no effect and the property returns the initial default value"
                .x(() => foo.Bar.Should().Be(value));
        }
    }
}
