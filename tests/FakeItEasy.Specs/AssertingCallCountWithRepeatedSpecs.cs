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

    public static class AssertingCallCountWithRepeatedSpecs
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
                new CallCountAsserter(1, call => call.MustHaveHappened(Repeated.Exactly.Once)),
                new CallCountAsserter(1, call => call.MustHaveHappened(Repeated.AtLeast.Once)),
                new CallCountAsserter(2, call => call.MustHaveHappened(Repeated.AtLeast.Once)),
                new CallCountAsserter(0, call => call.MustHaveHappened(Repeated.NoMoreThan.Once)),
                new CallCountAsserter(1, call => call.MustHaveHappened(Repeated.NoMoreThan.Once)),
                new CallCountAsserter(2, call => call.MustHaveHappened(Repeated.Exactly.Twice)),
                new CallCountAsserter(2, call => call.MustHaveHappened(Repeated.AtLeast.Twice)),
                new CallCountAsserter(3, call => call.MustHaveHappened(Repeated.AtLeast.Twice)),
                new CallCountAsserter(0, call => call.MustHaveHappened(Repeated.NoMoreThan.Twice)),
                new CallCountAsserter(1, call => call.MustHaveHappened(Repeated.NoMoreThan.Twice)),
                new CallCountAsserter(2, call => call.MustHaveHappened(Repeated.NoMoreThan.Twice)),
                new CallCountAsserter(0, call => call.MustHaveHappened(Repeated.Exactly.Times(0))),
                new CallCountAsserter(0, call => call.MustHaveHappened(Repeated.AtLeast.Times(0))),
                new CallCountAsserter(1, call => call.MustHaveHappened(Repeated.AtLeast.Times(0))),
                new CallCountAsserter(0, call => call.MustHaveHappened(Repeated.NoMoreThan.Times(0))),
                new CallCountAsserter(1, call => call.MustHaveHappened(Repeated.Exactly.Times(1))),
                new CallCountAsserter(1, call => call.MustHaveHappened(Repeated.AtLeast.Times(1))),
                new CallCountAsserter(2, call => call.MustHaveHappened(Repeated.AtLeast.Times(1))),
                new CallCountAsserter(0, call => call.MustHaveHappened(Repeated.NoMoreThan.Times(1))),
                new CallCountAsserter(1, call => call.MustHaveHappened(Repeated.NoMoreThan.Times(1))),
                new CallCountAsserter(2, call => call.MustHaveHappened(Repeated.Exactly.Times(2))),
                new CallCountAsserter(2, call => call.MustHaveHappened(Repeated.AtLeast.Times(2))),
                new CallCountAsserter(3, call => call.MustHaveHappened(Repeated.AtLeast.Times(2))),
                new CallCountAsserter(0, call => call.MustHaveHappened(Repeated.NoMoreThan.Times(2))),
                new CallCountAsserter(1, call => call.MustHaveHappened(Repeated.NoMoreThan.Times(2))),
                new CallCountAsserter(2, call => call.MustHaveHappened(Repeated.NoMoreThan.Times(2))),
                new CallCountAsserter(3, call => call.MustHaveHappened(Repeated.Exactly.Times(3))),
                new CallCountAsserter(3, call => call.MustHaveHappened(Repeated.AtLeast.Times(3))),
                new CallCountAsserter(4, call => call.MustHaveHappened(Repeated.AtLeast.Times(3))),
                new CallCountAsserter(0, call => call.MustHaveHappened(Repeated.NoMoreThan.Times(3))),
                new CallCountAsserter(1, call => call.MustHaveHappened(Repeated.NoMoreThan.Times(3))),
                new CallCountAsserter(2, call => call.MustHaveHappened(Repeated.NoMoreThan.Times(3))),
                new CallCountAsserter(3, call => call.MustHaveHappened(Repeated.NoMoreThan.Times(3))),
                new CallCountAsserter(0, call => call.MustHaveHappened(Repeated.Like(n => n % 2 == 0))),
                new CallCountAsserter(2, call => call.MustHaveHappened(Repeated.Like(n => n % 2 == 0))),
                new CallCountAsserter(3, call => call.MustHaveHappened(Repeated.Like(n => n % 3 == 0)))
            };
        }

        private static IEnumerable<CallCountAsserter> NonMatchingAssertions()
        {
            return new[]
            {
                new CallCountAsserter(0, call => call.MustHaveHappened(Repeated.Exactly.Once), "exactly once but no calls were made"),
                new CallCountAsserter(2, call => call.MustHaveHappened(Repeated.Exactly.Once), "exactly once but found it twice"),
                new CallCountAsserter(3, call => call.MustHaveHappened(Repeated.Exactly.Once), "exactly once but found it 3 times"),
                new CallCountAsserter(0, call => call.MustHaveHappened(Repeated.AtLeast.Once), "at least once but no calls were made"),
                new CallCountAsserter(2, call => call.MustHaveHappened(Repeated.NoMoreThan.Once), "no more than once but found it twice"),
                new CallCountAsserter(3, call => call.MustHaveHappened(Repeated.NoMoreThan.Once), "no more than once but found it 3 times"),
                new CallCountAsserter(0, call => call.MustHaveHappened(Repeated.Exactly.Twice), "exactly twice but no calls were made"),
                new CallCountAsserter(1, call => call.MustHaveHappened(Repeated.Exactly.Twice), "exactly twice but found it once"),
                new CallCountAsserter(3, call => call.MustHaveHappened(Repeated.Exactly.Twice), "exactly twice but found it 3 times"),
                new CallCountAsserter(0, call => call.MustHaveHappened(Repeated.AtLeast.Twice), "at least twice but no calls were made"),
                new CallCountAsserter(1, call => call.MustHaveHappened(Repeated.AtLeast.Twice), "at least twice but found it once"),
                new CallCountAsserter(3, call => call.MustHaveHappened(Repeated.NoMoreThan.Twice), "no more than twice but found it 3 times"),
                new CallCountAsserter(1, call => call.MustHaveHappened(Repeated.Exactly.Times(0)), "exactly 0 times but found it once"),
                new CallCountAsserter(2, call => call.MustHaveHappened(Repeated.Exactly.Times(0)), "exactly 0 times but found it twice"),
                new CallCountAsserter(3, call => call.MustHaveHappened(Repeated.Exactly.Times(0)), "exactly 0 times but found it 3 times"),
                new CallCountAsserter(1, call => call.MustHaveHappened(Repeated.NoMoreThan.Times(0)), "no more than 0 times but found it once"),
                new CallCountAsserter(2, call => call.MustHaveHappened(Repeated.NoMoreThan.Times(0)), "no more than 0 times but found it twice"),
                new CallCountAsserter(3, call => call.MustHaveHappened(Repeated.NoMoreThan.Times(0)), "no more than 0 times but found it 3 times"),
                new CallCountAsserter(0, call => call.MustHaveHappened(Repeated.Exactly.Times(1)), "exactly 1 times but no calls"),
                new CallCountAsserter(2, call => call.MustHaveHappened(Repeated.Exactly.Times(1)), "exactly 1 times but found it twice"),
                new CallCountAsserter(3, call => call.MustHaveHappened(Repeated.Exactly.Times(1)), "exactly 1 times but found it 3 times"),
                new CallCountAsserter(0, call => call.MustHaveHappened(Repeated.AtLeast.Times(1)), "at least 1 times but no calls were made"),
                new CallCountAsserter(2, call => call.MustHaveHappened(Repeated.NoMoreThan.Times(1)), "no more than 1 times but found it twice"),
                new CallCountAsserter(3, call => call.MustHaveHappened(Repeated.NoMoreThan.Times(1)), "no more than 1 times but found it 3 times"),
                new CallCountAsserter(0, call => call.MustHaveHappened(Repeated.Exactly.Times(2)), "exactly 2 times but no calls"),
                new CallCountAsserter(1, call => call.MustHaveHappened(Repeated.Exactly.Times(2)), "exactly 2 times but found it once"),
                new CallCountAsserter(3, call => call.MustHaveHappened(Repeated.Exactly.Times(2)), "exactly 2 times but found it 3 times"),
                new CallCountAsserter(0, call => call.MustHaveHappened(Repeated.AtLeast.Times(2)), "at least 2 times but no calls were made"),
                new CallCountAsserter(1, call => call.MustHaveHappened(Repeated.AtLeast.Times(2)), "at least 2 times but found it once"),
                new CallCountAsserter(3, call => call.MustHaveHappened(Repeated.NoMoreThan.Times(2)), "no more than 2 times but found it 3 times"),
                new CallCountAsserter(0, call => call.MustHaveHappened(Repeated.Exactly.Times(3)), "exactly 3 times but no calls were made"),
                new CallCountAsserter(1, call => call.MustHaveHappened(Repeated.Exactly.Times(3)), "exactly 3 times but found it once"),
                new CallCountAsserter(2, call => call.MustHaveHappened(Repeated.Exactly.Times(3)), "exactly 3 times but found it twice"),
                new CallCountAsserter(4, call => call.MustHaveHappened(Repeated.Exactly.Times(3)), "exactly 3 times but found it 4 times"),
                new CallCountAsserter(0, call => call.MustHaveHappened(Repeated.AtLeast.Times(3)), "at least 3 times but no calls were made"),
                new CallCountAsserter(1, call => call.MustHaveHappened(Repeated.AtLeast.Times(3)), "at least 3 times but found it once"),
                new CallCountAsserter(2, call => call.MustHaveHappened(Repeated.AtLeast.Times(3)), "at least 3 times but found it twice"),
                new CallCountAsserter(4, call => call.MustHaveHappened(Repeated.NoMoreThan.Times(3)), "no more than 3 times but found it 4 times"),
                new CallCountAsserter(0, call => call.MustHaveHappened(Repeated.Like(n => n % 2 == 1)), "the number of times specified by the predicate 'n => ((n % 2) == 1)' but no calls were made"),
                new CallCountAsserter(1, call => call.MustHaveHappened(Repeated.Like(n => n % 2 == 0)), "the number of times specified by the predicate 'n => ((n % 2) == 0)' but found it once"),
                new CallCountAsserter(2, call => call.MustHaveHappened(Repeated.Like(n => n % 2 == 1)), "the number of times specified by the predicate 'n => ((n % 2) == 1)' but found it twice"),
                new CallCountAsserter(3, call => call.MustHaveHappened(Repeated.Like(n => n % 2 == 0)), "the number of times specified by the predicate 'n => ((n % 2) == 0)' but found it 3 times")
            };
        }
    }
}
