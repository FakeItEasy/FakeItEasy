namespace FakeItEasy.Tests
{
    using System;
    using System.Collections.Generic;
    using FakeItEasy.Creation;
    using NUnit.Framework;

    [TestFixture]
    public class ATests : ConfigurableServiceLocatorTestBase
    {
        private IFakeCreatorFacade fakeCreator;
        private IDisposable scope;

        [Test]
        public void Static_equals_delegates_to_static_method_on_object()
        {
            Assert.That(A.Equals("foo", "foo"), Is.True);
        }

        [Test]
        public void Static_ReferenceEquals_delegates_to_static_method_on_object()
        {
            var s = string.Empty;

            Assert.That(A.ReferenceEquals(s, s), Is.True);
        }

        [Test]
        public void Fake_without_arguments_should_call_fake_creator_with_empty_action()
        {
            // Arrange
            var fake = A.Fake<IFoo>();
            A.CallTo(() => this.fakeCreator.CreateFake<IFoo>(A<Action<IFakeOptionsBuilder<IFoo>>>._)).Returns(fake);

            // Act
            var result = A.Fake<IFoo>();

            // Assert
            Assert.That(result, Is.SameAs(fake));
        }

        [Test]
        public void Fake_with_arguments_should_call_fake_creator_with_specified_options()
        {
            // Arrange
            var fake = A.Fake<IFoo>();

            Action<IFakeOptionsBuilder<IFoo>> options = x => { };
            A.CallTo(() => this.fakeCreator.CreateFake<IFoo>(options)).Returns(fake);

            // Act
            var result = A.Fake<IFoo>(options);

            // Assert
            Assert.That(result, Is.SameAs(fake));
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
            Assert.That(result, Is.SameAs(dummy));
        }

        [Test]
        public void CollectionOfFakes_should_delegate_to_fake_creator()
        {
            // Arrange            
            var returnedFromCreator = new List<IFoo>();

            var creator = this.StubResolveWithFake<IFakeCreatorFacade>();
            A.CallTo(() => creator.CollectionOfFake<IFoo>(10)).Returns(returnedFromCreator);

            // Act
            var result = A.CollectionOfFake<IFoo>(10);

            // Assert
            Assert.That(result, Is.SameAs(returnedFromCreator));
        }

        protected override void OnSetup()
        {
            base.OnSetup();
            this.fakeCreator = A.Fake<IFakeCreatorFacade>();
            this.StubResolve<IFakeCreatorFacade>(this.fakeCreator);
            this.scope = Fake.CreateScope();
        }

        protected override void OnTeardown()
        {
            this.scope.Dispose();
            base.OnTeardown();
        }
    }
}