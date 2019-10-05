namespace FakeItEasy.Specs
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using FakeItEasy.Creation;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xbehave;
    using Xunit;

    public class UserCallbackExceptionSpecs
    {
        public interface IFoo
        {
            int Bar(int x);

            int Baz(HasCustomValueFormatter d);

            void OutAndRef(ref int x, out string s);
        }

        [SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces", Justification = "Required for testing.")]
        public interface IHasFakeOptionsBuilder
        {
        }

        [Scenario]
        public void ExceptionInArgumentConstraintExpression(IFoo fake, Exception exception)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And an argument constraint factory that throws an exception"
                .See(ThrowingConstraintFactory);

            "When a call to the fake is configured using that constraint factory"
                .x(() => exception = Record.Exception(() => A.CallTo(() => fake.Bar(ThrowingConstraintFactory())).Returns(42)));

            "Then a UserCallbackException should be thrown"
                .x(() => exception.Should().BeAnExceptionOfType<UserCallbackException>());

            "And its message should describe where the exception was thrown from"
                .x(() => exception.Message.Should().Be("Argument constraint expression <ThrowingConstraintFactory()> threw an exception. See inner exception for details."));

            "And the inner exception should be the original exception"
                .x(() => exception.InnerException.Should().BeAnExceptionOfType<MyException>().Which.Message.Should().Be("Oops"));
        }

        [Scenario]
        public void ExceptionInArgumentMatcher(IFoo fake, Exception exception)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And a call to the fake is configured with a custom argument matcher that throws an exception"
                .x(() => A.CallTo(() => fake.Bar(A<int>.That.Matches(i => ThrowException()))).Returns(42));

            "When the configured method is called"
                .x(() => exception = Record.Exception(() => fake.Bar(0)));

            "Then a UserCallbackException should be thrown"
                .x(() => exception.Should().BeAnExceptionOfType<UserCallbackException>());

            "And its message should describe where the exception was thrown from"
                .x(() => exception.Message.Should().Be("Argument matcher <i => ThrowException()> threw an exception. See inner exception for details."));

            "And the inner exception should be the original exception"
                .x(() => exception.InnerException.Should().BeAnExceptionOfType<MyException>().Which.Message.Should().Be("Oops"));
        }

        [Scenario]
        public void ExceptionInArgumentMatcherDescription(IFoo fake, Exception exception)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And no call to the fake is made"
                .x(() => { });

            "When an assertion is made with a custom argument matcher whose description throws an exception"
                .x(() => exception = Record.Exception(() => A.CallTo(() => fake.Bar(A<int>.That.Matches(i => i % 2 == 0, o => o.Write(ThrowException())))).MustHaveHappened()));

            "Then a UserCallbackException should be thrown"
                .x(() => exception.Should().BeAnExceptionOfType<UserCallbackException>());

            "And its message should describe where the exception was thrown from"
                .x(() => exception.Message.Should().Be("Argument matcher description threw an exception. See inner exception for details."));

            "And the inner exception should be the original exception"
                .x(() => exception.InnerException.Should().BeAnExceptionOfType<MyException>().Which.Message.Should().Be("Oops"));
        }

        [Scenario]
        public void ExceptionInArgumentMatcherAndInDescription(IFoo fake, Exception exception)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And a call to the fake is configured with a custom argument matcher that throws an exception and whose description also throws an exception"
                .x(() => A.CallTo(() => fake.Bar(A<int>.That.Matches(i => ThrowException(), o => o.Write(ThrowException())))).Returns(42));

            "When the configured method is called"
                .x(() => exception = Record.Exception(() => fake.Bar(0)));

            "Then a UserCallbackException should be thrown"
                .x(() => exception.Should().BeAnExceptionOfType<UserCallbackException>());

            "And its message should describe where the exception was thrown from"
                .x(() => exception.Message.Should().Be("Argument matcher description threw an exception. See inner exception for details."));

            "And the inner exception should be the original exception"
                .x(() => exception.InnerException.Should().BeAnExceptionOfType<MyException>().Which.Message.Should().Be("Oops"));
        }

        [Scenario]
        public void ExceptionInCallCountSpecification(IFoo fake, Exception exception)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And no call to the fake is made"
                .x(() => { });

            "When an assertion is made with a call count constraint that throws an exception"
                .x(() => exception = Record.Exception(() => A.CallTo(() => fake.Bar(0)).MustHaveHappenedANumberOfTimesMatching(n => ThrowException())));

            "Then a UserCallbackException should be thrown"
                .x(() => exception.Should().BeAnExceptionOfType<UserCallbackException>());

            "And its message should describe where the exception was thrown from"
                .x(() => exception.Message.Should().Be("Call count constraint <a number of times matching the predicate 'n => ThrowException()'> threw an exception. See inner exception for details."));

            "And the inner exception should be the original exception"
                .x(() => exception.InnerException.Should().BeAnExceptionOfType<MyException>().Which.Message.Should().Be("Oops"));
        }

        [Scenario]
        public void ExceptionInCustomArgumentValueFormatter(IFoo fake, Exception exception)
        {
            "Given a custom argument value formatter that throws an exception"
                .See<HasCustomValueFormatterValueFormatter>();

            "And a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And no call is made to the fake"
                .x(() => { });

            "When an assertion using an argument of the type with the throwing formatter is made on the fake"
                .x(() => exception = Record.Exception(() => A.CallTo(() => fake.Baz(new HasCustomValueFormatter())).MustHaveHappened()));

            "Then a UserCallbackException should be thrown"
                .x(() => exception.Should().BeAnExceptionOfType<UserCallbackException>());

            "And its message should describe where the exception was thrown from"
                .x(() => exception.Message.Should().Be("Custom argument value formatter 'FakeItEasy.Specs.UserCallbackExceptionSpecs+HasCustomValueFormatterValueFormatter' threw an exception. See inner exception for details."));

            "And the inner exception should be the original exception"
                .x(() => exception.InnerException.Should().BeAnExceptionOfType<MyException>().Which.Message.Should().Be("Oops"));
        }

        [Scenario]
        public void ExceptionInDummyFactory(MyDummy dummy, Exception exception)
        {
            "Given a type"
                .See<MyDummy>();

            "And a dummy factory for this type that throws"
                .See<MyDummyFactory>();

            "When I try to create a dummy for this type"
                .x(() => exception = Record.Exception(() => dummy = A.Dummy<MyDummy>()));

            "Then a UserCallbackException should be thrown"
                .x(() => exception.Should().BeAnExceptionOfType<UserCallbackException>());

            "And its message should describe where the exception was thrown from"
                .x(() => exception.Message.Should().Be("Dummy factory 'FakeItEasy.Specs.UserCallbackExceptionSpecs+MyDummyFactory' threw an exception. See inner exception for details."));

            "And the inner exception should be the original exception"
                .x(() => exception.InnerException.Should().BeAnExceptionOfType<MyException>().Which.Message.Should().Be("Oops"));
        }

        [Scenario]
        public void ExceptionInFakeOptionsBuilder(IHasFakeOptionsBuilder fake, Exception exception)
        {
            "Given a type"
                .See<IHasFakeOptionsBuilder>();

            "And an option builder for this type that throws"
                .See<BadOptionsBuilder>();

            "When I try to create a fake of this type"
                .x(() => exception = Record.Exception(() => fake = A.Fake<IHasFakeOptionsBuilder>()));

            "Then a UserCallbackException should be thrown"
                .x(() => exception.Should().BeAnExceptionOfType<UserCallbackException>());

            "And its message should describe where the exception was thrown from"
                .x(() => exception.Message.Should().Be("Fake options builder 'FakeItEasy.Specs.UserCallbackExceptionSpecs+BadOptionsBuilder' threw an exception. See inner exception for details."));

            "And the inner exception should be the original exception"
                .x(() => exception.InnerException.Should().BeAnExceptionOfType<MyException>().Which.Message.Should().Be("Oops"));
        }

        [Scenario]
        public void ExceptionInWhenArgumentsMatchPredicateForExpressionCallSpec(IFoo fake, Exception exception)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And a method configured on this fake with a WhenArgumentsMatch predicate that throws an exception"
                .x(() => A.CallTo(() => fake.Bar(0)).WhenArgumentsMatch(args => ThrowException()).Returns(0));

            "When a call to the configured method is made"
                .x(() => exception = Record.Exception(() => fake.Bar(0)));

            "Then a UserCallbackException should be thrown"
                .x(() => exception.Should().BeAnExceptionOfType<UserCallbackException>());

            "And its message should describe where the exception was thrown from"
                .x(() => exception.Message.Should().Be("Arguments predicate threw an exception. See inner exception for details."));

            "And the inner exception should be the original exception"
                .x(() => exception.InnerException.Should().BeAnExceptionOfType<MyException>().Which.Message.Should().Be("Oops"));
        }

        [Scenario]
        public void ExceptionInWhenArgumentsMatchPredicateForAnyCallSpec(IFoo fake, Exception exception)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And calls configured on this fake with a WhenArgumentsMatch predicate that throws an exception"
                .x(() => A.CallTo(fake).WhenArgumentsMatch(args => ThrowException()).DoesNothing());

            "When a call to any method of the fake is made"
                .x(() => exception = Record.Exception(() => fake.Bar(0)));

            "Then a UserCallbackException should be thrown"
                .x(() => exception.Should().BeAnExceptionOfType<UserCallbackException>());

            "And its message should describe where the exception was thrown from"
                .x(() => exception.Message.Should().Be("Arguments predicate threw an exception. See inner exception for details."));

            "And the inner exception should be the original exception"
                .x(() => exception.InnerException.Should().BeAnExceptionOfType<MyException>().Which.Message.Should().Be("Oops"));
        }

        [Scenario]
        public void ExceptionInWherePredicate(IFoo fake, Exception exception)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And calls configured on this fake with a call filter predicate that throws an exception"
                .x(() => A.CallTo(fake).Where(call => ThrowException()).DoesNothing());

            "When a call to any method of the fake is made"
                .x(() => exception = Record.Exception(() => fake.Bar(0)));

            "Then a UserCallbackException should be thrown"
                .x(() => exception.Should().BeAnExceptionOfType<UserCallbackException>());

            "And its message should describe where the exception was thrown from"
                .x(() => exception.Message.Should().Be("Call filter <call => ThrowException()> threw an exception. See inner exception for details."));

            "And the inner exception should be the original exception"
                .x(() => exception.InnerException.Should().BeAnExceptionOfType<MyException>().Which.Message.Should().Be("Oops"));
        }

        [Scenario]
        public void ExceptionInWhereDescription(IFoo fake, Exception exception)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And no call to the fake is made"
                .x(() => { });

            "When an assertion is made with a call filter whose description throws an exception"
                .x(() => exception = Record.Exception(() => A.CallTo(fake).Where(call => true, o => o.Write(ThrowException())).MustHaveHappened()));

            "Then a UserCallbackException should be thrown"
                .x(() => exception.Should().BeAnExceptionOfType<UserCallbackException>());

            "And its message should describe where the exception was thrown from"
                .x(() => exception.Message.Should().Be("Call filter description threw an exception. See inner exception for details."));

            "And the inner exception should be the original exception"
                .x(() => exception.InnerException.Should().BeAnExceptionOfType<MyException>().Which.Message.Should().Be("Oops"));
        }

        [Scenario]
        public void ExceptionInWherePredicateAndInDescription(IFoo fake, Exception exception)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And calls configured on this fake with a call filter predicate that throws an exception and whose description also throws an exception"
                .x(() => A.CallTo(fake).Where(call => ThrowException(), o => o.Write(ThrowException())).DoesNothing());

            "When a call to any method of the fake is made"
                .x(() => exception = Record.Exception(() => fake.Bar(0)));

            "Then a UserCallbackException should be thrown"
                .x(() => exception.Should().BeAnExceptionOfType<UserCallbackException>());

            "And its message should describe where the exception was thrown from"
                .x(() => exception.Message.Should().Be("Call filter description threw an exception. See inner exception for details."));

            "And the inner exception should be the original exception"
                .x(() => exception.InnerException.Should().BeAnExceptionOfType<MyException>().Which.Message.Should().Be("Oops"));
        }

        [Scenario]
        public void ExceptionInReturnValueProducer(IFoo fake, Exception exception)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And a call to the fake is configured with a return value producer that throws an exception"
                .x(() => A.CallTo(() => fake.Bar(0)).ReturnsLazily(() => ThrowException().GetHashCode()));

            "When the configured method is called"
                .x(() => exception = Record.Exception(() => fake.Bar(0)));

            "Then a UserCallbackException should be thrown"
                .x(() => exception.Should().BeAnExceptionOfType<UserCallbackException>());

            "And its message should describe where the exception was thrown from"
                .x(() => exception.Message.Should().Be("Return value producer threw an exception. See inner exception for details."));

            "And the inner exception should be the original exception"
                .x(() => exception.InnerException.Should().BeAnExceptionOfType<MyException>().Which.Message.Should().Be("Oops"));
        }

        [Scenario]
        public void ExceptionInOutAndRefValueProducer(IFoo fake, Exception exception, int x, string s)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And a call to the fake is configured with an out and ref value producer that throws an exception"
                .x(() => A.CallTo(() => fake.OutAndRef(ref x, out s)).AssignsOutAndRefParametersLazily(call => new object[] { ThrowException() }));

            "When the configured method is called"
                .x(() => exception = Record.Exception(() => fake.OutAndRef(ref x, out s)));

            "Then a UserCallbackException should be thrown"
                .x(() => exception.Should().BeAnExceptionOfType<UserCallbackException>());

            "And its message should describe where the exception was thrown from"
                .x(() => exception.Message.Should().Be("Out and ref parameter value producer threw an exception. See inner exception for details."));

            "And the inner exception should be the original exception"
                .x(() => exception.InnerException.Should().BeAnExceptionOfType<MyException>().Which.Message.Should().Be("Oops"));
        }

        [Scenario]
        public void ExceptionInExceptionFactory(IFoo fake, Exception exception)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And a call to the fake is configured with an exception factory that throws an exception"
                .x(() => A.CallTo(() => fake.Bar(0)).Throws(call =>
                {
                    ThrowException();
                    return new Exception("test");
                }));

            "When the configured method is called"
                .x(() => exception = Record.Exception(() => fake.Bar(0)));

            "Then a UserCallbackException should be thrown"
                .x(() => exception.Should().BeAnExceptionOfType<UserCallbackException>());

            "And its message should describe where the exception was thrown from"
                .x(() => exception.Message.Should().Be("Exception factory threw an exception. See inner exception for details."));

            "And the inner exception should be the original exception"
                .x(() => exception.InnerException.Should().BeAnExceptionOfType<MyException>().Which.Message.Should().Be("Oops"));
        }

        [Scenario]
        public void ExceptionInCallbackIsNotWrapped(IFoo fake, Exception exception)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And a call to the fake is configured with a callback that throws an exception"
                .x(() => A.CallTo(() => fake.Bar(0)).Invokes(() => ThrowException()));

            "When the configured method is called"
                .x(() => exception = Record.Exception(() => fake.Bar(0)));

            "Then the original exception should be thrown"
                .x(() => exception.Should().BeAnExceptionOfType<MyException>().Which.Message.Should().Be("Oops"));
        }

        private static bool ThrowException()
        {
            throw new MyException("Oops");
        }

        private static int ThrowingConstraintFactory()
        {
            ThrowException();
            return 42;
        }

        public class MyException : Exception
        {
            public MyException(string message, Exception inner = null)
                : base(message, inner)
            {
            }
        }

        public class HasCustomValueFormatter
        {
        }

        public class HasCustomValueFormatterValueFormatter : ArgumentValueFormatter<HasCustomValueFormatter>
        {
            protected override string GetStringValue(HasCustomValueFormatter argumentValue)
            {
                throw new MyException("Oops");
            }
        }

        public class MyDummy
        {
        }

        public class MyDummyFactory : DummyFactory<MyDummy>
        {
            protected override MyDummy Create()
            {
                ThrowException();
                return default;
            }
        }

        public class BadOptionsBuilder : FakeOptionsBuilder<IHasFakeOptionsBuilder>
        {
            protected override void BuildOptions(IFakeOptions<IHasFakeOptionsBuilder> options)
            {
                ThrowException();
            }
        }
    }
}
