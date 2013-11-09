namespace FakeItEasy.Specs
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Machine.Specifications;

    public class when_matching_calls_with_parameter_arrays
    {
        static ITypeWithParameterArray fake;

        Establish context = () => fake = A.Fake<ITypeWithParameterArray>();

        Because of = () => fake.MethodWithParameterArray("foo", "bar", "baz");

        It should_be_able_to_match_the_call =
            () => A.CallTo(() => fake.MethodWithParameterArray("foo", "bar", "baz")).MustHaveHappened();

        It should_be_able_to_match_the_call_with_argument_constraints =
            () => A.CallTo(() => fake.MethodWithParameterArray(A<string>._, A<string>._, A<string>._)).MustHaveHappened();

        It should_be_able_to_match_the_call_mixing_constraints_and_values =
            () => A.CallTo(() => fake.MethodWithParameterArray(A<string>._, "bar", A<string>._)).MustHaveHappened();

        It should_be_able_to_match_using_array_syntax =
            () => A.CallTo(() => fake.MethodWithParameterArray("foo", A<string[]>.That.IsSameSequenceAs(new[] { "bar", "baz" }))).MustHaveHappened();

        public interface ITypeWithParameterArray
        {
            void MethodWithParameterArray(string arg, params string[] args);
        }
    }

    public class when_failing_to_match_non_generic_calls
    {
        static IFoo fake;
        static Exception exception;

        Establish context = () => fake = A.Fake<IFoo>();

        Because of = () =>
        {
            fake.Bar(1);
            fake.Bar(2);
            exception = Catch.Exception(() => A.CallTo(() => fake.Bar(3)).MustHaveHappened());
        };

        It should_tell_us_that_the_call_was_not_matched = () => exception.Message.ShouldEqual(
@"

  Assertion failed for the following call:
    FakeItEasy.Specs.when_failing_to_match_non_generic_calls+IFoo.Bar(3)
  Expected to find it at least once but found it #0 times among the calls:
    1: FakeItEasy.Specs.when_failing_to_match_non_generic_calls+IFoo.Bar(baz: 1)
    2: FakeItEasy.Specs.when_failing_to_match_non_generic_calls+IFoo.Bar(baz: 2)

");

        public interface IFoo
        {
            void Bar(int baz);
        }
    }

    public class when_failing_to_match_generic_calls
    {
        static IFoo fake;
        static Exception exception;

        Establish context = () => fake = A.Fake<IFoo>();

        Because of = () =>
        {
            fake.Bar(1, 2D);
            fake.Bar(new Generic<bool, long>(), 3);
            exception = Catch.Exception(() => A.CallTo(() => fake.Bar(A<string>.Ignored, A<string>.Ignored)).MustHaveHappened());
        };

        It should_tell_us_that_the_call_was_not_matched = () => exception.Message.ShouldEqual(
@"

  Assertion failed for the following call:
    FakeItEasy.Specs.when_failing_to_match_generic_calls+IFoo.Bar<System.String, System.String>(<Ignored>, <Ignored>)
  Expected to find it at least once but found it #0 times among the calls:
    1: FakeItEasy.Specs.when_failing_to_match_generic_calls+IFoo.Bar<System.Int32, System.Double>(baz1: 1, baz2: 2)
    2: FakeItEasy.Specs.when_failing_to_match_generic_calls+IFoo.Bar<FakeItEasy.Specs.when_failing_to_match_generic_calls+Generic<System.Boolean, System.Int64>, System.Int32>(baz1: FakeItEasy.Specs.when_failing_to_match_generic_calls+Generic`2[System.Boolean,System.Int64], baz2: 3)

");

        public interface IFoo
        {
            void Bar<T1, T2>(T1 baz1, T2 baz2);
        }

        public class Generic<T1, T2>
        {
        }
    }

    public class when_no_non_generic_calls
    {
        static IFoo fake;
        static Exception exception;

        Establish context = () => fake = A.Fake<IFoo>();

        Because of = () => exception = Catch.Exception(() => A.CallTo(() => fake.Bar(A<int>.Ignored)).MustHaveHappened());

        It should_tell_us_that_the_call_was_not_matched = () => exception.Message.ShouldEqual(
@"

  Assertion failed for the following call:
    FakeItEasy.Specs.when_no_non_generic_calls+IFoo.Bar(<Ignored>)
  Expected to find it at least once but no calls were made to the fake object.

");

        public interface IFoo
        {
            void Bar(int baz);
        }
    }

    public class when_no_generic_calls
    {
        static IFoo fake;
        static Exception exception;

        Establish context = () => fake = A.Fake<IFoo>();

        Because of = () => exception = Catch.Exception(() => A.CallTo(() => fake.Bar<Generic<string>>(A<Generic<string>>.Ignored)).MustHaveHappened());

        It should_tell_us_that_the_call_was_not_matched = () => exception.Message.ShouldEqual(
@"

  Assertion failed for the following call:
    FakeItEasy.Specs.when_no_generic_calls+IFoo.Bar<FakeItEasy.Specs.when_no_generic_calls+Generic<System.String>>(<Ignored>)
  Expected to find it at least once but no calls were made to the fake object.

");

        public interface IFoo
        {
            void Bar<T>(T baz);
        }

        public class Generic<T>
        {
        }
    }
}