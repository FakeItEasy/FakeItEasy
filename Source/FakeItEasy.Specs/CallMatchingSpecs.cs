namespace FakeItEasy.Specs
{
    using System;
    using Machine.Specifications;

    public class when_matching_calls_with_param_arrays
    {
        static ITypeWithParamArray fake;

        Establish context = () => fake = A.Fake<ITypeWithParamArray>();

        Because of = () => fake.MethodWithParamArray("foo", "bar", "baz");

        It should_be_able_to_match_the_call =
            () => A.CallTo(() => fake.MethodWithParamArray("foo", "bar", "baz")).MustHaveHappened();

        It should_be_able_to_match_the_call_with_argument_constraints =
            () => A.CallTo(() => fake.MethodWithParamArray(A<string>._, A<string>._, A<string>._)).MustHaveHappened();

        It should_be_able_to_match_the_call_mixing_constraints_and_values =
            () => A.CallTo(() => fake.MethodWithParamArray(A<string>._, "bar", A<string>._)).MustHaveHappened();

        It should_be_able_to_match_using_array_syntax =
            () => A.CallTo(() => fake.MethodWithParamArray("foo", A<string[]>.That.IsSameSequenceAs(new[] { "bar", "baz" }))).MustHaveHappened();

        public interface ITypeWithParamArray
        {
            void MethodWithParamArray(string arg, params string[] args);
        }
    }

    public class when_failing_to_match_non_generic_calls
    {
        private static IFoo fake;
        private static Exception exception;

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
        private static IFoo fake;
        private static Exception exception;

        Establish context = () => fake = A.Fake<IFoo>();

        Because of = () =>
        {
            fake.Bar<int>(1);
            fake.Bar<bool>(true);
            exception = Catch.Exception(() => A.CallTo(() => fake.Bar<string>(A<string>.Ignored)).MustHaveHappened());
        };

        It should_tell_us_that_the_call_was_not_matched = () => exception.Message.ShouldEqual(
@"

  Assertion failed for the following call:
    FakeItEasy.Specs.when_failing_to_match_generic_calls+IFoo.Bar<System.String>(<Ignored>)
  Expected to find it at least once but found it #0 times among the calls:
    1: FakeItEasy.Specs.when_failing_to_match_generic_calls+IFoo.Bar<System.Int32>(baz: 1)
    2: FakeItEasy.Specs.when_failing_to_match_generic_calls+IFoo.Bar<System.Boolean>(baz: True)

");

        public interface IFoo
        {
            void Bar<T>(T baz);
        }
    }

    public class when_no_non_generic_calls
    {
        private static IFoo fake;
        private static Exception exception;

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
        private static IFoo fake;
        private static Exception exception;

        Establish context = () => fake = A.Fake<IFoo>();

        Because of = () => exception = Catch.Exception(() => A.CallTo(() => fake.Bar<string>(A<string>.Ignored)).MustHaveHappened());

        It should_tell_us_that_the_call_was_not_matched = () => exception.Message.ShouldEqual(
@"

  Assertion failed for the following call:
    FakeItEasy.Specs.when_no_generic_calls+IFoo.Bar<System.String>(<Ignored>)
  Expected to find it at least once but no calls were made to the fake object.

");

        public interface IFoo
        {
            void Bar<T>(T baz);
        }
    }
}