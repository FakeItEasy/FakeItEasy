namespace FakeItEasy.Tests.Core
{
    using System;
    using System.Reflection;
    using FakeItEasy.Core;
    using NUnit.Framework;
    
    [TestFixture]
    public class DefaultFakeObjectCallFormatterTests
    {
        private DefaultFakeObjectCallFormatter formatter;

        [SetUp]
        public void SetUp()
        {
            this.formatter = new DefaultFakeObjectCallFormatter();
        }

        [Test]
        public void Should_start_with_method_name()
        {
            // Arrange
            var call = this.CreateFakeCall(typeof(object).GetMethod("Equals", new[] { typeof(object) }), "foo");

            // Act
            var description = this.formatter.GetDescription(call);

            // Assert
            Assert.That(description, Text.StartsWith(
                "System.Object.Equals("));
        }

        [Test]
        public void Should_write_empty_argument_list()
        {
            // Arrange
            var call = this.CreateFakeCall(typeof(object).GetMethod("ToString", new Type[] { }));
            
            // Act
            var description = this.formatter.GetDescription(call);

            // Assert
            Assert.That(description, Text.EndsWith(
                "()"));
        }

        [Test]
        [SetCulture("en-US")]
        public void Should_write_argument_list()
        {
            // Arrange
            var call = CreateFakeCallToFoo("argument value", 1);

            // Act
            var description = this.formatter.GetDescription(call);

            // Assert
            Assert.That(description,
                Text.EndsWith("(argument1: System.String = \"argument value\", argument2: System.Object = 1)"));
        }

        [Test]
        public void Should_write_null_values_correct()
        {
            // Arrange
            var call = CreateFakeCallToFoo(null, null);

            // Act
            var description = this.formatter.GetDescription(call);

            // Assert
            Assert.That(description, Text.Contains("<NULL>"));
        }

        [Test]
        public void Should_write_string_empty_correct()
        {
            // Arrange
            var call = CreateFakeCallToFoo(string.Empty, null);

            // Act
            var description = this.formatter.GetDescription(call);

            // Assert
            Assert.That(description, Text.Contains("string.Empty"));
        }

        [Test]
        public void Should_truncate_long_arguments()
        {
            // Arrange
            var call = CreateFakeCallToFoo("01234567890123456789012345678901234567890123456789", null);

            // Act
            var description = this.formatter.GetDescription(call);

            // Assert
            Assert.That(description, Text.Contains(
                "\"012345678901234567890123456789012345678..."));
        }

        [Test]
        public void Should_put_each_argument_on_separate_lines_when_more_than_two_arguments()
        {
            // Arrange
            var call = CreateFakeCall(
                typeof(ITypeWithMethodThatTakesArguments).GetMethod("MoreThanTwo", new[] { typeof(string), typeof(string), typeof(string) }),
                "one", "two", "three");

            // Act
            var description = this.formatter.GetDescription(call);

            // Assert
            Assert.That(description, Text.EndsWith(@"(
    one: System.String = ""one"",
    two: System.String = ""two"",
    three: System.String = ""three"")"));
        }

        private IFakeObjectCall CreateFakeCall(MethodInfo method, params object[] arguments)
        {
            var call = A.Fake<IFakeObjectCall>();
            
            A.CallTo(() => call.Method).Returns(method);
            A.CallTo(() => call.Arguments).Returns(new ArgumentCollection(arguments, method));
            A.CallTo(() => call.FakedObject).Returns(A.Fake<object>());

            return call;
        }

        private IFakeObjectCall CreateFakeCallToFoo(string argument1, object argument2)
        {
            return this.CreateFakeCall(typeof(ITypeWithMethodThatTakesArguments).GetMethod("Foo", new[] { typeof(string), typeof(object) }),
                argument1, argument2);
        }

        public interface ITypeWithMethodThatTakesArguments
        {
            void Foo(string argument1, object argument2);
            void MoreThanTwo(string one, string two, string three);
        }
    }
}
