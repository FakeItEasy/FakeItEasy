namespace FakeItEasy.Specs
{
    using FluentAssertions;
    using Xbehave;
    using Xunit;

    public static class ArgumentMatchingSpecs
    {
        public interface IFoo
        {
            int Bar(int a);

            int Bar(int a, string b);

            int Bar(int a, string b, bool c);

            int Bar(int a, string b, bool c, object d);
        }

        [Scenario]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(10)]
        public static void WithAnyArguments(int argument, IFoo fake, int result)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And a fake method with 1 parameter is configured to return a value when called with any arguments"
                .x(() => A.CallTo(() => fake.Bar(0))
                            .WithAnyArguments()
                            .Returns(42));

            $"When the method is called with argument {argument}"
                .x(() => result = fake.Bar(argument));

            "Then it returns the configured value"
                .x(() => result.Should().Be(42));
        }

        [Scenario]
        public static void WhenArgumentsMatchWith1ArgSuccess(IFoo fake, int result)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And a fake method with 1 parameter is configured to return a value when the arguments match a predicate"
                .x(() => A.CallTo(() => fake.Bar(0))
                            .WhenArgumentsMatch(args => args.Get<int>(0) % 2 == 0)
                            .Returns(42));

            "When the method is called with an argument that satisfies the predicate"
                .x(() => result = fake.Bar(4));

            "Then it returns the configured value"
                .x(() => result.Should().Be(42));
        }

        [Scenario]
        public static void WhenArgumentsMatchWith1ArgFailure(IFoo fake, int result)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And a fake method with 1 parameter is configured to return a value when the arguments match a predicate"
                .x(() => A.CallTo(() => fake.Bar(0))
                            .WhenArgumentsMatch(args => args.Get<int>(0) % 2 == 0)
                            .Returns(42));

            "When the method is called with an argument that doesn't satisfy the predicate"
                .x(() => result = fake.Bar(3));

            "Then it returns the default value"
                .x(() => result.Should().Be(0));
        }

        [Scenario]
        public static void WhenArgumentsMatchWith2ArgsSuccess(IFoo fake, int result)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And a fake method with 2 parameters is configured to return a value when the arguments match a predicate"
                .x(() => A.CallTo(() => fake.Bar(0, null))
                            .WhenArgumentsMatch(args => args.Get<int>(0) % 2 == 0 && args.Get<string>(1).Length < 3)
                            .Returns(42));

            "When the method is called with arguments that satisfy the predicate"
                .x(() => result = fake.Bar(4, "x"));

            "Then it returns the configured value"
                .x(() => result.Should().Be(42));
        }

        [Scenario]
        public static void WhenArgumentsMatchWith2ArgsFailure(IFoo fake, int result)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And a fake method with 2 parameters is configured to return a value when the arguments match a predicate"
                .x(() => A.CallTo(() => fake.Bar(0, null))
                            .WhenArgumentsMatch(args => args.Get<int>(0) % 2 == 0 && args.Get<string>(1).Length < 3)
                            .Returns(42));

            "When the method is called with arguments that don't satisfy the predicate"
                .x(() => result = fake.Bar(3, "hello"));

            "Then it returns the default value"
                .x(() => result.Should().Be(0));
        }
    }
}
