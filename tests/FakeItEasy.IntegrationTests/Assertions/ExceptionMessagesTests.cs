namespace FakeItEasy.IntegrationTests.Assertions
{
    using System;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xunit;

    public class ExceptionMessagesTests
    {
        [Fact]
        [UsingCulture("en-US")]
        public void Exception_message_should_be_correctly_formatted()
        {
            var foo = A.Fake<IFoo>();

            foo.Bar();
            foo.Bar();

            foo.Bar("test");
            foo.Bar(new DateTime(1977, 4, 5), "birthday");
            foo.ToString();
            foo.Biz();

            var exception = Record.Exception(() =>
                A.CallTo(() => foo.Bar(string.Empty)).MustHaveHappenedTwiceOrMore());

            var expectedMessage =
@"

  Assertion failed for the following call:
    FakeItEasy.IntegrationTests.IFoo.Bar(argument: string.Empty)
  Expected to find it twice or more but didn't find it among the calls:
    1: FakeItEasy.IntegrationTests.IFoo.Bar() 2 times
    ...
    3: FakeItEasy.IntegrationTests.IFoo.Bar(argument: ""test"")
    4: FakeItEasy.IntegrationTests.IFoo.Bar(argument: 4/5/1977 12:00:00 AM, argument2: ""birthday"")
    5: FakeItEasy.IntegrationTests.IFoo.ToString()
    6: FakeItEasy.IntegrationTests.IFoo.Biz()

";
            exception.Should().BeAnExceptionOfType<ExpectationException>()
                     .WithMessageModuloLineEndings(expectedMessage);
        }

        [Fact]
        [UsingCulture("en-US")]
        public void Exception_message_should_be_correctly_formatted_when_containing_call_with_three_or_more_arguments()
        {
            var foo = A.Fake<IFoo>();

            foo.Bar(1, 2, "three");
            foo.Bar(1, 2, "three");
            foo.Bar();

            var exception = Record.Exception(() =>
                A.CallTo(() => foo.Bar(string.Empty)).MustHaveHappenedTwiceOrMore());

            var expectedMessage =
@"

  Assertion failed for the following call:
    FakeItEasy.IntegrationTests.IFoo.Bar(argument: string.Empty)
  Expected to find it twice or more but didn't find it among the calls:
    1: FakeItEasy.IntegrationTests.IFoo.Bar(
          argument1: 1,
          argument2: 2,
          argument3: ""three"") 2 times
    ...
    3: FakeItEasy.IntegrationTests.IFoo.Bar()

";

            exception.Should().BeAnExceptionOfType<ExpectationException>()
                .WithMessageModuloLineEndings(expectedMessage);
        }

        [Fact]
        public void Should_be_able_to_assert_on_void_calls_from_configuration()
        {
            // Arrange
            var foo = A.Fake<IFoo>();

            // Act
            foo.Bar(new object(), "lorem ipsum");

            // Assert
            var exception = Record.Exception(() =>
                A.CallTo(() => foo.Bar(A<object>._, A<string>.That.StartsWith("lorem"))).MustHaveHappenedTwiceOrMore());

            var expectedMessage =
@"

  Assertion failed for the following call:
    FakeItEasy.IntegrationTests.IFoo.Bar(argument: <Ignored>, argument2: <Starts with ""lorem"">)
  Expected to find it twice or more but found it once among the calls:
    1: FakeItEasy.IntegrationTests.IFoo.Bar(argument: System.Object, argument2: ""lorem ipsum"")

";

            exception.Should().BeAnExceptionOfType<ExpectationException>().WithMessageModuloLineEndings(expectedMessage);
        }

        [Fact]
        public void Should_be_able_to_assert_on_function_calls_from_configuration()
        {
            // Arrange
            var foo = A.Fake<IFoo>();
            foo.Baz(new object(), "lorem ipsum");

            // Act
            var exception = Record.Exception(() =>
                A.CallTo(() => foo.Baz(A<object>._, A<string>.That.StartsWith("lorem"))).MustHaveHappenedTwiceOrMore());

            // Assert
            var expectedMessage =
@"

  Assertion failed for the following call:
    FakeItEasy.IntegrationTests.IFoo.Baz(argument: <Ignored>, argument2: <Starts with ""lorem"">)
  Expected to find it twice or more but found it once among the calls:
    1: FakeItEasy.IntegrationTests.IFoo.Baz(argument: System.Object, argument2: ""lorem ipsum"")

";
            exception.Should().BeAnExceptionOfType<ExpectationException>()
                .WithMessageModuloLineEndings(expectedMessage);
        }
    }
}
