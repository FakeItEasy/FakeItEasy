namespace FakeItEasy.Tests.Core
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Reflection;
    using FakeItEasy.Configuration;
    using FakeItEasy.Core;
    using FluentAssertions;
    using NUnit.Framework;

    [TestFixture]
    public class DefaultFakeObjectCallFormatterTests
    {
#pragma warning disable 649
        [UnderTest]
        private DefaultFakeObjectCallFormatter formatter;
#pragma warning restore 649

        public interface ITypeWithMethodThatTakesArguments
        {
            void Foo(string argument1, object argument2);

            void MoreThanTwo(string one, string two, string three);
        }

        [Fake]
        private ArgumentValueFormatter ArgumentFormatter { get; set; }

        [Fake]
        private IFakeManagerAccessor FakeManagerAccessor { get; set; }

        [SetUp]
        public void Setup()
        {
            Fake.InitializeFixture(this);
        }

        [Test]
        public void Should_start_with_method_name()
        {
            // Arrange
            var call = this.CreateFakeCall(typeof(string), typeof(object).GetMethod("Equals", new[] { typeof(object) }), "foo");

            // Act
            var description = this.formatter.GetDescription(call);

            // Assert
            description.Should().StartWith("System.String.Equals(");
        }

        [Test]
        public void Should_write_empty_argument_list()
        {
            // Arrange
            var call = this.CreateFakeCall(typeof(object).GetMethod("ToString", new Type[] { }));

            // Act
            var description = this.formatter.GetDescription(call);

            // Assert
            description.Should().EndWith("()");
        }

        [Test]
        [SetCulture("en-US")]
        public void Should_write_argument_list()
        {
            // Arrange
            var call = this.CreateFakeCallToFoo("argument value", 1);
            A.CallTo(() => this.ArgumentFormatter.GetArgumentValueAsString("argument value")).Returns("\"argument value\"");
            A.CallTo(() => this.ArgumentFormatter.GetArgumentValueAsString(1)).Returns("1");

            // Act
            var description = this.formatter.GetDescription(call);

            // Assert
            description.Should().EndWith("(argument1: \"argument value\", argument2: 1)");
        }

        [Test]
        public void Should_put_each_argument_on_separate_line_when_more_than_two_arguments()
        {
            // Arrange
            var call = this.CreateFakeCall(
                typeof(ITypeWithMethodThatTakesArguments).GetMethod("MoreThanTwo", new[] { typeof(string), typeof(string), typeof(string) }),
                "one",
                "two",
                "three");

            A.CallTo(() => this.ArgumentFormatter.GetArgumentValueAsString(A<object>._))
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

        [Test]
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

        [Test]
        public void Should_write_property_setter_properly()
        {
            // Arrange
            var propertyGetter = typeof(TypeWithProperties).GetProperty("NormalProperty").GetSetMethod();
            var call = this.CreateFakeCall(typeof(TypeWithProperties), propertyGetter, "foo");
            A.CallTo(() => this.ArgumentFormatter.GetArgumentValueAsString("foo")).Returns("\"foo\"");

            // Act
            var description = this.formatter.GetDescription(call);

            // Assert
            description.Should().Be("FakeItEasy.Tests.Core.DefaultFakeObjectCallFormatterTests+TypeWithProperties.NormalProperty = \"foo\"");
        }

        [Test]
        public void Should_write_indexed_property_getter_properly()
        {
            // Arrange
            var propertyGetter = typeof(TypeWithProperties).GetProperty("Item").GetGetMethod();
            var call = this.CreateFakeCall(typeof(TypeWithProperties), propertyGetter, 0);
            A.CallTo(() => this.ArgumentFormatter.GetArgumentValueAsString(0)).Returns("0");

            // Act
            var description = this.formatter.GetDescription(call);

            // Assert
            description.Should().Be("FakeItEasy.Tests.Core.DefaultFakeObjectCallFormatterTests+TypeWithProperties.Item[index: 0]");
        }

        [Test]
        public void Should_write_indexed_property_setter_properly()
        {
            // Arrange
            var propertyGetter = typeof(TypeWithProperties).GetProperty("Item").GetSetMethod();
            var call = this.CreateFakeCall(typeof(TypeWithProperties), propertyGetter, 0, "argument");
            A.CallTo(() => this.ArgumentFormatter.GetArgumentValueAsString(0)).Returns("0");
            A.CallTo(() => this.ArgumentFormatter.GetArgumentValueAsString("argument")).Returns("\"argument\"");

            // Act
            var description = this.formatter.GetDescription(call);

            // Assert
            description.Should().Be("FakeItEasy.Tests.Core.DefaultFakeObjectCallFormatterTests+TypeWithProperties.Item[index: 0] = \"argument\"");
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
            A.CallTo(() => this.FakeManagerAccessor.GetFakeManager(call.FakedObject)).Returns(fakeManager);
            A.CallTo(() => fakeManager.FakeObjectType).Returns(typeOfFakeObject);

            return call;
        }

        private IFakeObjectCall CreateFakeCallToFoo(string argument1, object argument2)
        {
            return this.CreateFakeCall(
                typeof(ITypeWithMethodThatTakesArguments).GetMethod("Foo", new[] { typeof(string), typeof(object) }),
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
                get { return index.ToString(CultureInfo.InvariantCulture); }
                set { }
            }
        }
    }
}
