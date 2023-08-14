namespace FakeItEasy.Specs
{
    using System;
    using System.Collections.Generic;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xbehave;
    using Xunit;

    public static class ArgumentEqualityComparerSpecs
    {
        [Scenario]
        public static void CustomArgumentEqualityComparer(IFoo fake, int result)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And a type for which a custom argument equality comparer exists"
                .See<ClassWithCustomArgumentEqualityComparer>();

            "When a call to the fake is configured with a specific argument value"
                .x(() => A.CallTo(() => fake.Bar(new ClassWithCustomArgumentEqualityComparer { Value = 1 })).Returns(42));

            "And a call to the fake is made with a distinct but identical instance"
                .x(() => result = fake.Bar(new ClassWithCustomArgumentEqualityComparer { Value = 1 }));

            "Then it should return the configured value"
                .x(() => result.Should().Be(42));
        }

        [Scenario]
        public static void TwoCustomArgumentEqualityComparers(IFoo fake, int result)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And a type for which two custom argument equality comparers exist"
                .See<ClassWithTwoEligibleArgumentEqualityComparers>();

            "When a call to the fake is configured with a specific argument value"
                .x(() => A.CallTo(() => fake.Baz(new ClassWithTwoEligibleArgumentEqualityComparers { X = 1, Y = 1 })).Returns(42));

            "And a call to the fake is made with a distinct but identical instance according to the higher-priority comparer"
                .x(() => result = fake.Baz(new ClassWithTwoEligibleArgumentEqualityComparers { X = 0, Y = 1 }));

            "Then it should return the configured value"
                .x(() => result.Should().Be(42));
        }

        [Scenario]
        public static void ArgumentEqualityComparerThatThrows(IFoo fake, Exception? exception)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And a type for which there's an argument equality comparer that throws"
                .See<ClassWithEqualityComparerThatThrows>();

            "When a call to the fake is configured with a specific argument value"
                .x(() => A.CallTo(() => fake.Frob(new ClassWithEqualityComparerThatThrows())).Returns(42));

            "And a call to the fake is made"
                .x(() => exception = Record.Exception(() => fake.Frob(new ClassWithEqualityComparerThatThrows())));

            "Then it should throw a UserCallbackException with a message explaining what happened"
                .x(() => exception.Should().BeOfType<UserCallbackException>()
                    .Which.Message.Should().Be("Argument Equality Comparer threw an exception. See inner exception for details."));

            "And the exception should wrap the original exception"
                .x(() => exception!.InnerException.Should().BeOfType<Exception>()
                    .Which.Message.Should().Be("Oops"));
        }

        [Scenario]
        public static void ArgumentEqualityComparerThatThrowsNullExample(IFoo fake, Exception exception)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And a type for which a throwing custom argument equality comparer exists"
                .See<ClassWithCustomArgumentEqualityComparer>();

            "And a method on the fake is called with a non-null value"
                .x(() => fake.Frob(new ClassWithEqualityComparerThatThrows()));

            "When the method is checked to see if the call was made with a null example argument"
                .x(() => exception = Record.Exception(() => A.CallTo(() => fake.Frob(null)).MustHaveHappened()));

            "Then it should fail with a descriptive message"
                .x(() => exception.Should().BeAnExceptionOfType<ExpectationException>()
                     .WithMessageModuloLineEndings(@"

  Assertion failed for the following call:
    FakeItEasy.Specs.ArgumentEqualityComparerSpecs+IFoo.Frob(arg: NULL)
  Expected to find it once or more but didn't find it among the calls:
    1: FakeItEasy.Specs.ArgumentEqualityComparerSpecs+IFoo.Frob(arg: FakeItEasy.Specs.ArgumentEqualityComparerSpecs+ClassWithEqualityComparerThatThrows)

"));
        }

        [Scenario]
        public static void ArgumentEqualityComparerThatThrowsNullArgument(IFoo fake, Exception exception)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And a type for which a throwing custom argument equality comparer exists"
                .See<ClassWithCustomArgumentEqualityComparer>();

            "And a method on the fake is called with null"
                .x(() => fake.Frob(null));

            "When the method is checked to see if the call was made with a non-null example argument"
                .x(() => exception = Record.Exception(() => A.CallTo(() => fake.Frob(new ClassWithEqualityComparerThatThrows())).MustHaveHappened()));

            "Then it should fail with a descriptive message"
                .x(() => exception.Should().BeAnExceptionOfType<ExpectationException>()
                     .WithMessageModuloLineEndings(@"

  Assertion failed for the following call:
    FakeItEasy.Specs.ArgumentEqualityComparerSpecs+IFoo.Frob(arg: FakeItEasy.Specs.ArgumentEqualityComparerSpecs+ClassWithEqualityComparerThatThrows)
  Expected to find it once or more but didn't find it among the calls:
    1: FakeItEasy.Specs.ArgumentEqualityComparerSpecs+IFoo.Frob(arg: NULL)

"));
        }

        [Scenario]
        public static void ArgumentEqualityComparerThatThrowsNullExampleAndArgument(IFoo fake, Exception exception)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And a type for which a throwing custom argument equality comparer exists"
                .See<ClassWithCustomArgumentEqualityComparer>();

            "And a method on the fake is called with null"
                .x(() => fake.Frob(null));

            "When the method is checked to see if the call was made with a null example argument"
                .x(() => exception = Record.Exception(() => A.CallTo(() => fake.Frob(null)).MustHaveHappened()));

            "Then it should pass"
                .x(() => exception.Should().BeNull());
        }

        [Scenario]
        public static void ArgumentEqualityComparerObjectParameter(IFoo fake, int result)
        {
            "Given a fake with a method that has an object parameter"
                .x(() => fake = A.Fake<IFoo>());

            "And a type for which a throwing custom argument equality comparer exists"
                .See<ClassWithCustomArgumentEqualityComparer>();

            "When a call to the fake is configured with a specific argument value"
                .x(() => A.CallTo(() => fake.ConsumeObject(new ClassWithCustomArgumentEqualityComparer { Value = 7 })).Returns(53));

            "And a call to the fake is made with a distinct but identical instance"
                .x(() => result = fake.ConsumeObject(new ClassWithCustomArgumentEqualityComparer { Value = 7 }));

            "Then it should return the configured value"
                .x(() => result.Should().Be(53));
        }

        [Scenario]
        public static void ArgumentEqualityComparerObjectWrongType(IFoo fake, int result)
        {
            "Given a fake with a method that has an object parameter"
                .x(() => fake = A.Fake<IFoo>());

            "And a type for which a throwing custom argument equality comparer exists"
                .See<ClassWithCustomArgumentEqualityComparer>();

            "When a call to the fake is configured with a specific argument value"
                .x(() => A.CallTo(() => fake.ConsumeObject(new ClassWithCustomArgumentEqualityComparer { Value = 7 })).Returns(53));

            "And a call to the fake is made with an instance of an incompatible type"
                .x(() => result = fake.ConsumeObject("I am not the right type"));

            "Then it should not return the configured value"
                .x(() => result.Should().NotBe(53));
        }

        [Scenario]
        public static void NestedCustomArgumentEqualityComparerMatches(IFoo fake, int result)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And a type for which a custom argument equality comparer exists"
                .See<ClassWithCustomArgumentEqualityComparer>();

            "When a call to the fake is configured with a sequence of those types"
                .x(() => A.CallTo(() => fake.Barlist(new[]
                {
                    new ClassWithCustomArgumentEqualityComparer { Value = 1 },
                    new ClassWithCustomArgumentEqualityComparer { Value = 2 },
                })).Returns(42));

            "And a call to the fake is made with a distinct but identical sequence"
                .x(() => result = fake.Barlist(
                    new List<ClassWithCustomArgumentEqualityComparer>
                    {
                        new ClassWithCustomArgumentEqualityComparer { Value = 1 },
                        new ClassWithCustomArgumentEqualityComparer { Value = 2 },
                    }));

            "Then it should return the configured value"
                .x(() => result.Should().Be(42));
        }

        [Scenario]
        public static void NestedCustomArgumentEqualityComparerMismatch(IFoo fake, int result)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And a type for which a custom argument equality comparer exists"
                .See<ClassWithCustomArgumentEqualityComparer>();

            "When a call to the fake is configured with a sequence of those types"
                .x(() => A.CallTo(() => fake.Barlist(new[]
                {
                    new ClassWithCustomArgumentEqualityComparer { Value = 1 },
                    new ClassWithCustomArgumentEqualityComparer { Value = 2 },
                })).Returns(42));

            "And a call to the fake is made with a different sequence"
                .x(() => result = fake.Barlist(
                    new List<ClassWithCustomArgumentEqualityComparer>
                    {
                        new ClassWithCustomArgumentEqualityComparer { Value = 2 },
                        new ClassWithCustomArgumentEqualityComparer { Value = 1 },
                    }));

            "Then it should not return the configured value"
                .x(() => result.Should().NotBe(42));
        }

        public interface IFoo
        {
            int Bar(ClassWithCustomArgumentEqualityComparer? arg);

            int Barlist(IEnumerable<ClassWithCustomArgumentEqualityComparer> arg);

            int Baz(ClassWithTwoEligibleArgumentEqualityComparers arg);

            int Frob(ClassWithEqualityComparerThatThrows? arg);

            int ConsumeObject(object arg);
        }

        public class ClassWithCustomArgumentEqualityComparer
        {
            public int Value { get; set; }
        }

        public class CustomComparer : ArgumentEqualityComparer<ClassWithCustomArgumentEqualityComparer>
        {
            protected override bool AreEqual(
                ClassWithCustomArgumentEqualityComparer expectedValue,
                ClassWithCustomArgumentEqualityComparer argumentValue)
            {
                return expectedValue.Value == argumentValue.Value;
            }
        }

        public class ClassWithTwoEligibleArgumentEqualityComparers
        {
            public int X { get; set; }

            public int Y { get; set; }
        }

        public class XComparer : ArgumentEqualityComparer<ClassWithTwoEligibleArgumentEqualityComparers>
        {
            public override Priority Priority => new Priority(1);

            protected override bool AreEqual(
                ClassWithTwoEligibleArgumentEqualityComparers expectedValue,
                ClassWithTwoEligibleArgumentEqualityComparers argumentValue)
            {
                return expectedValue.X == argumentValue.X;
            }
        }

        public class YComparer : ArgumentEqualityComparer<ClassWithTwoEligibleArgumentEqualityComparers>
        {
            public override Priority Priority => new Priority(2);

            protected override bool AreEqual(
                ClassWithTwoEligibleArgumentEqualityComparers expectedValue,
                ClassWithTwoEligibleArgumentEqualityComparers argumentValue)
            {
                return expectedValue.Y == argumentValue.Y;
            }
        }

        public class ClassWithEqualityComparerThatThrows
        {
        }

        public class ComparerThatThrows : ArgumentEqualityComparer<ClassWithEqualityComparerThatThrows>
        {
            protected override bool AreEqual(
                ClassWithEqualityComparerThatThrows expectedValue,
                ClassWithEqualityComparerThatThrows argumentValue)
            {
                throw new Exception("Oops");
            }
        }
    }
}
