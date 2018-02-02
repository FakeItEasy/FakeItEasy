namespace FakeItEasy.Specs
{
    using System;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xbehave;
    using Xunit;

    public static class ThenSpecs
    {
        public interface IFoo
        {
            void Bar();

            void Bar(int value);

            int Baz();
        }

        [Scenario]
        public static void VoidMethodWithInvokes(
            IFoo foo,
            Action action1,
            Action action2)
        {
            "Given a fake"
                .x(() => foo = A.Fake<IFoo>());

            "And two delegates"
                .x(() =>
                {
                    action1 = A.Fake<Action>();
                    action2 = A.Fake<Action>();
                });

            "When a void method is configured to invoke a delegate once then invoke another delegate"
                .x(() =>
                    A.CallTo(() => foo.Bar()).Invokes(action1).DoesNothing().Once()
                        .Then.Invokes(action2));

            "And the method is called 3 times"
                .x(() =>
                {
                    foo.Bar();
                    foo.Bar();
                    foo.Bar();
                });

            "Then the first delegate is invoked once, and the second delegate is invoked twice"
                .x(() => A.CallTo(() => action1()).MustHaveHappenedOnceExactly()
                    .Then(A.CallTo(() => action2()).MustHaveHappenedTwiceExactly()));
        }

        [Scenario]
        public static void VoidMethodWithThrows(
            IFoo foo,
            Action action,
            Exception exception)
        {
            "Given a fake"
                .x(() => foo = A.Fake<IFoo>());

            "And an action"
                .x(() => action = A.Fake<Action>());

            "When a void method is configured to invoke a delegate twice then throw an exception"
                .x(() =>
                    A.CallTo(() => foo.Bar()).Invokes(action).DoesNothing().Twice()
                        .Then.Throws<InvalidOperationException>());

            "And the method is called 3 times"
                .x(() =>
                {
                    foo.Bar();
                    foo.Bar();
                    exception = Record.Exception(() => foo.Bar());
                });

            "Then the delegate is invoked twice"
                .x(() => A.CallTo(() => action()).MustHaveHappenedTwiceExactly());

            "And the third call throws an exception"
                .x(() => exception.Should().BeAnExceptionOfType<InvalidOperationException>());
        }

        [Scenario]
        public static void NonVoidMethodWithReturns(
            IFoo foo,
            int result1,
            int result2,
            int result3)
        {
            "Given a fake"
                .x(() => foo = A.Fake<IFoo>());

            "When a non-void method is configured to return 1 once then return 2"
                .x(() =>
                    A.CallTo(() => foo.Baz()).Returns(1).Once()
                        .Then.Returns(2));

            "And the method is called 3 times"
                .x(() =>
                {
                    result1 = foo.Baz();
                    result2 = foo.Baz();
                    result3 = foo.Baz();
                });

            "Then the first call returns 1"
                .x(() => result1.Should().Be(1));

            "And the second call returns 2"
                .x(() => result2.Should().Be(2));

            "And the third call returns 2"
                .x(() => result3.Should().Be(2));
        }

        [Scenario]
        public static void AnyCallWithInvokesAndThrows(
            IFoo foo,
            Action action1,
            Action action2,
            Exception exception)
        {
            "Given a fake"
                .x(() => foo = A.Fake<IFoo>());

            "And two delegates"
                .x(() =>
                {
                    action1 = A.Fake<Action>();
                    action2 = A.Fake<Action>();
                });

            "When any method is configured to invoke a delegate once, then another delegate twice, then throw an exception"
                .x(() =>
                    A.CallTo(foo).Invokes(action1).DoesNothing().Once()
                        .Then.Invokes(action2).DoesNothing().Twice()
                        .Then.Throws<InvalidOperationException>());

            "And 4 calls are made on the fake"
                .x(() =>
                {
                    foo.Bar();
                    foo.Bar(1);
                    foo.Bar(2);
                    exception = Record.Exception(() => foo.Bar(3));
                });

            "Then the first delegate is invoked once, and the second delegate is invoked twice"
                .x(() => A.CallTo(() => action1()).MustHaveHappenedOnceExactly()
                    .Then(A.CallTo(() => action2()).MustHaveHappenedTwiceExactly()));

            "And the fourth call throws an exception"
                .x(() => exception.Should().BeAnExceptionOfType<InvalidOperationException>());
        }

        [Scenario]
        public static void NonVoidMethodOverrideWithThen(
            IFoo foo,
            int result1,
            int result2,
            int result3)
        {
            "Given a fake"
                .x(() => foo = A.Fake<IFoo>());

            "When a non-void method is configured to return 1"
                .x(() => A.CallTo(() => foo.Baz()).Returns(1));

            "And the configuration is overridden to return 2 once then 3 once"
                .x(() => A.CallTo(() => foo.Baz()).Returns(2).Once()
                    .Then.Returns(3).Once());

            "And the method is called 3 times"
                .x(() =>
                {
                    result1 = foo.Baz();
                    result2 = foo.Baz();
                    result3 = foo.Baz();
                });

            "Then the first call returns 2"
                .x(() => result1.Should().Be(2));

            "And the second call returns 3"
                .x(() => result2.Should().Be(3));

            "And the third call returns 1"
                .x(() => result3.Should().Be(1));
        }

        [Scenario]
        public static void NonVoidMethodThenAndOverride(
            IFoo foo,
            int result1,
            int result2,
            int result3)
        {
            "Given a fake"
                .x(() => foo = A.Fake<IFoo>());

            "When a non-void is configured to return 2 once then 3 once"
                .x(() => A.CallTo(() => foo.Baz()).Returns(2).Once()
                    .Then.Returns(3).Once());

            "And the configuration is overridden to return 1 once"
                .x(() => A.CallTo(() => foo.Baz()).Returns(1).Once());

            "And the method is called 3 times"
                .x(() =>
                {
                    result1 = foo.Baz();
                    result2 = foo.Baz();
                    result3 = foo.Baz();
                });

            "Then the first call returns 1"
                .x(() => result1.Should().Be(1));

            "And the second call returns 2"
                .x(() => result2.Should().Be(2));

            "And the third call returns 3"
                .x(() => result3.Should().Be(3));
        }
    }
}
