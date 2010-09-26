namespace FakeItEasy.Tests.Core
{
    using System;
    using System.Reflection;
    using FakeItEasy.Core;
    using NUnit.Framework;
    using System.Linq;
    using System.IO;
    using System.Text;
    using System.Collections.Generic;
    using System.Collections;

    [TestFixture]
    public class DefaultFakeObjectCallFormatterTests
    {
        private DefaultFakeObjectCallFormatter formatter;
        private ArgumentValueFormatter argumentFormatter;

        [SetUp]
        public void SetUp()
        {
            this.argumentFormatter = A.Fake<ArgumentValueFormatter>();

            this.formatter = new DefaultFakeObjectCallFormatter(this.argumentFormatter);
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
            A.CallTo(() => this.argumentFormatter.GetArgumentValueAsString("argument value")).Returns("\"argument value\"");
            A.CallTo(() => this.argumentFormatter.GetArgumentValueAsString(1)).Returns("1");

            // Act
            var description = this.formatter.GetDescription(call);

            // Assert
            Assert.That(description,
                Text.EndsWith("(argument1: \"argument value\", argument2: 1)"));
        }

      

        [Test]
        public void Should_put_each_argument_on_separate_lines_when_more_than_two_arguments()
        {
            // Arrange
            var call = CreateFakeCall(
                typeof(ITypeWithMethodThatTakesArguments).GetMethod("MoreThanTwo", new[] { typeof(string), typeof(string), typeof(string) }),
                "one", "two", "three");
            A.CallTo(() => this.argumentFormatter.GetArgumentValueAsString(A<object>.Ignored))
                .ReturnsLazily(x => x.GetArgument<object>(0).ToString());

            // Act
            var description = this.formatter.GetDescription(call);

            // Assert
            Assert.That(description, Text.EndsWith(@"(
    one: one,
    two: two,
    three: three)"));
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
