namespace FakeItEasy.Specs
{
    using System;
    using FakeItEasy.Configuration;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using LambdaTale;
    using Xunit;

    public static class OrderedCallMatchingSpecs
    {
        public interface IFoo
        {
            void Bar(int baz);
        }

        public interface ISomething
        {
            void SomethingMethod();
        }

        public interface ISomethingBaz : ISomething
        {
            void BazMethod();
        }

        public interface ISomethingQux : ISomething
        {
            void QuxMethod();
        }

        [Scenario]
        public static void OrderedAssertionsInOrder(IFoo fake, Exception exception)
        {
            "Given a Fake"
                .x(() => fake = A.Fake<IFoo>());

            "And a call on the Fake, passing argument 1"
                .x(() => fake.Bar(1));

            "And a call on the Fake, passing argument 1"
                .x(() => fake.Bar(1));

            "And a call on the Fake, passing argument 2"
                .x(() => fake.Bar(2));

            "And a call on the Fake, passing argument 3"
                .x(() => fake.Bar(3));

            "When I assert that a call with argument 1 was made twice exactly, then a call with argument 2, and then a call with argument 3"
                .x(() => exception = Record.Exception(() =>
                    A.CallTo(() => fake.Bar(1)).MustHaveHappenedTwiceExactly()
                        .Then(A.CallTo(() => fake.Bar(2)).MustHaveHappened())
                        .Then(A.CallTo(() => fake.Bar(3)).MustHaveHappened())));

            "Then the assertion should pass"
                .x(() => exception.Should().BeNull());
        }

