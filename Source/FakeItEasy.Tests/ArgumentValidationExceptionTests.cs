using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace FakeItEasy.Tests
{
    [TestFixture]
    public class ArgumentValidationExceptionTests
        : ExceptionContractTests<ArgumentValidationException>
    {
        [Test]
        public void Constructor_that_takes_message_should_set_message_to_exception()
        {
            var exception = new ArgumentValidationException("foo");

            Assert.That(exception.Message, Is.EqualTo("foo"));
        }

        [Test]
        public void DefaultConstructor_should_set_message_to_exception()
        {
            var exception = new ArgumentValidationException();

            Assert.That(exception.Message, Is.EqualTo("An argument validation was not configured correctly."));
        }

        protected override ArgumentValidationException CreateException()
        {
            return new ArgumentValidationException();
        }
    }
}