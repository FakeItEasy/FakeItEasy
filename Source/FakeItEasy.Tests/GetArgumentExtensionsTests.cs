namespace FakeItEasy.Tests
{
    using FakeItEasy.Core;
    using FluentAssertions;
    using NUnit.Framework;

    [TestFixture]
    public class GetArgumentExtensionsTests : ConfigurableServiceLocatorTestBase
    {
        [Test]
        public void Should_delegate_to_the_argument_collections_get_method_when_using_index()
        {
            // Arrange
            var call = A.Fake<IFakeObjectCall>();
            var arguments = new ArgumentCollection(new object[] { 1, 2 }, new string[] { "argument1", "argument2" });
            A.CallTo(() => call.Arguments).Returns(arguments);

            // Act
            var result = call.GetArgument<int>(0);

            // Assert
            result.Should().Be(1);
        }

        [Test]
        public void Should_delegate_to_the_argument_collections_get_method_when_using_name()
        {
            // Arrange
            var call = A.Fake<IFakeObjectCall>();
            var arguments = new ArgumentCollection(new object[] { 1, 2 }, new string[] { "argument1", "argument2" });
            A.CallTo(() => call.Arguments).Returns(arguments);

            // Act
            var result = call.GetArgument<int>("argument2");

            // Assert
            result.Should().Be(2);
        }

        [Test]
        public void Should_be_null_guarded_when_using_index()
        {
            // Arrange

            // Act

            // Assert
            NullGuardedConstraint.Assert(() =>
                A.Fake<IFakeObjectCall>().GetArgument<int>(0));
        }

        [Test]
        public void Should_be_null_guarded_when_using_argument_name()
        {
            // Arrange

            // Act

            // Assert
            NullGuardedConstraint.Assert(() =>
                A.Fake<IFakeObjectCall>().GetArgument<int>("foo"));
        }
    }
}
