namespace FakeItEasy.Specs
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using FakeItEasy.Configuration;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xbehave;
    using Xunit;

    public static class AssertingCallCountSpecs
    {
        public interface IFoo
        {
            void Method();

            int Property { get; set; }
        }

        public class CallCountAsserter
        {
            public int NumberOfCalls { get; }

            private readonly Expression<Action<IAssertConfiguration>> assertion;

            public string AssertionDescription => this.assertion.Body.ToString();

            public string AssertionError { get; }

            public CallCountAsserter(int numberOfCalls, Expression<Action<IAssertConfiguration>> assertion, string assertionError = null)
            {
                this.NumberOfCalls = numberOfCalls;
                this.assertion = assertion;
                this.AssertionError = assertionError;
            }

            public void AssertCallCount(IAssertConfiguration configuration)
            {
                this.assertion.Compile().Invoke(configuration);
            }

            public override string ToString()
            {
                return $"{this.NumberOfCalls} calls, asserting {this.AssertionDescription}";
            }
        }

        public class CountableCall
        {
            private readonly Func<IFoo, IAssertConfiguration> callSpecifier;
            private readonly string description;
            private readonly Delegate invocation;

            public CountableCall(Expression<Func<IFoo, IAssertConfiguration>> callSpecifier, Action<IFoo> invocation)
            {
                this.callSpecifier = callSpecifier.Compile();
                this.description = callSpecifier.Body.ToString();
                this.invocation = invocation;
            }

            public CountableCall(Expression<Func<IFoo, IAssertConfiguration>> callSpecifier, Func<IFoo, int> invocation)
            {
                this.callSpecifier = callSpecifier.Compile();
                this.description = callSpecifier.Body.ToString();
                this.invocation = invocation;
            }

            public override string ToString() => this.description;

            public IAssertConfiguration BeginAssertion(IFoo fake)
            {
                return this.callSpecifier.Invoke(fake);
            }

            public void Invoke(IFoo fake)
            {
                this.invocation.DynamicInvoke(fake);
            }
        }

        [Scenario]
        [MemberData(nameof(MatchingTestCases))]
        public static void CallCountConstraintWithMatchingNumberOfCalls(CountableCall call, CallCountAsserter callCountAsserter, IFoo fake, Exception exception)
        {
            "Given a Fake"
                .x(() => fake = A.Fake<IFoo>());

            $"And I make {callCountAsserter.NumberOfCalls} calls to the fake"
                .x(() =>
                {
                    for (int i = 0; i < callCountAsserter.NumberOfCalls; ++i)
                    {
                        call.Invoke(fake);
                    }
                });

            $"When I assert {callCountAsserter.AssertionDescription}"
                .x(() => exception = Record.Exception(() => callCountAsserter.AssertCallCount(call.BeginAssertion(fake))));

            "Then the assertion passes"
                .x(() => exception.Should().BeNull());
        }

        [Scenario]
        [MemberData(nameof(NonMatchingTestCases))]
        public static void CallCountConstraintWithNonMatchingNumberOfCalls(CountableCall call, CallCountAsserter callCountAsserter, IFoo fake, Exception exception)
        {
            "Given a Fake"
                .x(() => fake = A.Fake<IFoo>());

            $"And I make {callCountAsserter.NumberOfCalls} calls to the fake"
                .x(() =>
                {
                    for (int i = 0; i < callCountAsserter.NumberOfCalls; ++i)
                    {
                        call.Invoke(fake);
                    }
                });

            $"When I assert {callCountAsserter.AssertionDescription}"
                .x(() => exception = Record.Exception(() => callCountAsserter.AssertCallCount(call.BeginAssertion(fake))));

            "Then the assertion fails"
                .x(() => exception.Should().BeAnExceptionOfType<ExpectationException>()
                    .WithMessage($"*\r\nExpected to find it {callCountAsserter.AssertionError}*"));
        }

        private static IEnumerable<object[]> MatchingTestCases()
        {
            foreach (var call in AllCountableCalls())
            {
                foreach (var assertion in MatchingAssertions())
                {
                    yield return new object[] { call, assertion };
                }
            }
        }

        private static IEnumerable<object[]> NonMatchingTestCases()
        {
            foreach (var call in AllCountableCalls())
            {
                foreach (var assertion in NonMatchingAssertions())
                {
                    yield return new object[] { call, assertion };
                }
            }
        }

        private static IEnumerable<CountableCall> AllCountableCalls()
        {
            return new[]
            {
                new CountableCall(fake => A.CallTo(() => fake.Method()), fake => fake.Method()),
                new CountableCall(fake => A.CallTo(fake), fake => fake.Method()),
                new CountableCall(fake => A.CallTo(fake).WithNonVoidReturnType(), fake => fake.Property),
                new CountableCall(fake => A.CallToSet(() => fake.Property), fake => fake.Property = 3),
                new CountableCall(fake => A.CallToSet(() => fake.Property).To(7), fake => fake.Property = 7)
            };
        }

        private static IEnumerable<CallCountAsserter> MatchingAssertions()
        {
            return new[]
            {
                new CallCountAsserter(1, call => call.MustHaveHappened()),
                new CallCountAsserter(2, call => call.MustHaveHappened()),
                new CallCountAsserter(0, call => call.MustNotHaveHappened()),
                new CallCountAsserter(1, call => call.MustHaveHappenedOnceExactly()),
                new CallCountAsserter(1, call => call.MustHaveHappenedOnceOrMore()),
                new CallCountAsserter(2, call => call.MustHaveHappenedOnceOrMore()),
                new CallCountAsserter(0, call => call.MustHaveHappenedOnceOrLess()),
                new CallCountAsserter(1, call => call.MustHaveHappenedOnceOrLess()),
                new CallCountAsserter(2, call => call.MustHaveHappenedTwiceExactly()),
                new CallCountAsserter(2, call => call.MustHaveHappenedTwiceOrMore()),
                new CallCountAsserter(3, call => call.MustHaveHappenedTwiceOrMore()),
                new CallCountAsserter(0, call => call.MustHaveHappenedTwiceOrLess()),
                new CallCountAsserter(1, call => call.MustHaveHappenedTwiceOrLess()),
                new CallCountAsserter(2, call => call.MustHaveHappenedTwiceOrLess()),
                new CallCountAsserter(0, call => call.MustHaveHappened(0, Times.Exactly)),
                new CallCountAsserter(0, call => call.MustHaveHappened(0, Times.OrMore)),
                new CallCountAsserter(1, call => call.MustHaveHappened(0, Times.OrMore)),
                new CallCountAsserter(0, call => call.MustHaveHappened(0, Times.OrLess)),
                new CallCountAsserter(1, call => call.MustHaveHappened(1, Times.Exactly)),
                new CallCountAsserter(1, call => call.MustHaveHappened(1, Times.OrMore)),
                new CallCountAsserter(2, call => call.MustHaveHappened(1, Times.OrMore)),
                new CallCountAsserter(0, call => call.MustHaveHappened(1, Times.OrLess)),
                new CallCountAsserter(1, call => call.MustHaveHappened(1, Times.OrLess)),
                new CallCountAsserter(2, call => call.MustHaveHappened(2, Times.Exactly)),
                new CallCountAsserter(2, call => call.MustHaveHappened(2, Times.OrMore)),
                new CallCountAsserter(3, call => call.MustHaveHappened(2, Times.OrMore)),
                new CallCountAsserter(0, call => call.MustHaveHappened(2, Times.OrLess)),
                new CallCountAsserter(1, call => call.MustHaveHappened(2, Times.OrLess)),
                new CallCountAsserter(2, call => call.MustHaveHappened(2, Times.OrLess)),
                new CallCountAsserter(3, call => call.MustHaveHappened(3, Times.Exactly)),
                new CallCountAsserter(3, call => call.MustHaveHappened(3, Times.OrMore)),
                new CallCountAsserter(4, call => call.MustHaveHappened(3, Times.OrMore)),
                new CallCountAsserter(0, call => call.MustHaveHappened(3, Times.OrLess)),
                new CallCountAsserter(1, call => call.MustHaveHappened(3, Times.OrLess)),
                new CallCountAsserter(2, call => call.MustHaveHappened(3, Times.OrLess)),
                new CallCountAsserter(3, call => call.MustHaveHappened(3, Times.OrLess)),
                new CallCountAsserter(0, call => call.MustHaveHappenedANumberOfTimesMatching(n => n % 2 == 0)),
                new CallCountAsserter(2, call => call.MustHaveHappenedANumberOfTimesMatching(n => n % 2 == 0)),
                new CallCountAsserter(3, call => call.MustHaveHappenedANumberOfTimesMatching(n => n % 3 == 0))
            };
        }

        private static IEnumerable<CallCountAsserter> NonMatchingAssertions()
        {
            return new[]
            {
                new CallCountAsserter(0, call => call.MustHaveHappened(), "once or more but no calls were made"),
                new CallCountAsserter(1, call => call.MustNotHaveHappened(), "never but found it once"),
                new CallCountAsserter(2, call => call.MustNotHaveHappened(), "never but found it twice"),
                new CallCountAsserter(3, call => call.MustNotHaveHappened(), "never but found it 3 times"),
                new CallCountAsserter(0, call => call.MustHaveHappenedOnceExactly(), "once exactly but no calls were made"),
                new CallCountAsserter(2, call => call.MustHaveHappenedOnceExactly(), "once exactly but found it twice"),
                new CallCountAsserter(3, call => call.MustHaveHappenedOnceExactly(), "once exactly but found it 3 times"),
                new CallCountAsserter(0, call => call.MustHaveHappenedOnceOrMore(), "once or more but no calls were made"),
                new CallCountAsserter(2, call => call.MustHaveHappenedOnceOrLess(), "once or less but found it twice"),
                new CallCountAsserter(3, call => call.MustHaveHappenedOnceOrLess(), "once or less but found it 3 times"),
                new CallCountAsserter(0, call => call.MustHaveHappenedTwiceExactly(), "twice exactly but no calls were made"),
                new CallCountAsserter(1, call => call.MustHaveHappenedTwiceExactly(), "twice exactly but found it once"),
                new CallCountAsserter(3, call => call.MustHaveHappenedTwiceExactly(), "twice exactly but found it 3 times"),
                new CallCountAsserter(0, call => call.MustHaveHappenedTwiceOrMore(), "twice or more but no calls were made"),
                new CallCountAsserter(1, call => call.MustHaveHappenedTwiceOrMore(), "twice or more but found it once"),
                new CallCountAsserter(3, call => call.MustHaveHappenedTwiceOrLess(), "twice or less but found it 3 times"),
                new CallCountAsserter(1, call => call.MustHaveHappened(0, Times.Exactly), "never but found it once"),
                new CallCountAsserter(2, call => call.MustHaveHappened(0, Times.Exactly), "never but found it twice"),
                new CallCountAsserter(3, call => call.MustHaveHappened(0, Times.Exactly), "never but found it 3 times"),
                new CallCountAsserter(1, call => call.MustHaveHappened(0, Times.OrLess), "never but found it once"),
                new CallCountAsserter(2, call => call.MustHaveHappened(0, Times.OrLess), "never but found it twice"),
                new CallCountAsserter(3, call => call.MustHaveHappened(0, Times.OrLess), "never but found it 3 times"),
                new CallCountAsserter(0, call => call.MustHaveHappened(1, Times.Exactly), "once exactly but no calls"),
                new CallCountAsserter(2, call => call.MustHaveHappened(1, Times.Exactly), "once exactly but found it twice"),
                new CallCountAsserter(3, call => call.MustHaveHappened(1, Times.Exactly), "once exactly but found it 3 times"),
                new CallCountAsserter(0, call => call.MustHaveHappened(1, Times.OrMore), "once or more but no calls were made"),
                new CallCountAsserter(2, call => call.MustHaveHappened(1, Times.OrLess), "once or less but found it twice"),
                new CallCountAsserter(3, call => call.MustHaveHappened(1, Times.OrLess), "once or less but found it 3 times"),
                new CallCountAsserter(0, call => call.MustHaveHappened(2, Times.Exactly), "twice exactly but no calls"),
                new CallCountAsserter(1, call => call.MustHaveHappened(2, Times.Exactly), "twice exactly but found it once"),
                new CallCountAsserter(3, call => call.MustHaveHappened(2, Times.Exactly), "twice exactly but found it 3 times"),
                new CallCountAsserter(0, call => call.MustHaveHappened(2, Times.OrMore), "twice or more but no calls were made"),
                new CallCountAsserter(1, call => call.MustHaveHappened(2, Times.OrMore), "twice or more but found it once"),
                new CallCountAsserter(3, call => call.MustHaveHappened(2, Times.OrLess), "twice or less but found it 3 times"),
                new CallCountAsserter(0, call => call.MustHaveHappened(3, Times.Exactly), "3 times exactly but no calls were made"),
                new CallCountAsserter(1, call => call.MustHaveHappened(3, Times.Exactly), "3 times exactly but found it once"),
                new CallCountAsserter(2, call => call.MustHaveHappened(3, Times.Exactly), "3 times exactly but found it twice"),
                new CallCountAsserter(4, call => call.MustHaveHappened(3, Times.Exactly), "3 times exactly but found it 4 times"),
                new CallCountAsserter(0, call => call.MustHaveHappened(3, Times.OrMore), "3 times or more but no calls were made"),
                new CallCountAsserter(1, call => call.MustHaveHappened(3, Times.OrMore), "3 times or more but found it once"),
                new CallCountAsserter(2, call => call.MustHaveHappened(3, Times.OrMore), "3 times or more but found it twice"),
                new CallCountAsserter(4, call => call.MustHaveHappened(3, Times.OrLess), "3 times or less but found it 4 times"),
                new CallCountAsserter(0, call => call.MustHaveHappenedANumberOfTimesMatching(n => n % 2 == 1), "a number of times matching the predicate 'n => ((n % 2) == 1)' but no calls were made"),
                new CallCountAsserter(1, call => call.MustHaveHappenedANumberOfTimesMatching(n => n % 2 == 0), "a number of times matching the predicate 'n => ((n % 2) == 0)' but found it once"),
                new CallCountAsserter(2, call => call.MustHaveHappenedANumberOfTimesMatching(n => n % 2 == 1), "a number of times matching the predicate 'n => ((n % 2) == 1)' but found it twice"),
                new CallCountAsserter(3, call => call.MustHaveHappenedANumberOfTimesMatching(n => n % 2 == 0), "a number of times matching the predicate 'n => ((n % 2) == 0)' but found it 3 times")
            };
        }
    }
}
