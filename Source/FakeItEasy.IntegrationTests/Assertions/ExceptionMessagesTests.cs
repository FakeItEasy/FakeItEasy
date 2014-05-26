namespace FakeItEasy.IntegrationTests.Assertions
{
    using System;
    using FakeItEasy.Tests;
    using NUnit.Framework;

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
                A.CallTo(() => foo.Bar(string.Empty)).MustHaveHappened(Repeated.AtLeast.Twice));

            var expectedMessage =
@"

  Assertion failed for the following call:
    FakeItEasy.Tests.IFoo.Bar("""")
  Expected to find it at least twice but found it #0 times among the calls:
    1: FakeItEasy.Tests.IFoo.Bar() repeated 2 times
    ...
    3: FakeItEasy.Tests.IFoo.Bar(argument: ""test"")
    4: FakeItEasy.Tests.IFoo.Bar(argument: 4/5/1977 12:00:00 AM, argument2: ""birthday"")
    5: FakeItEasy.Tests.IFoo.ToString()
    6: FakeItEasy.Tests.IFoo.Biz()

";
            Assert.That(exception.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        [SetCulture("en-US")]
        public void Exception_message_should_be_correctly_formatted_when_containing_call_with_three_or_more_arguments()
        {
            var foo = A.Fake<IFoo>();

            foo.Bar(1, 2, "three");
            foo.Bar(1, 2, "three");
            foo.Bar();

            var exception = Assert.Throws<ExpectationException>(() =>
                A.CallTo(() => foo.Bar(string.Empty)).MustHaveHappened(Repeated.AtLeast.Twice));

            var expectedMessage =
@"

  Assertion failed for the following call:
    FakeItEasy.Tests.IFoo.Bar("""")
  Expected to find it at least twice but found it #0 times among the calls:
    1: FakeItEasy.Tests.IFoo.Bar(
          argument1: 1,
          argument2: 2,
          argument3: ""three"") repeated 2 times
    ...
    3: FakeItEasy.Tests.IFoo.Bar()

";
            Assert.That(exception.Message, Is.EqualTo(expectedMessage));
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
                A.CallTo(() => foo.Bar(A<object>._, A<string>.That.StartsWith("lorem"))).MustHaveHappened(Repeated.AtLeast.Twice));

            var expectedMessage =
@"

  Assertion failed for the following call:
    FakeItEasy.Tests.IFoo.Bar(<Ignored>, <Starts with ""lorem"">)
  Expected to find it at least twice but found it #1 times among the calls:
    1: FakeItEasy.Tests.IFoo.Bar(argument: System.Object, argument2: ""lorem ipsum"")

";

            Assert.That(thrown.Message, Is.EqualTo(expectedMessage));
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
                A.CallTo(() => foo.Baz(A<object>._, A<string>.That.StartsWith("lorem"))).MustHaveHappened(Repeated.AtLeast.Twice));

            var expectedMessage =
@"

  Assertion failed for the following call:
    FakeItEasy.Tests.IFoo.Baz(<Ignored>, <Starts with ""lorem"">)
  Expected to find it at least twice but found it #1 times among the calls:
    1: FakeItEasy.Tests.IFoo.Baz(argument: System.Object, argument2: ""lorem ipsum"")

";

            Assert.That(thrown.Message, Is.EqualTo(expectedMessage));
        }
    }
}
