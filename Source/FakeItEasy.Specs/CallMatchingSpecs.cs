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

    public class when_failing_to_match_calls
    {
        private static ITypeWithGenericMethod fake;
        private static Exception exception;

        Establish context = () => fake = A.Fake<ITypeWithGenericMethod>();

        Because of = () =>
        {
            fake.GenericMethod<int>(1);
            fake.GenericMethod<bool>(true);
            exception = Catch.Exception(() => A.CallTo(() => fake.GenericMethod<string>(A<string>.Ignored)).MustHaveHappened());
        };

        It should_tell_us_that_the_call_was_not_matched =
            () => exception.Message.ShouldEqual(
@"

  Assertion failed for the following call:
    FakeItEasy.Specs.when_failing_to_match_calls+ITypeWithGenericMethod.GenericMethod(<Ignored>)
  Expected to find it at least once but found it #0 times among the calls:
    1: FakeItEasy.Specs.when_failing_to_match_calls+ITypeWithGenericMethod.GenericMethod(t: 1)
    2: FakeItEasy.Specs.when_failing_to_match_calls+ITypeWithGenericMethod.GenericMethod(t: True)

");

        public interface ITypeWithGenericMethod
        {
            void GenericMethod<T>(T t);
        }
    }

    public class when_no_calls
    {
        private static ITypeWithGenericMethod fake;
        private static Exception exception;

        Establish context = () => fake = A.Fake<ITypeWithGenericMethod>();

        Because of = () => exception = Catch.Exception(() => A.CallTo(() => fake.GenericMethod<string>(A<string>.Ignored)).MustHaveHappened());

        It should_tell_us_that_the_call_was_not_matched =
            () => exception.Message.ShouldEqual(
@"

  Assertion failed for the following call:
    FakeItEasy.Specs.when_no_calls+ITypeWithGenericMethod.GenericMethod(<Ignored>)
  Expected to find it at least once but no calls were made to the fake object.

");

        public interface ITypeWithGenericMethod
        {
            void GenericMethod<T>(T t);
        }
    }
}