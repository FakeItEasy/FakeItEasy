namespace FakeItEasy.Specs
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using FluentAssertions;
    using Machine.Specifications;

    public class when_failing_to_match_ordered_non_generic_calls
    {
        static IFoo fake;
        static Exception exception;

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

        It should_tell_us_that_the_call_was_not_matched = () => exception.Message.Should().Be(
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
        static IFoo fake;
        static Exception exception;

        Establish context = () => fake = A.Fake<IFoo>();

        Because of = () =>
        {
            using (var scope = Fake.CreateScope())
            {
                fake.Bar(1);
                fake.Bar(new Generic<bool>());
                using (scope.OrderedAssertions())
                {
                    A.CallTo(() => fake.Bar(A<Generic<bool>>.Ignored)).MustHaveHappened();
                    exception = Catch.Exception(() => A.CallTo(() => fake.Bar(A<int>.Ignored)).MustHaveHappened());
                }
            }
        };

        It should_tell_us_that_the_call_was_not_matched = () => exception.Message.Should().Be(
@"

  Assertion failed for the following calls:
    'FakeItEasy.Specs.when_failing_to_match_ordered_generic_calls+IFoo.Bar<FakeItEasy.Specs.when_failing_to_match_ordered_generic_calls+Generic<System.Boolean>>(<Ignored>)' repeated at least once
    'FakeItEasy.Specs.when_failing_to_match_ordered_generic_calls+IFoo.Bar<System.Int32>(<Ignored>)' repeated at least once
  The calls where found but not in the correct order among the calls:
    1: FakeItEasy.Specs.when_failing_to_match_ordered_generic_calls+IFoo.Bar<System.Int32>(baz: 1)
    2: FakeItEasy.Specs.when_failing_to_match_ordered_generic_calls+IFoo.Bar<FakeItEasy.Specs.when_failing_to_match_ordered_generic_calls+Generic<System.Boolean>>(baz: FakeItEasy.Specs.when_failing_to_match_ordered_generic_calls+Generic`1[System.Boolean])
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