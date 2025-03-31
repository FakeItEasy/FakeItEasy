namespace FakeItEasy.Specs
{
    using FluentAssertions;
    using LambdaTale;

    public static class AnArgumentConstraintSpecs
    {
        public interface IFoo
        {
            int Bar(int x);

            int Bar(Apple apple);
        }

        [Scenario]
        public static void AnIntIgnored(IFoo fake, int result)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And a method of the fake configured with the An<int>.Ignored argument constraint"
                .x(() => A.CallTo(() => fake.Bar(An<int>.Ignored)).Returns(42));

            "When the configured method is called"
                .x(() => result = fake.Bar(123));

            "Then it returns the configured value"
                .x(() => result.Should().Be(42));
        }

        [Scenario]
        public static void AnIntUnderscore(IFoo fake, int result)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And a method of the fake configured with the An<int>._ argument constraint"
                .x(() => A.CallTo(() => fake.Bar(An<int>._)).Returns(42));

            "When the configured method is called"
                .x(() => result = fake.Bar(123));

            "Then it returns the configured value"
                .x(() => result.Should().Be(42));
        }

        [Scenario]
        public static void AnIntThatWithMatchingArgument(IFoo fake, int result)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And a method of the fake configured with the An<int>.That argument constraint"
                .x(() => A.CallTo(() => fake.Bar(An<int>.That.Matches(i => i % 2 == 1))).Returns(42));

            "When the configured method is called with an argument that matches the constraint"
                .x(() => result = fake.Bar(123));

            "Then it returns the configured value"
                .x(() => result.Should().Be(42));
        }

        [Scenario]
        public static void AnIntThatWithNonMatchingArgument(IFoo fake, int result)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And a method of the fake configured with the An<int>.That argument constraint"
                .x(() => A.CallTo(() => fake.Bar(An<int>.That.Matches(i => i % 2 == 1))).Returns(42));

            "When the configured method is called with an argument that doesn't match the constraint"
                .x(() => result = fake.Bar(12));

            "Then it returns the default value"
                .x(() => result.Should().Be(0));
        }

        [Scenario]
        public static void AnAppleIgnored(IFoo fake, int result)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And a method of the fake configured with the An<Apple>.Ignored argument constraint"
                .x(() => A.CallTo(() => fake.Bar(An<Apple>.Ignored)).Returns(42));

            "When the configured method is called"
                .x(() => result = fake.Bar(new Apple("Red")));

            "Then it returns the configured value"
                .x(() => result.Should().Be(42));
        }

        [Scenario]
        public static void AnAppleUnderscore(IFoo fake, int result)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And a method of the fake configured with the An<Apple>._ argument constraint"
                .x(() => A.CallTo(() => fake.Bar(An<Apple>._)).Returns(42));

            "When the configured method is called"
                .x(() => result = fake.Bar(new Apple("Red")));

            "Then it returns the configured value"
                .x(() => result.Should().Be(42));
        }

        [Scenario]
        public static void AnAppleThatWithMatchingArgument(IFoo fake, int result)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And a method of the fake configured with the An<Apple>.That argument constraint"
                .x(() => A.CallTo(() => fake.Bar(An<Apple>.That.Matches(a => a.Color == "Red"))).Returns(42));

            "When the configured method is called with an argument that matches the constraint"
                .x(() => result = fake.Bar(new Apple("Red")));

            "Then it returns the configured value"
                .x(() => result.Should().Be(42));
        }

        [Scenario]
        public static void AnAppleThatNonWithMatchingArgument(IFoo fake, int result)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And a method of the fake configured with the An<Apple>.That argument constraint"
                .x(() => A.CallTo(() => fake.Bar(An<Apple>.That.Matches(a => a.Color == "Red"))).Returns(42));

            "When the configured method is called with an argument that doesn't match the constraint"
                .x(() => result = fake.Bar(new Apple("Green")));

            "Then it returns the default value"
                .x(() => result.Should().Be(0));
        }

        public class Apple
        {
            public Apple(string color)
            {
                this.Color = color;
            }

            public string Color { get; }
        }
    }
}
