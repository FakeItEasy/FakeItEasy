namespace FakeItEasy.Tests
{
    using System.Linq;
    using System.Linq.Expressions;
    using FakeItEasy.Configuration;
    using FakeItEasy.Core;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xunit;

    public class FakeObjectCallExtensionsTests
    {
        [Fact]
        public void GetArgument_should_delegate_to_the_argument_collections_get_method_when_using_index()
        {
            // Arrange
            var call = A.Fake<IFakeObjectCall>();
            var method = FakeMethodHelper.CreateFakeMethod(new[] { "argument1", "argument2" });
            var arguments = new ArgumentCollection(new object[] { 1, 2 }, method);
            A.CallTo(() => call.Arguments).Returns(arguments);

            // Act
            var result = call.GetArgument<int>(0);

            // Assert
            result.Should().Be(1);
        }

        [Fact]
        public void GetArgument_should_delegate_to_the_argument_collections_get_method_when_using_name()
        {
            // Arrange
            var call = A.Fake<IFakeObjectCall>();
            var method = FakeMethodHelper.CreateFakeMethod(new[] { "argument1", "argument2" });
            var arguments = new ArgumentCollection(new object[] { 1, 2 }, method);
            A.CallTo(() => call.Arguments).Returns(arguments);

            // Act
            var result = call.GetArgument<int>("argument2");

            // Assert
            result.Should().Be(2);
        }

        [Fact]
        public void GetArgument_should_be_null_guarded_when_using_index()
        {
            // Arrange

            // Act

            // Assert
            Expression<System.Action> call = () => A.Fake<IFakeObjectCall>().GetArgument<int>(0);
            call.Should().BeNullGuarded();
        }

        [Fact]
        public void GetArgument_should_be_null_guarded_when_using_argument_name()
        {
            // Arrange

            // Act

            // Assert
            Expression<System.Action> call = () => A.Fake<IFakeObjectCall>().GetArgument<int>("foo");
            call.Should().BeNullGuarded();
        }

        [Fact]
        public void GetDescription_should_render_method_name_and_empty_arguments_list_when_call_has_no_arguments()
        {
            // Arrange
            var call = FakeCall.Create<object>("GetType");

            // Act
            var description = call.GetDescription();

            // Assert
            description.Should().Be("System.Object.GetType()");
        }

        [Fact]
        public void GetDescription_should_render_method_name_and_all_arguments_when_call_has_arguments()
        {
            // Arrange
            var call = CreateFakeCallToFooDotBar("abc", 123);

            // Act
            var description = call.GetDescription();

            // Assert
            description.Should().Be("FakeItEasy.Tests.IFoo.Bar(\"abc\", 123)");
        }

        [Fact]
        public void GetDescription_should_render_null_when_argument_is_null()
        {
            // Arrange
            var call = CreateFakeCallToFooDotBar(null, 123);

            // Act
            var description = call.GetDescription();

            // Assert
            description.Should().Be("FakeItEasy.Tests.IFoo.Bar(<NULL>, 123)");
        }

        [Fact]
        public void GetDescription_should_render_string_empty_when_string_is_empty()
        {
            // Arrange
            var call = CreateFakeCallToFooDotBar(string.Empty, 123);

            // Act
            var description = call.GetDescription();

            // Assert
            description.Should().Be("FakeItEasy.Tests.IFoo.Bar(<string.Empty>, 123)");
        }

        private static FakeCall CreateFakeCallToFooDotBar(object argument1, object argument2)
        {
            return FakeCall.Create<IFoo>("Bar", new[] { typeof(object), typeof(object) }, new[] { argument1, argument2 });
        }
    }
}
