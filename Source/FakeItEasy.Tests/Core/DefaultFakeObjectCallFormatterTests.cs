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
        [Fake] internal ArgumentValueFormatter ArgumentFormatter;
        [Fake] internal IFakeManagerAccessor FakeManagerAccessor;


        [SetUp]
        public void SetUp()
        {
            Fake.InitializeFixture(this);

            this.formatter = new DefaultFakeObjectCallFormatter(this.ArgumentFormatter, this.FakeManagerAccessor);
        }

        [Test]
        public void Should_start_with_method_name()
        {
            // Arrange
            var call = this.CreateFakeCall(typeof(string), typeof(object).GetMethod("Equals", new[] { typeof(object) }), "foo");

            // Act
            var description = this.formatter.GetDescription(call);

            // Assert
            Assert.That(description, Is.StringStarting(
                "System.String.Equals("));
        }

        [Test]
        public void Should_write_empty_argument_list()
        {
            // Arrange
            var call = this.CreateFakeCall(typeof(object).GetMethod("ToString", new Type[] { }));
            
            // Act
            var description = this.formatter.GetDescription(call);

            // Assert
            Assert.That(description, Is.StringEnding(
                "()"));
        }

        [Test]
        [SetCulture("en-US")]
        public void Should_write_argument_list()
        {
            // Arrange
            var call = CreateFakeCallToFoo("argument value", 1);
            A.CallTo(() => this.ArgumentFormatter.GetArgumentValueAsString("argument value")).Returns("\"argument value\"");
            A.CallTo(() => this.ArgumentFormatter.GetArgumentValueAsString(1)).Returns("1");

            // Act
            var description = this.formatter.GetDescription(call);

            // Assert
            Assert.That(description,
                Is.StringEnding("(argument1: \"argument value\", argument2: 1)"));
        }

      

        [Test]
        public void Should_put_each_argument_on_separate_line_when_more_than_two_arguments()
        {
            // Arrange
            var call = CreateFakeCall(
                typeof(ITypeWithMethodThatTakesArguments).GetMethod("MoreThanTwo", new[] { typeof(string), typeof(string), typeof(string) }),
                "one", "two", "three");
            A.CallTo(() => this.ArgumentFormatter.GetArgumentValueAsString(A<object>.Ignored))
                .ReturnsLazily(x => x.GetArgument<object>(0).ToString());

            // Act
            var description = this.formatter.GetDescription(call);

            // Assert
            Assert.That(description, Is.StringEnding(@"(
    one: one,
    two: two,
    three: three)"));
        }

        [Test]
        public void Should_write_property_getter_properly()
        {
            // Arrange
            var propertyGetter = typeof (TypeWithProperties).GetProperty("NormalProperty").GetGetMethod();
            var call = CreateFakeCall(typeof(TypeWithProperties), propertyGetter);

            // Act
            var description = this.formatter.GetDescription(call);

            // Assert
            Assert.That(description, Is.EqualTo("FakeItEasy.Tests.Core.DefaultFakeObjectCallFormatterTests+TypeWithProperties.NormalProperty"));
        }

        [Test]
        public void Should_write_property_setter_properly()
        {
            // Arrange
            var propertyGetter = typeof(TypeWithProperties).GetProperty("NormalProperty").GetSetMethod();
            var call = CreateFakeCall(typeof(TypeWithProperties), propertyGetter, "foo");
            A.CallTo(() => this.ArgumentFormatter.GetArgumentValueAsString("foo")).Returns("\"foo\"");

            // Act
            var description = this.formatter.GetDescription(call);

            // Assert
            Assert.That(description, Is.EqualTo("FakeItEasy.Tests.Core.DefaultFakeObjectCallFormatterTests+TypeWithProperties.NormalProperty = \"foo\""));
        }

        [Test]
        public void Should_write_indexed_property_getter_properly()
        {
            // Arrange
            var propertyGetter = typeof(TypeWithProperties).GetProperty("Item").GetGetMethod();
            var call = CreateFakeCall(typeof(TypeWithProperties), propertyGetter, 0);
            A.CallTo(() => this.ArgumentFormatter.GetArgumentValueAsString(0)).Returns("0");

            // Act
            var description = this.formatter.GetDescription(call);

            // Assert
            Assert.That(description, Is.EqualTo("FakeItEasy.Tests.Core.DefaultFakeObjectCallFormatterTests+TypeWithProperties.Item[index: 0]"));
        }

        [Test]
        public void Should_write_indexed_property_setter_properly()
        {
            // Arrange
            var propertyGetter = typeof(TypeWithProperties).GetProperty("Item").GetSetMethod();
            var call = CreateFakeCall(typeof(TypeWithProperties), propertyGetter, 0, "argument");
            A.CallTo(() => this.ArgumentFormatter.GetArgumentValueAsString(0)).Returns("0");
            A.CallTo(() => this.ArgumentFormatter.GetArgumentValueAsString("argument")).Returns("\"argument\"");

            // Act
            var description = this.formatter.GetDescription(call);

            // Assert
            Assert.That(description, Is.EqualTo("FakeItEasy.Tests.Core.DefaultFakeObjectCallFormatterTests+TypeWithProperties.Item[index: 0] = \"argument\""));
        }

        private IFakeObjectCall CreateFakeCall(MethodInfo method, params object[] arguments)
        {
            return CreateFakeCall(typeof (object), method, arguments);
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
            return this.CreateFakeCall(typeof(ITypeWithMethodThatTakesArguments).GetMethod("Foo", new[] { typeof(string), typeof(object) }),
                argument1, argument2);
        }

        public class TypeWithProperties
        {
            public string NormalProperty { get; set; }

            public string this[int index]
            {
                get { return index.ToString(); }
                set { }
            }
        }

        public interface ITypeWithMethodThatTakesArguments
        {
            void Foo(string argument1, object argument2);
            void MoreThanTwo(string one, string two, string three);
        }
    }
}
