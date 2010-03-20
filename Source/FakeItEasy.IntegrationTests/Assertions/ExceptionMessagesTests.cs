using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using FakeItEasy.Tests;

namespace FakeItEasy.IntegrationTests.Assertions
{
    [TestFixture]
    public class ExceptionMessagesTests
    {
        [Test]
        [SetCulture("en-US")]
        public void Exception_message_should_be_correctly_formatted()
        {
            var foo = A.Fake<IFoo>();

            foo.Bar();
            foo.Bar();

            foo.Bar("test");
            foo.Bar(new DateTime(1977, 4, 5), "birthday");
            foo.ToString();
            foo.Biz();

            var exception = Assert.Throws<ExpectationException>(() =>
                A.CallTo(() => foo.Bar("")).MustHaveHappened(Repeated.Twice));

            Assert.That(exception.Message, Is.EqualTo(@"

  Assertion failed for the following call:
    'FakeItEasy.Tests.IFoo.Bar("""")'
  Expected to find it twice but found it #0 times among the calls:
    1.  'FakeItEasy.Tests.IFoo.Bar()' repeated 2 times
    ...
    3.  'FakeItEasy.Tests.IFoo.Bar(""test"")'
    4.  'FakeItEasy.Tests.IFoo.Bar(4/5/1977 12:00:00 AM, ""birthday"")'
    5.  'System.Object.ToString()'
    6.  'FakeItEasy.Tests.IFoo.Biz()'

"));
        }
        
        [Test]
        public void Should_be_able_to_assert_on_void_calls_from_configuration()
        {
            // Arrange
            var foo = A.Fake<IFoo>();

            // Act
            foo.Bar(new object(), "lorem ipsum");

            // Assert
            var thrown = Assert.Throws<ExpectationException>(() =>
                A.CallTo(() => foo.Bar(A<object>.Ignored, A<string>.That.StartsWith("lorem"))).MustHaveHappened(Repeated.Twice));

            Assert.That(thrown.Message, Is.EqualTo(@"

  Assertion failed for the following call:
    'FakeItEasy.Tests.IFoo.Bar(<Ignored>, <Starts with ""lorem"">)'
  Expected to find it twice but found it #1 times among the calls:
    1.  'FakeItEasy.Tests.IFoo.Bar(System.Object, ""lorem ipsum"")'

"));
        }

        [Test]
        public void Should_be_able_to_assert_on_function_calls_from_configuration()
        {
            // Arrange
            var foo = A.Fake<IFoo>();

            // Act
            foo.Baz(new object(), "lorem ipsum");

            // Assert
            var thrown = Assert.Throws<ExpectationException>(() =>
                A.CallTo(() => foo.Baz(A<object>.Ignored, A<string>.That.StartsWith("lorem"))).MustHaveHappened(Repeated.Twice));

            Assert.That(thrown.Message, Is.EqualTo(@"

  Assertion failed for the following call:
    'FakeItEasy.Tests.IFoo.Baz(<Ignored>, <Starts with ""lorem"">)'
  Expected to find it twice but found it #1 times among the calls:
    1.  'FakeItEasy.Tests.IFoo.Baz(System.Object, ""lorem ipsum"")'

"));
        }
    }
}
