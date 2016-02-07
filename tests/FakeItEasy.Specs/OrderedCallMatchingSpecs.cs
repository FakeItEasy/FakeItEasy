namespace FakeItEasy.Specs
{
    using System;
    using FluentAssertions;
    using Xbehave;
    using Xunit;

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
        public static void NonGenericCalls(
            IHaveNoGenericParameters fake,
            Exception exception)
        {
            "establish"
                .x(() => fake = A.Fake<IHaveNoGenericParameters>());

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
                                exception = Record.Exception(() => A.CallTo(() => fake.Bar(1)).MustHaveHappened());
                            }
                        }
                    });

            "it should tell us that the call was not matched"
                .x(() => exception.Message.Should().Be(
                    @"

  Assertion failed for the following calls:
    'FakeItEasy.Specs.OrderedCallMatchingSpecs+IHaveNoGenericParameters.Bar(2)' repeated at least once
    'FakeItEasy.Specs.OrderedCallMatchingSpecs+IHaveNoGenericParameters.Bar(1)' repeated at least once
  The calls where found but not in the correct order among the calls:
    1: FakeItEasy.Specs.OrderedCallMatchingSpecs+IHaveNoGenericParameters.Bar(baz: 1)
    2: FakeItEasy.Specs.OrderedCallMatchingSpecs+IHaveNoGenericParameters.Bar(baz: 2)
"));
        }

        [Scenario]
        public static void GenericCalls(
            IHaveOneGenericParameter fake,
            Exception exception)
        {
            "establish"
                .x(() => fake = A.Fake<IHaveOneGenericParameter>());

            "when failing to match ordered generic calls"
                .x(() =>
                    {
                        using (var scope = Fake.CreateScope())
                        {
                            fake.Bar(1);
                            fake.Bar(new Generic<bool>());
                            using (scope.OrderedAssertions())
                            {
                                A.CallTo(() => fake.Bar(A<Generic<bool>>.Ignored)).MustHaveHappened();
                                exception = Record.Exception(() => A.CallTo(() => fake.Bar(A<int>.Ignored)).MustHaveHappened());
                            }
                        }
                    });

            "it should tell us that the call was not matched"
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

        public class Generic<T>
        {
        }
    }
}
