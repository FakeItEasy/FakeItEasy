namespace FakeItEasy.Specs
{
    using FakeItEasy.Core;
    using FluentAssertions;
    using Xbehave;

    public static class ObjectMembersSpecs
    {
        [Scenario]
        public static void DefaultEqualsWithSelf(IFoo fake, bool equals)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "When Equals is called on the fake with the fake as the argument"
                .x(() => equals = fake.Equals(fake));

            "Then it returns true"
                .x(() => equals.Should().BeTrue());
        }

        [Scenario]
        public static void DefaultEqualsWithOtherFake(IFoo fake, IFoo otherFake, bool equals)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And another fake of the same type"
                .x(() => otherFake = A.Fake<IFoo>());

            "When Equals is called on the first fake with the other fake as the argument"
                .x(() => equals = fake.Equals(otherFake));

            "Then it returns false"
                .x(() => equals.Should().BeFalse());
        }

        [Scenario]
        public static void DefaultGetHashCode(IFoo fake, FakeManager manager, int fakeHashCode)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And its manager"
                .x(() => manager = Fake.GetFakeManager(fake));

            "When GetHashCode is called on the fake"
                .x(() => fakeHashCode = fake.GetHashCode());

            "Then it returns the manager's hash code"
                .x(() => fakeHashCode.Should().Be(manager.GetHashCode()));
        }

        [Scenario]
        public static void DefaultToString(IFoo fake, string? stringRepresentation)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "When ToString is called on the fake"
                .x(() => stringRepresentation = fake.ToString());

            "Then it should return a string representation of the fake"
                .x(() => stringRepresentation.Should().Be("Faked FakeItEasy.Specs.ObjectMembersSpecs+IFoo"));
        }

        public interface IFoo
        {
            void Bar();
        }
    }
}
