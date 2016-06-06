namespace FakeItEasy.Tests
{
    using System;
    using FakeItEasy.Creation;
    using FluentAssertions;
    using Xunit;

    public class ATests : ConfigurableServiceLocatorTestBase
    {
        private readonly IFakeCreatorFacade fakeCreator;

        public ATests()
        {
            this.fakeCreator = A.Fake<IFakeCreatorFacade>();
            this.StubResolve(this.fakeCreator);
        }

        [Fact]
        public void Fake_without_arguments_should_call_fake_creator_with_empty_options_builder()
        {
            // Arrange
            var fake = A.Fake<IFoo>();
            A.CallTo(() => this.fakeCreator.CreateFake(A<Action<IFakeOptions<IFoo>>>._)).Returns(fake);

            // Act
            var result = A.Fake<IFoo>();

            // Assert
            result.Should().BeSameAs(fake);
        }

        [Fact]
        public void Fake_with_arguments_should_call_fake_creator_with_specified_options_builder()
        {
            // Arrange
            var fake = A.Fake<IFoo>();

            Action<IFakeOptions<IFoo>> optionsBuilder = x => { };
            A.CallTo(() => this.fakeCreator.CreateFake(optionsBuilder)).Returns(fake);

            // Act
            var result = A.Fake(optionsBuilder);

            // Assert
            result.Should().BeSameAs(fake);
        }

        [Fact]
        public void Dummy_should_return_dummy_from_fake_creator()
        {
            // Arrange
            var dummy = A.Fake<IFoo>();
            A.CallTo(() => this.fakeCreator.CreateDummy<IFoo>()).Returns(dummy);

            // Act
            var result = A.Dummy<IFoo>();

            // Assert
            result.Should().BeSameAs(dummy);
        }
    }
}
