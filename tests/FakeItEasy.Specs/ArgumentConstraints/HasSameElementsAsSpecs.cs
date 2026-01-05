namespace FakeItEasy.Specs.ArgumentConstraints;

using System;
using System.Collections.Generic;
using FakeItEasy.Tests.TestHelpers;
using FluentAssertions;
using LambdaTale;
using Xunit;

public static class HasSameElementsAsSpecs
{
    public interface IIHaveACollectionParameter
    {
        int Bar(int[] args);

        int Baz(string?[] args);
    }

    [Scenario]
    public static void Success_SameSequence(IIHaveACollectionParameter fake)
    {
        "Given a fake with a method that has a collection parameter"
            .x(() => fake = A.Fake<IIHaveACollectionParameter>());

        "When the method is called with a specific array"
            .x(() => fake.Bar(new[] { 1, 2, 3 }));

        "Then an assertion that it was called with an array that has the same elements in the same order should pass"
            .x(() => A.CallTo(() => fake.Bar(A<int[]>.That.HasSameElementsAs(1, 2, 3))).MustHaveHappened());
    }

    [Scenario]
    public static void Success_SameElementsDifferentOrder(IIHaveACollectionParameter fake)
    {
        "Given a fake with a method that has a collection parameter"
            .x(() => fake = A.Fake<IIHaveACollectionParameter>());

        "When the method is called with a specific array"
            .x(() => fake.Bar(new[] { 3, 2, 1, 2 }));

        "Then an assertion that it was called with an array that has the same elements in a different order should pass"
            .x(() => A.CallTo(() => fake.Bar(A<int[]>.That.HasSameElementsAs(new[] { 1, 2, 2, 3 }))).MustHaveHappened());
    }

    [Scenario]
    public static void Success_SameElementsDifferentOrder_UsingParamsOverload(IIHaveACollectionParameter fake)
    {
        "Given a fake with a method that has a collection parameter"
            .x(() => fake = A.Fake<IIHaveACollectionParameter>());

        "When the method is called with a specific array"
            .x(() => fake.Bar([3, 2, 1, 2]));

        "Then an assertion that it was called with an array that has the same elements in a different order should pass"
            .x(() => A.CallTo(() => fake.Bar(A<int[]>.That.HasSameElementsAs(1, 2, 2, 3))).MustHaveHappened());
    }

    [Scenario]
    public static void Success_SameElements_DifferentOrder_WithEqualityComparer(IIHaveACollectionParameter fake)
    {
        "Given a fake with a method that has a collection parameter"
            .x(() => fake = A.Fake<IIHaveACollectionParameter>());

        "When the method is called with a specific array"
            .x(() => fake.Bar(new[] { 3, -2, -1, 2 }));

        "Then an assertion that it was called with an array that has the same elements in a different order, using a specific equality comparer, should pass"
            .x(() => A.CallTo(() => fake.Bar(A<int[]>.That.HasSameElementsAs(
                new[] { 1, 2, 2, 3 },
                new AbsoluteValueIntEqualityComparer()))).MustHaveHappened());
    }

    [Scenario]
    public static void Success_SameElementsDifferentOrder_WithNullElements(IIHaveACollectionParameter fake)
    {
        "Given a fake with a method that has a collection parameter with nullable elements"
            .x(() => fake = A.Fake<IIHaveACollectionParameter>());

        "When the method is called with a specific array including null elements"
            .x(() => fake.Baz(new[] { "hello", null, "world", null }));

        "Then an assertion that it was called with an array that has the same elements in a different order should pass"
            .x(() => A.CallTo(() => fake.Baz(A<string?[]>.That.HasSameElementsAs(new[] { "hello", "world", null, null }))).MustHaveHappened());
    }

    [Scenario]
    public static void Failure_DifferentElements(IIHaveACollectionParameter fake, Exception? exception)
    {
        "Given a fake with a method that has a collection parameter"
            .x(() => fake = A.Fake<IIHaveACollectionParameter>());

        "When the method is called with a specific array"
            .x(() => fake.Bar(new[] { 1, 2, 3 }));

        "And an assertion is made that it was called with an array that has a different set of elements"
            .x(() => exception = Record.Exception(() =>
                A.CallTo(() => fake.Bar(A<int[]>.That.HasSameElementsAs(new[] { 4, 5, 6 }))).MustHaveHappened()));

        "Then an exception should be thrown"
            .x(() => exception.Should().NotBeNull());

        "And the exception message should tell us that the call was not matched, and include the values of the actual collection elements"
            .x(() => exception!.Message.Should().BeModuloLineEndings("""


                  Assertion failed for the following call:
                    FakeItEasy.Specs.ArgumentConstraints.HasSameElementsAsSpecs+IIHaveACollectionParameter.Bar(args: <4, 5, 6 (in any order)>)
                  Expected to find it once or more but didn't find it among the calls:
                    1: FakeItEasy.Specs.ArgumentConstraints.HasSameElementsAsSpecs+IIHaveACollectionParameter.Bar(args: [1, 2, 3])


                """));
    }

    [Scenario]
    public static void Failure_CallWithNull(IIHaveACollectionParameter fake, Exception? exception)
    {
        "Given a fake with a method that has a collection parameter"
            .x(() => fake = A.Fake<IIHaveACollectionParameter>());

        "When the method is called with null"
            .x(() => fake.Bar(null!));

        "And an assertion is made that it was called with a given array"
            .x(() => exception = Record.Exception(() =>
                A.CallTo(() => fake.Bar(A<int[]>.That.HasSameElementsAs(new[] { 4, 5, 6 }))).MustHaveHappened()));

        "Then an exception should be thrown"
            .x(() => exception.Should().NotBeNull());

        "And the exception message should tell us that the call was not matched, and include the values of the actual collection elements"
            .x(() => exception!.Message.Should().BeModuloLineEndings("""


                  Assertion failed for the following call:
                    FakeItEasy.Specs.ArgumentConstraints.HasSameElementsAsSpecs+IIHaveACollectionParameter.Bar(args: <4, 5, 6 (in any order)>)
                  Expected to find it once or more but didn't find it among the calls:
                    1: FakeItEasy.Specs.ArgumentConstraints.HasSameElementsAsSpecs+IIHaveACollectionParameter.Bar(args: NULL)


                """));
    }

    private sealed class AbsoluteValueIntEqualityComparer : IEqualityComparer<int>
    {
        public bool Equals(int x, int y) => EqualityComparer<int>.Default.Equals(Math.Abs(x), Math.Abs(y));

        public int GetHashCode(int obj) => EqualityComparer<int>.Default.GetHashCode(Math.Abs(obj));
    }
}
