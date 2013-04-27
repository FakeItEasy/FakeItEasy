namespace FakeItEasy.Specs
{
    using System;
    using Machine.Specifications;

    public class when_failing_to_match_ordered_non_generic_calls
    {
        private static IFoo fake;
        private static Exception exception;

        Establish context = () => fake = A.Fake<IFoo>();

        Because of = () =>
        {
            using (var scope = Fake.CreateScope())
            {
                fake.Bar(1);
                fake.Bar(2);
                using (scope.OrderedAssertions())
                {
                    A.CallTo(() => fake.Bar(2)).MustHaveHappened();
                    exception = Catch.Exception(() => A.CallTo(() => fake.Bar(1)).MustHaveHappened());
                }
            }
        };

        It should_tell_us_that_the_call_was_not_matched = () => exception.Message.ShouldEqual(
@"

  Assertion failed for the following calls:
    'FakeItEasy.Specs.when_failing_to_match_ordered_non_generic_calls+IFoo.Bar(2)' repeated at least once
    'FakeItEasy.Specs.when_failing_to_match_ordered_non_generic_calls+IFoo.Bar(1)' repeated at least once
  The calls where found but not in the correct order among the calls:
    1: FakeItEasy.Specs.when_failing_to_match_ordered_non_generic_calls+IFoo.Bar(baz: 1)
    2: FakeItEasy.Specs.when_failing_to_match_ordered_non_generic_calls+IFoo.Bar(baz: 2)
");

        public interface IFoo
        {
            void Bar(int baz);
        }
    }
    
    public class when_failing_to_match_ordered_generic_calls
    {
        private static IFoo fake;
        private static Exception exception;

        Establish context = () => fake = A.Fake<IFoo>();

        Because of = () =>
        {
            using (var scope = Fake.CreateScope())
            {
                fake.Bar<int>(1);
                fake.Bar<bool>(true);
                using (scope.OrderedAssertions())
                {
                    A.CallTo(() => fake.Bar<bool>(A<bool>.Ignored)).MustHaveHappened();
                    exception = Catch.Exception(() => A.CallTo(() => fake.Bar<int>(A<int>.Ignored)).MustHaveHappened());
                }
            }
        };

        It should_tell_us_that_the_call_was_not_matched = () => exception.Message.ShouldEqual(
@"

  Assertion failed for the following calls:
    'FakeItEasy.Specs.when_failing_to_match_ordered_generic_calls+IFoo.Bar<System.Boolean>(<Ignored>)' repeated at least once
    'FakeItEasy.Specs.when_failing_to_match_ordered_generic_calls+IFoo.Bar<System.Int32>(<Ignored>)' repeated at least once
  The calls where found but not in the correct order among the calls:
    1: FakeItEasy.Specs.when_failing_to_match_ordered_generic_calls+IFoo.Bar<System.Int32>(baz: 1)
    2: FakeItEasy.Specs.when_failing_to_match_ordered_generic_calls+IFoo.Bar<System.Boolean>(baz: True)
");

        public interface IFoo
        {
            void Bar<T>(T baz);
        }
    }
}