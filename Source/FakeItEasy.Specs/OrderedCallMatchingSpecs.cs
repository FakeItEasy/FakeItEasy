namespace FakeItEasy.Specs
{
    using System;
    using Machine.Specifications;

    public class when_failing_to_match_ordered_calls
    {
        private static ITypeWithGenericMethod fake;
        private static Exception exception;

        Establish context = () => fake = A.Fake<ITypeWithGenericMethod>();

        Because of = () =>
        {
            using (var scope = Fake.CreateScope())
            {
                fake.GenericMethod<int>(1);
                fake.GenericMethod<bool>(true);
                using (scope.OrderedAssertions())
                {
                    A.CallTo(() => fake.GenericMethod<bool>(A<bool>.Ignored)).MustHaveHappened();
                    exception = Catch.Exception(() => A.CallTo(() => fake.GenericMethod<int>(A<int>.Ignored)).MustHaveHappened());
                }
            }
        };

        It should_tell_us_that_the_call_was_not_matched =
            () => exception.Message.ShouldEqual(
@"

  Assertion failed for the following calls:
    'FakeItEasy.Specs.when_failing_to_match_ordered_calls+ITypeWithGenericMethod.GenericMethod(<Ignored>)' repeated at least once
    'FakeItEasy.Specs.when_failing_to_match_ordered_calls+ITypeWithGenericMethod.GenericMethod(<Ignored>)' repeated at least once
  The calls where found but not in the correct order among the calls:
    1: FakeItEasy.Specs.when_failing_to_match_ordered_calls+ITypeWithGenericMethod.GenericMethod(t: 1)
    2: FakeItEasy.Specs.when_failing_to_match_ordered_calls+ITypeWithGenericMethod.GenericMethod(t: True)
");

        public interface ITypeWithGenericMethod
        {
            void GenericMethod<T>(T t);
        }
    }
}