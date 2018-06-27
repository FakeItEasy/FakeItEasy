namespace FakeItEasy.Specs
{
    using System;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xbehave;
    using Xunit;

    public class UserCallbackExceptionSpecs
    {
        public interface IFoo
        {
            int Bar(int x);
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

        private static bool ThrowException()
        {
            throw new MyException("Oops");
        }

        public class MyException : Exception
        {
            public MyException(string message, Exception inner = null)
                : base(message, inner)
            {
            }
        }
    }
}
