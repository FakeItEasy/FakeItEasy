namespace FakeItEasy.Specs;

using System;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy.Tests.TestHelpers;
using FluentAssertions;
using LambdaTale;
using Xunit;

public static class ArgumentCapturingSpecs
{
    [Scenario]
    public static void CaptureSingleValue(Action<int> fake, Captured<int> capturedArgument)
    {
        "Given a fake"
            .x(() => fake = A.Fake<Action<int>>());

        "And a captured argument instance"
            .x(() => capturedArgument = A.Captured<int>());

        "And a fake method is configured to capture its argument to that instance"
            .x(() => A.CallTo(() => fake.Invoke(capturedArgument._))
                .DoesNothing());

        "When the method is called"
            .x(() => fake.Invoke(354897));

        "Then the captured argument instance's last value matches the input"
            .x(() => capturedArgument.GetLastValue().Should().Be(354897));
    }

    [Scenario]
    public static void MismatchedVerifyWithCapture(
        Action<int, int> fake, Captured<int> capturedArgument, Exception exception)
    {
        "Given a fake"
            .x(() => fake = A.Fake<Action<int, int>>());

        "And a captured argument instance"
            .x(() => capturedArgument = A.Captured<int>());

        "And a method on the fake is called"
            .x(() => fake.Invoke(354897, -3));

        "When we assert that the call must've happened with a capturing argument and another non-matching argument"
            .x(() => exception = Record.Exception(
                () => A.CallTo(() => fake.Invoke(capturedArgument._, 3))
                    .MustHaveHappened()));

        "Then an exception is thrown, indicating that there was no constraint on the argument"
            .x(() => exception.Should().BeAnExceptionOfType<ExpectationException>()
                .WithMessageModuloLineEndings(@"

  Assertion failed for the following call:
    System.Action`2[System.Int32,System.Int32].Invoke(arg1: <Ignored>, arg2: 3)
  Expected to find it once or more but didn't find it among the calls:
    1: System.Action`2[System.Int32,System.Int32].Invoke(arg1: 354897, arg2: -3)

"));
    }

    [Scenario]
    public static void ValueNotCapturedOnRuleMismatch(
        Action<int, string> fake, Captured<int> capturedArgument)
    {
        "Given a fake"
            .x(() => fake = A.Fake<Action<int, string>>());

        "And a captured argument instance"
            .x(() => capturedArgument = A.Captured<int>());

        "And a fake method with 2 parameters is configured to capture an argument to that instance"
            .x(() => A.CallTo(() => fake.Invoke(capturedArgument._, "matching"))
                .DoesNothing());

        "When the method is called with non-matching arguments"
            .x(() => fake.Invoke(354897, "not matching"));

        "Then no values are captured"
            .x(() => capturedArgument.Values.Should().BeEmpty());
    }

    [Scenario]
    public static void GetLastValueThrowsAfterNoCaptures(
        Action<int, string> fake, Captured<int> capturedArgument, Exception exception)
    {
        "Given a fake"
            .x(() => fake = A.Fake<Action<int, string>>());

        "And a captured argument instance"
            .x(() => capturedArgument = A.Captured<int>());

        "And a fake method is configured to capture an argument to that instance"
            .x(() => A.CallTo(() => fake.Invoke(capturedArgument.Ignored, "matching"))
                .DoesNothing());

        "When the method is called with non-matching arguments"
            .x(() => fake.Invoke(354897, "not matching"));

        "And we try to get the last captured value"
            .x(() => exception = Record.Exception(() => capturedArgument.GetLastValue()));

        "Then an exception is thrown"
            .x(() => exception.Should()
                .BeAnExceptionOfType<ExpectationException>()
                .WithMessage("No values were captured."));
    }

    [Scenario]
    public static void CaptureArgumentsFromMultipleCalls(Action<int> fake, Captured<int> capturedArgument)
    {
        "Given a fake"
            .x(() => fake = A.Fake<Action<int>>());

        "And a captured argument instance"
            .x(() => capturedArgument = A.Captured<int>());

        "And a fake method is configured to capture its argument to that instance"
            .x(() => A.CallTo(() => fake.Invoke(capturedArgument._))
                .DoesNothing());

        "When the method is called"
            .x(() => fake.Invoke(589711));

        "And the method is called again"
            .x(() => fake.Invoke(846722));

        "And the method is called yet again"
            .x(() => fake.Invoke(359633));

        "Then the captured argument instance's values match the inputs"
            .x(() => capturedArgument.Values.Should().Equal(589711, 846722, 359633));
    }

    [Scenario]
    public static void CaptureConstrainedValues(Action<int> fake, Captured<int> capturedArgument)
    {
        "Given a fake"
            .x(() => fake = A.Fake<Action<int>>());

        "And a captured argument instance"
            .x(() => capturedArgument = A.Captured<int>());

        "And a fake method is configured to capture arguments bigger than 10"
            .x(() => A.CallTo(() =>
                    fake.Invoke(capturedArgument.That.IsGreaterThan(10)))
                .DoesNothing());

        "When the method is called with a few values"
            .x(() =>
            {
                fake.Invoke(354897);
                fake.Invoke(9);
                fake.Invoke(10);
                fake.Invoke(11);
                fake.Invoke(-10);
                fake.Invoke(14);
                fake.Invoke(2);
            });

        "Then the captured argument instance's Values contain the big numbers"
            .x(() => capturedArgument.Values.Should().Equal(354897, 11, 14));
    }

    [Scenario]
    public static void CaptureNegatedConstrainedValues(Action<int> fake, Captured<int> capturedArgument)
    {
        "Given a fake"
            .x(() => fake = A.Fake<Action<int>>());

        "And a captured argument instance"
            .x(() => capturedArgument = A.Captured<int>());

        "And a fake method is configured to capture arguments except those bigger than 10"
            .x(() => A.CallTo(() =>
                    fake.Invoke(capturedArgument.That.Not.IsGreaterThan(10)))
                .DoesNothing());

        "When the method is called with a few values"
            .x(() =>
            {
                fake.Invoke(354897);
                fake.Invoke(9);
                fake.Invoke(10);
                fake.Invoke(11);
                fake.Invoke(-10);
                fake.Invoke(14);
                fake.Invoke(2);
            });

        "Then the captured argument instance's Values contain the little numbers"
            .x(() => capturedArgument.Values.Should().Equal(9, 10, -10, 2));
    }

    [Scenario]
    public static void MismatchedVerifyWithConstrainedCapture(
        Action<int, int> fake, Captured<int> capturedArgument, Exception exception)
    {
        "Given a fake"
            .x(() => fake = A.Fake<Action<int, int>>());

        "And a captured argument instance"
            .x(() => capturedArgument = A.Captured<int>());

        "And a method on the fake is called"
            .x(() => fake.Invoke(-354897, 3));

        "When we assert that the call must've happened with a constrained capturing argument and another non-matching argument"
            .x(() => exception = Record.Exception(
                () => A.CallTo(() =>
                        fake.Invoke(capturedArgument.That.IsGreaterThan(0), -3))
                    .MustHaveHappened()));

        "Then an exception is thrown, indicating the constraint on the argument"
            .x(() => exception.Should().BeAnExceptionOfType<ExpectationException>()
                .WithMessageModuloLineEndings(@"

  Assertion failed for the following call:
    System.Action`2[System.Int32,System.Int32].Invoke(arg1: <greater than 0>, arg2: -3)
  Expected to find it once or more but didn't find it among the calls:
    1: System.Action`2[System.Int32,System.Int32].Invoke(arg1: -354897, arg2: 3)

"));
    }

    [Scenario]
    public static void MismatchedVerifyWithNegatedConstrainedCapture(
        Action<int, int> fake, Captured<int> capturedArgument, Exception exception)
    {
        "Given a fake"
            .x(() => fake = A.Fake<Action<int, int>>());

        "And a captured argument instance"
            .x(() => capturedArgument = A.Captured<int>());

        "And a method on the fake is called"
            .x(() => fake.Invoke(-354897, 3));

        "When we assert that the call must've happened with a negated constrained capturing argument and another non-matching argument"
            .x(() => exception = Record.Exception(
                () => A.CallTo(() =>
                        fake.Invoke(capturedArgument.That.Not.IsGreaterThan(0), -3))
                    .MustHaveHappened()));

        "Then an exception is thrown, indicating the constraint on the argument"
            .x(() => exception.Should().BeAnExceptionOfType<ExpectationException>()
                .WithMessageModuloLineEndings(@"

  Assertion failed for the following call:
    System.Action`2[System.Int32,System.Int32].Invoke(arg1: <not greater than 0>, arg2: -3)
  Expected to find it once or more but didn't find it among the calls:
    1: System.Action`2[System.Int32,System.Int32].Invoke(arg1: -354897, arg2: 3)

"));
    }

    [Scenario]
    public static void CaptureMutatingArgument(
        Action<IList<int>> fake, IList<int> listOfInts, Captured<IList<int>, List<int>> capturedArgument)
    {
        "Given a fake"
            .x(() => fake = A.Fake<Action<IList<int>>>());

        "And a captured argument instance that freezes its arguments"
            .x(() => capturedArgument = A.Captured<IList<int>>().FrozenBy(l => l.ToList()));

        "And a fake method is configured to capture its argument to that instance"
            .x(() => A.CallTo(() => fake.Invoke(capturedArgument._))
                .DoesNothing());

        "And mutable variable that will be input to the method"
            .x(() => listOfInts = [1, 2, 3, 4, 5]);

        "When the method is called with the variable"
            .x(() => fake.Invoke(listOfInts));

        "And the variable is mutated"
            .x(() => listOfInts.Add(6));

        "And the method is called with the variable again"
            .x(() => fake.Invoke(listOfInts));

        "Then the captured arguments instance's values match the input at the time of the calls"
            .x(() => capturedArgument.Values.Should().BeEquivalentTo(
                new[]
                {
                    new[] { 1, 2, 3, 4, 5 },
                    new[] { 1, 2, 3, 4, 5, 6 },
                },
                options => options.WithStrictOrdering()));
    }

    [Scenario]
    public static void CaptureTransformedArgument(
        Action<IList<int>> fake, IList<int> listOfInts, Captured<IList<int>, string> capturedArgument)
    {
        "Given a fake"
            .x(() => fake = A.Fake<Action<IList<int>>>());

        "And a captured argument instance that freezes its arguments"
            .x(() => capturedArgument = A.Captured<IList<int>>().FrozenBy(l => string.Join(" ", l)));

        "And a fake method is configured to capture its argument to that instance"
            .x(() => A.CallTo(() => fake.Invoke(capturedArgument._)).DoesNothing());

        "And mutable variable that will be input to the method"
            .x(() => listOfInts = [1, 2, 3, 4, 5]);

        "When the method is called with the variable"
            .x(() => fake.Invoke(listOfInts));

        "And the variable is mutated"
            .x(() => listOfInts.Add(6));

        "And the method is called with the variable again"
            .x(() => fake.Invoke(listOfInts));

        "Then the captured arguments instance's values match the input at the time of the calls"
            .x(() => capturedArgument.Values.Should().Equal("1 2 3 4 5", "1 2 3 4 5 6"));
    }
}
