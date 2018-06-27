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
