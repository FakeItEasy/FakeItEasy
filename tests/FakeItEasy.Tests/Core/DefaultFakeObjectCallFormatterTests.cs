namespace FakeItEasy.Tests.Core
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using FakeItEasy.Configuration;
    using FakeItEasy.Core;
    using FluentAssertions;
    using Xunit;

    public class DefaultFakeObjectCallFormatterTests
    {
        private readonly DefaultFakeObjectCallFormatter formatter;
        private readonly ArgumentValueFormatter argumentFormatter;
        private readonly IFakeManagerAccessor fakeManagerAccessor;

        public DefaultFakeObjectCallFormatterTests()
        {
            this.argumentFormatter = A.Fake<ArgumentValueFormatter>();
            this.fakeManagerAccessor = A.Fake<IFakeManagerAccessor>();
            this.formatter = new DefaultFakeObjectCallFormatter(this.argumentFormatter, this.fakeManagerAccessor);
        }

        public interface ITypeWithMethodThatTakesArguments
        {
            void Foo(string argument1, object argument2);

            void MoreThanTwo(string one, string two, string three);
        }

        [Fact]
        public void Should_start_with_method_name()
        {
            // Arrange
            var call = this.CreateFakeCall(typeof(string), typeof(object).GetMethod("Equals", new[] { typeof(object) }), "foo");

            // Act
            var description = this.formatter.GetDescription(call);

            // Assert
            description.Should().StartWith("System.String.Equals(");
        }

        [Fact]
        public void Should_write_empty_argument_list()
        {
            // Arrange
            var call = this.CreateFakeCall(typeof(object).GetMethod("ToString", new Type[] { }));

            // Act
            var description = this.formatter.GetDescription(call);

            // Assert
            description.Should().EndWith("()");
        }

        [Fact]
        public void Should_write_argument_list()
        {
            // Arrange
            var call = this.CreateFakeCallToFoo("argument value", 1);
            A.CallTo(() => this.argumentFormatter.GetArgumentValueAsString("argument value")).Returns(@"""argument value""");
            A.CallTo(() => this.argumentFormatter.GetArgumentValueAsString(1)).Returns("1");

            // Act
            var description = this.formatter.GetDescription(call);

            // Assert
            description.Should().EndWith(@"(argument1: ""argument value"", argument2: 1)");
        }

        [Fact]
        public void Should_put_each_argument_on_separate_line_when_more_than_two_arguments()
        {
            // Arrange
            var call = this.CreateFakeCall(
                typeof(ITypeWithMethodThatTakesArguments).GetMethod("MoreThanTwo", new[] { typeof(string), typeof(string), typeof(string) }),
                "one",
                "two",
                "three");

            A.CallTo(() => this.argumentFormatter.GetArgumentValueAsString(A<object>._))
                .ReturnsLazily(x => x.GetArgument<object>(0).ToString());

            // Act
            var description = this.formatter.GetDescription(call);

            // Assert
            var expectedDescription =
@"(
    one: one,
    two: two,
    three: three)";

            description.Should().EndWith(expectedDescription);
        }

        [Fact]
        public void Should_write_property_getter_properly()
        {
            // Arrange
            var propertyGetter = typeof(TypeWithProperties).GetProperty("NormalProperty").GetGetMethod();
            var call = this.CreateFakeCall(typeof(TypeWithProperties), propertyGetter);

            // Act
            var description = this.formatter.GetDescription(call);

            // Assert
            description.Should().Be("FakeItEasy.Tests.Core.DefaultFakeObjectCallFormatterTests+TypeWithProperties.NormalProperty");
        }

        [Fact]
        public void Should_write_property_setter_properly()
        {
            // Arrange
            var propertyGetter = typeof(TypeWithProperties).GetProperty("NormalProperty").GetSetMethod();
            var call = this.CreateFakeCall(typeof(TypeWithProperties), propertyGetter, "foo");
            A.CallTo(() => this.argumentFormatter.GetArgumentValueAsString("foo")).Returns(@"""foo""");

            // Act
            var description = this.formatter.GetDescription(call);

            // Assert
            description.Should().Be(@"FakeItEasy.Tests.Core.DefaultFakeObjectCallFormatterTests+TypeWithProperties.NormalProperty = ""foo""");
        }

        [Fact]
        public void Should_write_indexed_property_getter_properly()
        {
            // Arrange
            var propertyGetter = typeof(TypeWithProperties).GetProperty("Item").GetGetMethod();
            var call = this.CreateFakeCall(typeof(TypeWithProperties), propertyGetter, 0);
            A.CallTo(() => this.argumentFormatter.GetArgumentValueAsString(0)).Returns("0");

            // Act
            var description = this.formatter.GetDescription(call);

            // Assert
            description.Should().Be("FakeItEasy.Tests.Core.DefaultFakeObjectCallFormatterTests+TypeWithProperties.Item[index: 0]");
        }

        [Fact]
        public void Should_write_indexed_property_setter_properly()
        {
            // Arrange
            var propertyGetter = typeof(TypeWithProperties).GetProperty("Item").GetSetMethod();
            var call = this.CreateFakeCall(typeof(TypeWithProperties), propertyGetter, 0, "argument");
            A.CallTo(() => this.argumentFormatter.GetArgumentValueAsString(0)).Returns("0");
            A.CallTo(() => this.argumentFormatter.GetArgumentValueAsString("argument")).Returns(@"""argument""");

            // Act
            var description = this.formatter.GetDescription(call);

            // Assert
            description.Should().Be(@"FakeItEasy.Tests.Core.DefaultFakeObjectCallFormatterTests+TypeWithProperties.Item[index: 0] = ""argument""");
        }

        private IFakeObjectCall CreateFakeCall(MethodInfo method, params object[] arguments)
        {
            return this.CreateFakeCall(typeof(object), method, arguments);
        }

        private IFakeObjectCall CreateFakeCall(Type typeOfFakeObject, MethodInfo method, params object[] arguments)
        {
            var call = A.Fake<IFakeObjectCall>();

            A.CallTo(() => call.Method).Returns(method);
            A.CallTo(() => call.Arguments).Returns(new ArgumentCollection(arguments, method));
            A.CallTo(() => call.FakedObject).Returns(A.Fake<object>());

            var fakeManager = A.Fake<FakeManager>();
            A.CallTo(() => this.fakeManagerAccessor.GetFakeManager(call.FakedObject)).Returns(fakeManager);
            A.CallTo(() => fakeManager.FakeObjectType).Returns(typeOfFakeObject);

            return call;
        }

        private IFakeObjectCall CreateFakeCallToFoo(string argument1, object argument2)
        {
            return this.CreateFakeCall(
                typeof(ITypeWithMethodThatTakesArguments)
                    .GetMethod(nameof(ITypeWithMethodThatTakesArguments.Foo), new[] { typeof(string), typeof(object) }),
                argument1,
                argument2);
        }

        private class TypeWithProperties
        {
            public string NormalProperty { get; set; }

            [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "index", Justification = "Required for testing.")]
            [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "value", Justification = "Required for testing.")]
            [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for testing.")]
            public string this[int index]
            {
                get { return index.ToString(); }
                set { }
            }
        }
    }
}
