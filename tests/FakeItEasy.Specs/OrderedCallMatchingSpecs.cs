namespace FakeItEasy.Specs
{
    using System;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xbehave;

    public static class OrderedCallMatchingSpecs
    {
        public interface IHaveNoGenericParameters
        {
            void Bar(int baz);
        }

        public interface IHaveOneGenericParameter
        {
            void Bar<T>(T baz);
        }

        [Scenario]
        public static void NonGenericCallsSuccess(
            IHaveNoGenericParameters fake,
            ISequentialCallContext sequentialCallContext,
            Exception exception)
        {
            "Given a Fake"
                .x(() => fake = A.Fake<IHaveNoGenericParameters>());

            "And a call on the Fake, passing argument 1"
                .x(() => fake.Bar(1));

            "And a call on the Fake, passing argument 2"
                .x(() => fake.Bar(2));

            "And a call-ordering context"
                .x(c => sequentialCallContext = A.SequentialCallContext());

            "When I expect the call with argument 1 to have been made"
                .x(() => A.CallTo(() => fake.Bar(1)).MustHaveHappened().InOrder(sequentialCallContext));

            "And I expect the call with argument 2 to have been made next"
                .x(() => exception = Record.Exception(() => A.CallTo(() => fake.Bar(2)).MustHaveHappened().InOrder(sequentialCallContext)));

            "Then the assertion should succeed"
                .x(() => exception.Should().BeNull("because the assertion should have succeeded"));
        }

        [Scenario]
        public static void NonGenericCallsFailure(
            IHaveNoGenericParameters fake,
            ISequentialCallContext sequentialCallContext,
            Exception exception)
        {
            "Given a Fake"
                .x(() => fake = A.Fake<IHaveNoGenericParameters>());

            "And a call on the Fake, passing argument 1"
                .x(() => fake.Bar(1));

            "And a call on the Fake, passing argument 2"
                .x(() => fake.Bar(2));

            "And a call-ordering context"
                .x(c => sequentialCallContext = A.SequentialCallContext());

            "When I expect the call with argument 2 to have been made"
                .x(() => A.CallTo(() => fake.Bar(2)).MustHaveHappened().InOrder(sequentialCallContext));

            "And I expect the call with argument 1 to have been made next"
                .x(() => exception = Record.Exception(() => A.CallTo(() => fake.Bar(1)).MustHaveHappened().InOrder(sequentialCallContext)));

            "Then the assertion should fail"
                .x(() => exception.Should().BeAnExceptionOfType<ExpectationException>());

            "And the error should tell us that the calls were not matched in order"
                .x(() => exception.Message.Should().Be(@"

  Assertion failed for the following calls:
    'FakeItEasy.Specs.OrderedCallMatchingSpecs+IHaveNoGenericParameters.Bar(2)' repeated at least once
    'FakeItEasy.Specs.OrderedCallMatchingSpecs+IHaveNoGenericParameters.Bar(1)' repeated at least once
  The calls where found but not in the correct order among the calls:
    1: FakeItEasy.Specs.OrderedCallMatchingSpecs+IHaveNoGenericParameters.Bar(baz: 1)
    2: FakeItEasy.Specs.OrderedCallMatchingSpecs+IHaveNoGenericParameters.Bar(baz: 2)
"));
        }

        [Scenario]
        public static void GenericCallsSuccess(
            IHaveOneGenericParameter fake,
            ISequentialCallContext sequentialCallContext,
            Exception exception)
        {
            "Given a Fake"
                .x(() => fake = A.Fake<IHaveOneGenericParameter>());

            "And a call on the Fake, passing argument of type int"
                .x(() => fake.Bar(1));

            "And a call on the Fake, passing argument of type Generic<bool>"
                .x(() => fake.Bar(new Generic<bool>()));

            "And a call-ordering context"
                .x(c => sequentialCallContext = A.SequentialCallContext());

            "When I expect the call with any argument of type int to have been made"
                .x(() => A.CallTo(() => fake.Bar(A<int>.Ignored)).MustHaveHappened().InOrder(sequentialCallContext));

            "And I expect the call with any argument of type Generic<bool> to have been made next"
                .x(() => exception = Record.Exception(() => A.CallTo(() => fake.Bar(A<Generic<bool>>.Ignored)).MustHaveHappened().InOrder(sequentialCallContext)));

            "Then the assertion should succeed"
                .x(() => exception.Should().BeNull("because the assertion should have succeeded"));
        }

        [Scenario]
        public static void GenericCallsFailure(
            IHaveOneGenericParameter fake,
            ISequentialCallContext sequentialCallContext,
            Exception exception)
        {
            "Given a Fake"
                .x(() => fake = A.Fake<IHaveOneGenericParameter>());

            "And a call on the Fake, passing argument of type int"
                .x(() => fake.Bar(1));

            "And a call on the Fake, passing argument of type Generic<bool>"
                .x(() => fake.Bar(new Generic<bool>()));

            "And a call-ordering context"
                .x(c => sequentialCallContext = A.SequentialCallContext());

            "When I expect the call with any argument of type Generic<bool> to have been made"
                .x(() => A.CallTo(() => fake.Bar(A<Generic<bool>>.Ignored)).MustHaveHappened().InOrder(sequentialCallContext));

            "And I expect the call with any argument of type int to have been made next"
                .x(() => exception = Record.Exception(() => A.CallTo(() => fake.Bar(A<int>.Ignored)).MustHaveHappened().InOrder(sequentialCallContext)));

            "Then the assertion should fail"
                .x(() => exception.Should().BeAnExceptionOfType<ExpectationException>());

            "And the error should tell us that the calls were not matched in order"
                .x(() => exception.Message.Should().Be(
                    @"

  Assertion failed for the following calls:
    'FakeItEasy.Specs.OrderedCallMatchingSpecs+IHaveOneGenericParameter.Bar<FakeItEasy.Specs.OrderedCallMatchingSpecs+Generic<System.Boolean>>(<Ignored>)' repeated at least once
    'FakeItEasy.Specs.OrderedCallMatchingSpecs+IHaveOneGenericParameter.Bar<System.Int32>(<Ignored>)' repeated at least once
  The calls where found but not in the correct order among the calls:
    1: FakeItEasy.Specs.OrderedCallMatchingSpecs+IHaveOneGenericParameter.Bar<System.Int32>(baz: 1)
    2: FakeItEasy.Specs.OrderedCallMatchingSpecs+IHaveOneGenericParameter.Bar<FakeItEasy.Specs.OrderedCallMatchingSpecs+Generic<System.Boolean>>(baz: FakeItEasy.Specs.OrderedCallMatchingSpecs+Generic`1[System.Boolean])
"));
        }

        [Scenario]
        public static void WithRepeatConstraintSuccess(
            IHaveNoGenericParameters fake,
            ISequentialCallContext sequentialCallContext,
            Exception exception)
        {
            "Given a Fake"
                .x(() => fake = A.Fake<IHaveNoGenericParameters>());

            "And a call on the Fake, passing argument 1"
                .x(() => fake.Bar(1));

            "And a call on the Fake, passing argument 1"
                .x(() => fake.Bar(1));

            "And a call on the Fake, passing argument 1"
                .x(() => fake.Bar(1));

            "And a call on the Fake, passing argument 2"
                .x(() => fake.Bar(2));

            "And a call-ordering context"
                .x(c => sequentialCallContext = A.SequentialCallContext());

            "When I expect the call with argument 1 to have been made at least twice"
                .x(() => A.CallTo(() => fake.Bar(1)).MustHaveHappened(Repeated.AtLeast.Twice).InOrder(sequentialCallContext));

            "And I expect the call with argument 2 to have been made next exactly once"
                .x(() => exception = Record.Exception(() => A.CallTo(() => fake.Bar(2)).MustHaveHappened(Repeated.Exactly.Once).InOrder(sequentialCallContext)));

            "Then the assertion should succeed"
                .x(() => exception.Should().BeNull("because the assertion should have succeeded"));
        }

        [Scenario]
        public static void WithRepeatConstraintFailure(
            IHaveNoGenericParameters fake,
            ISequentialCallContext sequentialCallContext,
            Exception exception)
        {
            "Given a Fake"
                .x(() => fake = A.Fake<IHaveNoGenericParameters>());

            "And a call on the Fake, passing argument 1"
                .x(() => fake.Bar(1));

            "And a call on the Fake, passing argument 2"
                .x(() => fake.Bar(2));

            "And a call on the Fake, passing argument 1"
                .x(() => fake.Bar(1));

            "And a call-ordering context"
                .x(c => sequentialCallContext = A.SequentialCallContext());

            "When I expect the call with argument 2 to have been made"
                .x(() => A.CallTo(() => fake.Bar(2)).MustHaveHappened().InOrder(sequentialCallContext));

            "And I expect the call with argument 1 to have been made next exactly once"
                .x(() => exception = Record.Exception(() => A.CallTo(() => fake.Bar(1)).MustHaveHappened(Repeated.Exactly.Once).InOrder(sequentialCallContext)));

            "Then the assertion should fail"
                .x(() => exception.Should().BeAnExceptionOfType<ExpectationException>());

            "And the error should tell us that the call to Bar(1) was found too many times"
                .x(() => exception.Message.Should().Be(@"

  Assertion failed for the following call:
    FakeItEasy.Specs.OrderedCallMatchingSpecs+IHaveNoGenericParameters.Bar(1)
  Expected to find it exactly once but found it #2 times among the calls:
    1: FakeItEasy.Specs.OrderedCallMatchingSpecs+IHaveNoGenericParameters.Bar(baz: 1)
    2: FakeItEasy.Specs.OrderedCallMatchingSpecs+IHaveNoGenericParameters.Bar(baz: 2)
    3: FakeItEasy.Specs.OrderedCallMatchingSpecs+IHaveNoGenericParameters.Bar(baz: 1)

"));
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "NextCall", Justification = "Because it's the correct spelling")]
        [Scenario]
        public static void NextCallToSuccess(
            IHaveNoGenericParameters fake,
            ISequentialCallContext sequentialCallContext,
            Exception exception)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IHaveNoGenericParameters>());

            "And a call on the Fake, passing argument 1"
                .x(() => fake.Bar(1));

            "And a call on the Fake, passing argument 2"
                .x(() => fake.Bar(2));

            "And a call-ordering context"
                .x(c => sequentialCallContext = A.SequentialCallContext());

            "When I expect the call with argument 1 to have been made using NextCall.To"
                .x(() =>
                {
                    NextCall.To(fake).MustHaveHappened().InOrder(sequentialCallContext);
                    fake.Bar(1);
                });

            "And I expect the call with argument 2 to have been made next using NextCall.To"
                .x(() => exception = Record.Exception(() =>
                {
                    NextCall.To(fake).MustHaveHappened().InOrder(sequentialCallContext);
                    fake.Bar(2);
                }));

            "Then the assertion should succeed"
                .x(() => exception.Should().BeNull("because the assertion should have succeeded"));
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "NextCall", Justification = "Because it's the correct spelling")]
        [Scenario]
        public static void NextCallToFailure(
            IHaveNoGenericParameters fake,
            ISequentialCallContext sequentialCallContext,
            Exception exception)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IHaveNoGenericParameters>());

            "And a call on the Fake, passing argument 1"
                .x(() => fake.Bar(1));

            "And a call on the Fake, passing argument 2"
                .x(() => fake.Bar(2));

            "And a call-ordering context"
                .x(c => sequentialCallContext = A.SequentialCallContext());

            "When I expect the call with argument 2 to have been made using NextCall.To"
                .x(() =>
                {
                    NextCall.To(fake).MustHaveHappened().InOrder(sequentialCallContext);
                    fake.Bar(2);
                });

            "And I expect the call with argument 1 to have been made next using NextCall.To"
                .x(() => exception = Record.Exception(() =>
                {
                    NextCall.To(fake).MustHaveHappened().InOrder(sequentialCallContext);
                    fake.Bar(1);
                }));

            "Then the assertion should fail"
                .x(() => exception.Should().BeAnExceptionOfType<ExpectationException>());

            "And the error should tell us that the calls were not matched in order"
                .x(() => exception.Message.Should().Be(@"

  Assertion failed for the following calls:
    'FakeItEasy.Specs.OrderedCallMatchingSpecs+IHaveNoGenericParameters.Bar(baz: 2)' repeated at least once
    'FakeItEasy.Specs.OrderedCallMatchingSpecs+IHaveNoGenericParameters.Bar(baz: 1)' repeated at least once
  The calls where found but not in the correct order among the calls:
    1: FakeItEasy.Specs.OrderedCallMatchingSpecs+IHaveNoGenericParameters.Bar(baz: 1)
    2: FakeItEasy.Specs.OrderedCallMatchingSpecs+IHaveNoGenericParameters.Bar(baz: 2)
"));
        }

        public class Generic<T>
        {
        }
    }
}
