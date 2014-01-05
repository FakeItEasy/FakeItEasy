namespace FakeItEasy.Tests.Core
{
    using FluentAssertions;
    using NUnit.Framework;

    [TestFixture]
    public class HelpersTests : ConfigurableServiceLocatorTestBase
    {
        [Test]
        public void GetDescription_should_render_method_name_and_empty_arguments_list_when_call_has_no_arguments()
        {
            // arrange
            var call = FakeCall.Create<object>("GetType");

            // act
            var description = call.GetDescription();

            // assert
            description.Should().Be("System.Object.GetType()");
        }

        [Test]
        public void GetDescription_should_render_method_name_and_all_arguments_when_call_has_arguments()
        {
            // arrange
            var call = CreateFakeCallToFooDotBar("abc", 123);

            // act
            var description = call.GetDescription();

            // assert
            description.Should().Be("FakeItEasy.Tests.IFoo.Bar(\"abc\", 123)");
        }

        [Test]
        public void GetDescription_should_render_NULL_when_argument_is_null()
        {
            // arrange
            var call = CreateFakeCallToFooDotBar(null, 123);

            // act
            var description = call.GetDescription();

            // assert
            description.Should().Be("FakeItEasy.Tests.IFoo.Bar(<NULL>, 123)");
        }

        [Test]
        public void GetDescription_should_render_string_empty_when_string_is_empty()
        {
            // arrange
            var call = CreateFakeCallToFooDotBar(string.Empty, 123);

            // act
            var description = call.GetDescription();

            // assert
            description.Should().Be("FakeItEasy.Tests.IFoo.Bar(<string.Empty>, 123)");
        }

        private static FakeCall CreateFakeCallToFooDotBar(object argument1, object argument2)
        {
            return FakeCall.Create<IFoo>("Bar", new[] { typeof(object), typeof(object) }, new[] { argument1, argument2 });
        }
    }
}
