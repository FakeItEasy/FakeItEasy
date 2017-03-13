namespace FakeItEasy.Specs
{
    using System;
    using FakeItEasy.Configuration;
    using FakeItEasy.Tests.TestHelpers;
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

        [Scenario]
        public static void StronglyTypedWhenArgumentsMatchWith1ArgSuccess(IFoo fake, int result)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And a fake method with 1 parameter is configured to return a value when the arguments match a predicate"
                .x(() => A.CallTo(() => fake.Bar(0))
                            .WhenArgumentsMatch((int a) => a % 2 == 0)
                            .Returns(42));

            "When the method is called with an argument that satisfies the predicate"
                .x(() => result = fake.Bar(4));

            "Then it returns the configured value"
                .x(() => result.Should().Be(42));
        }

        [Scenario]
        public static void StronglyTypedWhenArgumentsMatchWith1ArgFailure(IFoo fake, int result)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And a fake method with 1 parameter is configured to return a value when the arguments match a predicate"
                .x(() => A.CallTo(() => fake.Bar(0))
                            .WhenArgumentsMatch((int a) => a % 2 == 0)
                            .Returns(42));

            "When the method is called with an argument that doesn't satisfy the predicate"
                .x(() => result = fake.Bar(3));

            "Then it returns the default value"
                .x(() => result.Should().Be(0));
        }

        [Scenario]
        public static void StronglyTypedWhenArgumentsMatchWith1ArgWrongSignature(IFoo fake, Exception exception)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And a fake method with 1 parameter is configured with an arguments predicate with an incompatible signature"
                .x(() => exception = Record.Exception(
                        () => A.CallTo(() => fake.Bar(0))
                            .WhenArgumentsMatch((long a) => true)
                            .Returns(42)));

            "When the method is called"
                .x(() => exception = Record.Exception(() => fake.Bar(3)));

            "Then it throws a FakeConfigurationException that describes the problem"
                .x(() => exception.Should().BeAnExceptionOfType<FakeConfigurationException>()
                            .WithMessage("The faked method has the signature (System.Int32), but when arguments match was used with (System.Int64)."));
        }

        [Scenario]
        public static void StronglyTypedWhenArgumentsMatchWith2ArgsSuccess(IFoo fake, int result)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And a fake method with 2 parameters is configured to return a value when the arguments match a predicate"
                .x(() => A.CallTo(() => fake.Bar(0, null))
                            .WhenArgumentsMatch((int a, string b) => a % 2 == 0 && b.Length < 3)
                            .Returns(42));

            "When the method is called with arguments that satisfy the predicate"
                .x(() => result = fake.Bar(4, "x"));

            "Then it returns the configured value"
                .x(() => result.Should().Be(42));
        }

        [Scenario]
        public static void StronglyTypedWhenArgumentsMatchWith2ArgsFailure(IFoo fake, int result)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And a fake method with 2 parameters is configured to return a value when the arguments match a predicate"
                .x(() => A.CallTo(() => fake.Bar(0, null))
                            .WhenArgumentsMatch((int a, string b) => a % 2 == 0 && b.Length < 3)
                            .Returns(42));

            "When the method is called with arguments that don't satisfy the predicate"
                .x(() => result = fake.Bar(3, "hello"));

            "Then it returns the default value"
                .x(() => result.Should().Be(0));
        }

        [Scenario]
        public static void StronglyTypedWhenArgumentsMatchWith2ArgsWrongSignature(IFoo fake, Exception exception)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And a fake method with 2 parameters is configured with an arguments predicate with an incompatible signature"
                .x(() => exception = Record.Exception(
                        () => A.CallTo(() => fake.Bar(0, null))
                            .WhenArgumentsMatch((long a, DateTime b) => true)
                            .Returns(42)));

            "When the method is called"
                .x(() => exception = Record.Exception(() => fake.Bar(3, "hello")));

            "Then it throws a FakeConfigurationException that describes the problem"
                .x(() => exception.Should().BeAnExceptionOfType<FakeConfigurationException>()
                            .WithMessage("The faked method has the signature (System.Int32, System.String), but when arguments match was used with (System.Int64, System.DateTime)."));
        }

        [Scenario]
        public static void StronglyTypedWhenArgumentsMatchWith3ArgsSuccess(IFoo fake, int result)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And a fake method with 3 parameters is configured to return a value when the arguments match a predicate"
                .x(() => A.CallTo(() => fake.Bar(0, null, false))
                            .WhenArgumentsMatch((int a, string b, bool c) => a % 2 == 0 && b.Length < 3 && c)
                            .Returns(42));

            "When the method is called with arguments that satisfy the predicate"
                .x(() => result = fake.Bar(4, "x", true));

            "Then it returns the configured value"
                .x(() => result.Should().Be(42));
        }

        [Scenario]
        public static void StronglyTypedWhenArgumentsMatchWith3ArgsFailure(IFoo fake, int result)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And a fake method with 3 parameters is configured to return a value when the arguments match a predicate"
                .x(() => A.CallTo(() => fake.Bar(0, null, false))
                            .WhenArgumentsMatch((int a, string b, bool c) => a % 2 == 0 && b.Length < 3 && c)
                            .Returns(42));

            "When the method is called with arguments that don't satisfy the predicate"
                .x(() => result = fake.Bar(3, "hello", false));

            "Then it returns the default value"
                .x(() => result.Should().Be(0));
        }

        [Scenario]
        public static void StronglyTypedWhenArgumentsMatchWith3ArgsWrongSignature(IFoo fake, Exception exception)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And a fake method with 3 parameters is configured with an arguments predicate with an incompatible signature"
                .x(() => exception = Record.Exception(
                        () => A.CallTo(() => fake.Bar(0, null, false))
                            .WhenArgumentsMatch((long a, DateTime b, Type c) => true)
                            .Returns(42)));

            "When the method is called"
                .x(() => exception = Record.Exception(() => fake.Bar(3, "hello", true)));

            "Then it throws a FakeConfigurationException that describes the problem"
                .x(() => exception.Should().BeAnExceptionOfType<FakeConfigurationException>()
                            .WithMessage("The faked method has the signature (System.Int32, System.String, System.Boolean), but when arguments match was used with (System.Int64, System.DateTime, System.Type)."));
        }

        [Scenario]
        public static void StronglyTypedWhenArgumentsMatchWith4ArgsSuccess(IFoo fake, int result)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And a fake method with 4 parameters is configured to return a value when the arguments match a predicate"
                .x(() => A.CallTo(() => fake.Bar(0, null, false, null))
                            .WhenArgumentsMatch((int a, string b, bool c, object d) => a % 2 == 0 && b.Length < 3 && c && d != null)
                            .Returns(42));

            "When the method is called with arguments that satisfy the predicate"
                .x(() => result = fake.Bar(4, "x", true, new object()));

            "Then it returns the configured value"
                .x(() => result.Should().Be(42));
        }

        [Scenario]
        public static void StronglyTypedWhenArgumentsMatchWith4ArgsFailure(IFoo fake, int result)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And a fake method with 4 parameters is configured to return a value when the arguments match a predicate"
                .x(() => A.CallTo(() => fake.Bar(0, null, false, null))
                            .WhenArgumentsMatch((int a, string b, bool c, object d) => a % 2 == 0 && b.Length < 3 && c && d != null)
                            .Returns(42));

            "When the method is called with arguments that don't satisfy the predicate"
                .x(() => result = fake.Bar(3, "hello", false, null));

            "Then it returns the default value"
                .x(() => result.Should().Be(0));
        }

        [Scenario]
        public static void StronglyTypedWhenArgumentsMatchWith4ArgsWrongSignature(IFoo fake, Exception exception)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And a fake method with 4 parameters is configured with an arguments predicate with an incompatible signature"
                .x(() => exception = Record.Exception(
                        () => A.CallTo(() => fake.Bar(0, null, false, null))
                            .WhenArgumentsMatch((long a, DateTime b, Type c, char d) => true)
                            .Returns(42)));

            "When the method is called"
                .x(() => exception = Record.Exception(() => fake.Bar(3, "hello", true, new object())));

            "Then it throws a FakeConfigurationException that describes the problem"
                .x(() => exception.Should().BeAnExceptionOfType<FakeConfigurationException>()
                            .WithMessage("The faked method has the signature (System.Int32, System.String, System.Boolean, System.Object), but when arguments match was used with (System.Int64, System.DateTime, System.Type, System.Char)."));
        }
    }
}