        [Scenario]
        public static void OrderedAssertionsOutOfOrder(IFoo fake, Exception exception)
        {
            "Given a Fake"
                .x(() => fake = A.Fake<IFoo>());

            "And a call on the Fake, passing argument 3"
                .x(() => fake.Bar(3));

            "And a call on the Fake, passing argument 1"
                .x(() => fake.Bar(1));

            "And a call on the Fake, passing argument 1"
                .x(() => fake.Bar(1));

            "And a call on the Fake, passing argument 2"
                .x(() => fake.Bar(2));

            "When I assert that a call with argument 1 was made twice exactly, then a call with argument 2, and then a call with argument 3"
                .x(() => exception = Record.Exception(() =>
                    A.CallTo(() => fake.Bar(1)).MustHaveHappenedTwiceExactly()
                        .Then(A.CallTo(() => fake.Bar(2)).MustHaveHappened())
                        .Then(A.CallTo(() => fake.Bar(3)).MustHaveHappened())));

            "Then the assertion should fail"
                .x(() => exception.Should().BeAnExceptionOfType<ExpectationException>().WithMessageModuloLineEndings(@"

  Assertion failed for the following calls:
    'FakeItEasy.Specs.OrderedCallMatchingSpecs+IFoo.Bar(baz: 1)' twice exactly
    'FakeItEasy.Specs.OrderedCallMatchingSpecs+IFoo.Bar(baz: 2)' once or more
    'FakeItEasy.Specs.OrderedCallMatchingSpecs+IFoo.Bar(baz: 3)' once or more
  The calls were found but not in the correct order among the calls:
    1: FakeItEasy.Specs.OrderedCallMatchingSpecs+IFoo.Bar(baz: 3)
    2: FakeItEasy.Specs.OrderedCallMatchingSpecs+IFoo.Bar(baz: 1) 2 times
    ...
    4: FakeItEasy.Specs.OrderedCallMatchingSpecs+IFoo.Bar(baz: 2)"));
        }

        [Scenario]
        public static void OrderedAssertionsOnDifferentObjectsInOrder(IFoo fake1, IFoo fake2, Exception exception)
        {
            "Given a Fake"
                .x(() => fake1 = A.Fake<IFoo>());

            "And another Fake of the same type"
                .x(() => fake2 = A.Fake<IFoo>());

            "And a call on the first Fake, passing argument 1"
                .x(() => fake1.Bar(1));

            "And a call on the second Fake, passing argument 1"
                .x(() => fake2.Bar(1));

            "And a call on the first Fake, passing argument 2"
                .x(() => fake1.Bar(2));

            "When I assert that a call with argument 1 was made on the first Fake, then on the second, and then that a call with argument 2 was made on the first Fake"
                .x(() => exception = Record.Exception(() =>
                    A.CallTo(() => fake1.Bar(1)).MustHaveHappened()
                        .Then(A.CallTo(() => fake2.Bar(1)).MustHaveHappened())
                        .Then(A.CallTo(() => fake1.Bar(2)).MustHaveHappened())));

            "Then the assertion should pass"
                .x(() => exception.Should().BeNull());
        }

        [Scenario]
        public static void OrderedAssertionsOnDifferentObjectsOutOfOrder(IFoo fake1, IFoo fake2, Exception exception)
        {
            "Given a Fake"
                .x(() => fake1 = A.Fake<IFoo>());

            "And another Fake of the same type"
                .x(() => fake2 = A.Fake<IFoo>());

            "And a call on the second Fake, passing argument 1"
                .x(() => fake2.Bar(1));

            "And a call on the first Fake, passing argument 1"
                .x(() => fake1.Bar(1));

            "And a call on the first Fake, passing argument 2"
                .x(() => fake1.Bar(2));

            "When I assert that a call with argument 1 was made on the first Fake, then on the second, and then that a call with argument 2 was made on the first Fake"
                .x(() => exception = Record.Exception(() =>
                    A.CallTo(() => fake1.Bar(1)).MustHaveHappened()
                        .Then(A.CallTo(() => fake2.Bar(1)).MustHaveHappened())
                        .Then(A.CallTo(() => fake1.Bar(2)).MustHaveHappened())));

            "Then the assertion should fail"
                .x(() => exception.Should().BeAnExceptionOfType<ExpectationException>().WithMessageModuloLineEndings(@"

  Assertion failed for the following calls:
    'FakeItEasy.Specs.OrderedCallMatchingSpecs+IFoo.Bar(baz: 1)' once or more
    'FakeItEasy.Specs.OrderedCallMatchingSpecs+IFoo.Bar(baz: 1)' once or more
  The calls were found but not in the correct order among the calls:
    1: FakeItEasy.Specs.OrderedCallMatchingSpecs+IFoo.Bar(baz: 1)
    2: FakeItEasy.Specs.OrderedCallMatchingSpecs+IFoo.Bar(baz: 1)
    3: FakeItEasy.Specs.OrderedCallMatchingSpecs+IFoo.Bar(baz: 2)
"));
        }

        [Scenario]
        public static void OrderedAssertionsOnDifferentNamedObjectsOutOfOrder(IFoo fake1, IFoo fake2, Exception exception)
        {
            "Given a named Fake"
                .x(() => fake1 = A.Fake<IFoo>(o => o.Named("Foo1")));

            "And another named Fake of the same type"
                .x(() => fake2 = A.Fake<IFoo>(o => o.Named("Foo2")));

            "And a call on the second Fake, passing argument 1"
                .x(() => fake2.Bar(1));

            "And a call on the first Fake, passing argument 1"
                .x(() => fake1.Bar(1));

            "And a call on the first Fake, passing argument 2"
                .x(() => fake1.Bar(2));

            "When I assert that a call with argument 1 was made on the first Fake, then on the second, and then that a call with argument 2 was made on the first Fake"
                .x(() => exception = Record.Exception(() =>
                    A.CallTo(() => fake1.Bar(1)).MustHaveHappened()
                        .Then(A.CallTo(() => fake2.Bar(1)).MustHaveHappened())
                        .Then(A.CallTo(() => fake1.Bar(2)).MustHaveHappened())));

            "Then the assertion should fail"
                .x(() => exception.Should().BeAnExceptionOfType<ExpectationException>().WithMessageModuloLineEndings(@"

  Assertion failed for the following calls:
    'FakeItEasy.Specs.OrderedCallMatchingSpecs+IFoo.Bar(baz: 1) on Foo1' once or more
    'FakeItEasy.Specs.OrderedCallMatchingSpecs+IFoo.Bar(baz: 1) on Foo2' once or more
  The calls were found but not in the correct order among the calls:
    1: FakeItEasy.Specs.OrderedCallMatchingSpecs+IFoo.Bar(baz: 1) on Foo2
    2: FakeItEasy.Specs.OrderedCallMatchingSpecs+IFoo.Bar(baz: 1) on Foo1
    3: FakeItEasy.Specs.OrderedCallMatchingSpecs+IFoo.Bar(baz: 2) on Foo1
"));
        }

        [Scenario]
        public static void MultistepOrderedAssertionsInOrder(
            IFoo fake,
            Exception exception,
            IOrderableCallAssertion lastAssertion)
        {
            "Given a Fake"
                .x(() => fake = A.Fake<IFoo>());

            "And a call on the Fake, passing argument 1"
                .x(() => fake.Bar(1));

            "And a call on the Fake, passing argument 2"
                .x(() => fake.Bar(2));

            "And a call on the Fake, passing argument 2"
                .x(() => fake.Bar(2));

            "And a call on the Fake, passing argument 3"
                .x(() => fake.Bar(3));

            "When I assert that a call with argument 1 was made once exactly"
                .x(() => lastAssertion = A.CallTo(() => fake.Bar(1)).MustHaveHappenedOnceExactly());

            "And then a call with argument 2"
                .x(() => lastAssertion = lastAssertion.Then(A.CallTo(() => fake.Bar(2)).MustHaveHappened()));

            "And then a call with argument 3 once exactly"
                .x(() => exception = Record.Exception(() => lastAssertion.Then(A.CallTo(() => fake.Bar(3)).MustHaveHappenedOnceExactly())));

            "Then the assertions should pass"
                .x(() => exception.Should().BeNull());
        }

        [Scenario]
        public static void MultistepOrderedAssertionsOutOfOrder(
            IFoo fake,
            Exception exception,
            IOrderableCallAssertion lastAssertion)
        {
            "Given a Fake"
                .x(() => fake = A.Fake<IFoo>());

            "And a call on the Fake, passing argument 3"
                .x(() => fake.Bar(3));

            "And a call on the Fake, passing argument 1"
                .x(() => fake.Bar(1));

            "And a call on the Fake, passing argument 1"
                .x(() => fake.Bar(1));

            "And a call on the Fake, passing argument 2"
                .x(() => fake.Bar(2));

            "When I assert that a call with argument 1 was made twice exactly"
                .x(() => lastAssertion = A.CallTo(() => fake.Bar(1)).MustHaveHappenedTwiceExactly());

            "And then a call with argument 2"
                .x(() => lastAssertion = lastAssertion.Then(A.CallTo(() => fake.Bar(2)).MustHaveHappened()));

            "And then that a call with argument 3 was made once exactly"
                .x(() => exception = Record.Exception(() => lastAssertion.Then(A.CallTo(() => fake.Bar(3)).MustHaveHappenedOnceExactly())));

            "Then the last assertion should fail"
                .x(() => exception.Should().BeAnExceptionOfType<ExpectationException>().WithMessageModuloLineEndings(@"

  Assertion failed for the following calls:
    'FakeItEasy.Specs.OrderedCallMatchingSpecs+IFoo.Bar(baz: 1)' twice exactly
    'FakeItEasy.Specs.OrderedCallMatchingSpecs+IFoo.Bar(baz: 2)' once or more
    'FakeItEasy.Specs.OrderedCallMatchingSpecs+IFoo.Bar(baz: 3)' once exactly
  The calls were found but not in the correct order among the calls:
    1: FakeItEasy.Specs.OrderedCallMatchingSpecs+IFoo.Bar(baz: 3)
    2: FakeItEasy.Specs.OrderedCallMatchingSpecs+IFoo.Bar(baz: 1) 2 times
    ...
    4: FakeItEasy.Specs.OrderedCallMatchingSpecs+IFoo.Bar(baz: 2)"));
        }

        // This is perhaps not the most intuitive behavior, but has been
        // in place since ordered assertions were introduced: the
        // MustHaveHappened is executed first and checks all calls.
        [Scenario]
        public static void OrderedAssertionWithCallCountConstraintFailure(
            IFoo fake,
            IOrderableCallAssertion lastAssertion,
            Exception exception)
        {
            "Given a Fake"
                .x(() => fake = A.Fake<IFoo>());

            "And a call on the Fake, passing argument 1"
                .x(() => fake.Bar(1));

            "And a call on the Fake, passing argument 2"
                .x(() => fake.Bar(2));

            "And a call on the Fake, passing argument 1"
                .x(() => fake.Bar(1));

            "When I assert that a call with argument 2 was made"
                .x(() => lastAssertion = A.CallTo(() => fake.Bar(2)).MustHaveHappened());

            "And then that a call with argument 1 was made once exactly"
                .x(() => exception = Record.Exception(() => lastAssertion.Then(A.CallTo(() => fake.Bar(1)).MustHaveHappenedOnceExactly())));

            "Then the last assertion should fail"
                .x(() => exception.Should().BeAnExceptionOfType<ExpectationException>());

            "And the message should say that the call to Bar(1) was found too many times"
                .x(() => exception.Message.Should().BeModuloLineEndings(@"

  Assertion failed for the following call:
    FakeItEasy.Specs.OrderedCallMatchingSpecs+IFoo.Bar(baz: 1)
  Expected to find it once exactly but found it twice among the calls:
    1: FakeItEasy.Specs.OrderedCallMatchingSpecs+IFoo.Bar(baz: 1)
    2: FakeItEasy.Specs.OrderedCallMatchingSpecs+IFoo.Bar(baz: 2)
    3: FakeItEasy.Specs.OrderedCallMatchingSpecs+IFoo.Bar(baz: 1)

"));
        }

        // Reported as issue 182 (https://github.com/FakeItEasy/FakeItEasy/issues/182).
        [Scenario]
        public static void OrderedAssertionOnInterfacesWithCommonParent(ISomethingBaz baz, ISomethingQux qux, IOrderableCallAssertion lastAssertion, Exception exception)
        {
            "Given a Fake baz"
                .x(() => baz = A.Fake<ISomethingBaz>());

            "And a Fake qux implementing an interface in common with baz"
                .x(() => qux = A.Fake<ISomethingQux>());

            "And a call on qux"
                .x(() => qux.QuxMethod());

            "And a call on baz"
                .x(() => baz.BazMethod());

            "When I assert that the qux call was made"
                .x(() => lastAssertion = A.CallTo(() => qux.QuxMethod()).MustHaveHappened());

            "And I make the same assertion again"
                .x(() => exception = Record.Exception(() => lastAssertion.Then(A.CallTo(() => qux.QuxMethod()).MustHaveHappened())));

            "Then the second assertion should fail"
                .x(() => exception.Should().BeAnExceptionOfType<ExpectationException>());
        }

        // Reported as issue 1583 (https://github.com/FakeItEasy/FakeItEasy/issues/1583).
        [Scenario]
        public static void OrderedAssertionOnCallThatCallsAnotherFakedMethod(
            ClassWithAMethodThatCallsASibling fake,
            IOrderableCallAssertion lastAssertion,
            Exception exception)
        {
            "Given a class with a method that calls another class method"
                .See<ClassWithAMethodThatCallsASibling>();

            "And a Fake of that type, configured to call base methods"
                .x(() => fake = A.Fake<ClassWithAMethodThatCallsASibling>(options => options.CallsBaseMethods()));

            "And I call the outer method"
                .x(() => fake.OuterMethod());

            "When I assert that the outer method was called"
                .x(() => lastAssertion = A.CallTo(() => fake.OuterMethod()).MustHaveHappened());

            "And then I assert that the inner method was called afterward"
                .x(() => exception = Record.Exception(() =>
                    lastAssertion.Then(A.CallTo(() => fake.InnerMethod()).MustHaveHappened())));

            "Then the assertions pass"
                .x(() => exception.Should().BeNull());
        }

        public abstract class ClassWithAMethodThatCallsASibling
        {
            public virtual void OuterMethod() => this.InnerMethod();

            public abstract void InnerMethod();
        }
    }
}
