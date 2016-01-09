namespace FakeItEasy.Tests
{
    using System;
    using System.Collections.Generic;
    using FakeItEasy.Creation;
    using FluentAssertions;
    using NUnit.Framework;

    [TestFixture]
    public class ATests : ConfigurableServiceLocatorTestBase
    {
        private IFakeCreatorFacade fakeCreator;
        private IDisposable scope;

        [Test]
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

        [Test]
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

        [Test]
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

        [Test]
        public void CollectionOfFakes_should_delegate_to_fake_creator()
        {
            // Arrange            
            var returnedFromCreator = new List<IFoo>();

            var creator = this.StubResolveWithFake<IFakeCreatorFacade>();
            A.CallTo(() => creator.CollectionOfFake<IFoo>(10)).Returns(returnedFromCreator);

            // Act
            object result = A.CollectionOfFake<IFoo>(10);

            // Assert
            result.Should().BeSameAs(returnedFromCreator);
        }

        protected override void OnSetup()
        {
            base.OnSetup();
            this.fakeCreator = A.Fake<IFakeCreatorFacade>();
            this.StubResolve(this.fakeCreator);
            this.scope = Fake.CreateScope();
        }

        protected override void OnTeardown()
        {
            this.scope.Dispose();
            base.OnTeardown();
        }
    }
}