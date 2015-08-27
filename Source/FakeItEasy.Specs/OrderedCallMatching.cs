namespace FakeItEasy.Specs
{
    using System;
    using FluentAssertions;
    using Xbehave;

    public class OrderedCallMatching
    {
        [Scenario]
        public void when_failing_to_match_ordered_non_generic_calls(
            IFoo fake,
            Exception exception)
        {
            "establish"
                .x(() => fake = A.Fake<IFoo>());

            "when failing to match ordered non generic calls"
                .x(() =>
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
                    });

            "it should tell us that the call was not matched"
                .x(() => exception.Message.Should().Be(
                    @"

  Assertion failed for the following calls:
    'FakeItEasy.Specs.OrderedCallMatching+IFoo.Bar(2)' repeated at least once
    'FakeItEasy.Specs.OrderedCallMatching+IFoo.Bar(1)' repeated at least once
  The calls where found but not in the correct order among the calls:
    1: FakeItEasy.Specs.OrderedCallMatching+IFoo.Bar(baz: 1)
    2: FakeItEasy.Specs.OrderedCallMatching+IFoo.Bar(baz: 2)
"));
        }

        public interface IFoo
        {
            void Bar(int baz);
        }
    
        [Scenario]
        public void when_failing_to_match_ordered_generic_calls(
            IGenericFoo fake,
            Exception exception)
        {
            "establish"
                .x(() => fake = A.Fake<IGenericFoo>());

            "when failing to match ordered non generic calls"
                .x(() =>
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
                    });

            "it should tell us that the call was not matched"
                .x(() => exception.Message.Should().Be(
                    @"

  Assertion failed for the following calls:
    'FakeItEasy.Specs.OrderedCallMatching+IGenericFoo.Bar<FakeItEasy.Specs.OrderedCallMatching+Generic<System.Boolean>>(<Ignored>)' repeated at least once
    'FakeItEasy.Specs.OrderedCallMatching+IGenericFoo.Bar<System.Int32>(<Ignored>)' repeated at least once
  The calls where found but not in the correct order among the calls:
    1: FakeItEasy.Specs.OrderedCallMatching+IGenericFoo.Bar<System.Int32>(baz: 1)
    2: FakeItEasy.Specs.OrderedCallMatching+IGenericFoo.Bar<FakeItEasy.Specs.OrderedCallMatching+Generic<System.Boolean>>(baz: FakeItEasy.Specs.OrderedCallMatching+Generic`1[System.Boolean])
"));
        }
        

        public interface IGenericFoo
        {
            void Bar<T>(T baz);
        }

        public class Generic<T>
        {
        }
    }
}