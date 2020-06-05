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

        [Scenario]
        public static void OverriddenEqualsWithSelf(Foo fake, bool equals)
        {
            "Given a fake of a type that overrides Equals"
                .x(() => fake = A.Fake<Foo>(o => o.WithArgumentsForConstructor(() => new Foo(1))));

            "When Equals is called on the fake with the fake as the argument"
                .x(() => equals = fake.Equals(fake));

            "Then it returns true"
                .x(() => equals.Should().BeTrue());
        }

        [Scenario]
        public static void OverriddenEqualsWithOtherFake(Foo fake, Foo otherFake, bool equals)
        {
            "Given a fake of a type that overrides Equals"
                .x(() => fake = A.Fake<Foo>(o => o.WithArgumentsForConstructor(() => new Foo(1))));

            "And another fake of the same type"
                .x(() => otherFake = A.Fake<Foo>(o => o.WithArgumentsForConstructor(() => new Foo(2))));

            "When Equals is called on the first fake with the other fake as the argument"
                .x(() => equals = fake.Equals(otherFake));

            "Then it returns false"
                .x(() => equals.Should().BeFalse());
        }

        [Scenario]
        public static void OverriddenGetHashCode(Foo fake, FakeManager manager, int fakeHashCode)
        {
            "Given a fake of a type that overrides GetHashCode"
                .x(() => fake = A.Fake<Foo>(o => o.WithArgumentsForConstructor(() => new Foo(1))));

            "And its manager"
                .x(() => manager = Fake.GetFakeManager(fake));

            "When GetHashCode is called on the fake"
                .x(() => fakeHashCode = fake.GetHashCode());

            "Then it returns the manager's hash code"
                .x(() => fakeHashCode.Should().Be(manager.GetHashCode()));
        }

        [Scenario]
        public static void OverriddenToString(Foo fake, string? stringRepresentation)
        {
            "Given a fake of a type that overrides ToString"
                .x(() => fake = A.Fake<Foo>(o => o.WithArgumentsForConstructor(() => new Foo(1))));

            "When ToString is called on the fake"
                .x(() => stringRepresentation = fake.ToString());

            "Then it should return a string representation of the fake"
                .x(() => stringRepresentation.Should().Be("Faked FakeItEasy.Specs.ObjectMembersSpecs+Foo"));
        }

        public interface IFoo
        {
            void Bar();
        }

        public class Foo
        {
            public Foo(int value)
            {
                this.Value = value;
            }

            public int Value { get; }

            public override bool Equals(object? obj) => obj is Foo other && other.Value == this.Value;

            public override int GetHashCode() => this.Value.GetHashCode();

            public override string ToString() => $"Foo {this.Value}";
        }
    }
}
