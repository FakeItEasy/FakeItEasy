namespace FakeItEasy.Tests.Creation
{
    using System;
    using FakeItEasy.Core;
    using FakeItEasy.Creation;
    using NUnit.Framework;

    [TestFixture]
    public class DefaultFakeAndDummyManagerTests
    {
        [Fake] private IDummyValueCreationSession dummySession;
        [Fake] private FakeObjectCreator fakeCreator;
        [Fake] private IFakeWrapperConfigurer wrapperConfigurer;
        
        private DefaultFakeAndDummyManager manager;

        [SetUp]
        public void SetUp()
        {
            Fake.InitializeFixture(this);

            this.manager = new DefaultFakeAndDummyManager(
                this.dummySession,
                this.fakeCreator,
                this.wrapperConfigurer);
        }

        [Test]
        public void Should_perform_actions_from_options_on_created_fake_when_creating_fake()
        {
            // Arrange
            object invokedWithFake = null;
            
            var options = new FakeOptions();

            options.OnFakeCreatedActions.Add(x => invokedWithFake = x);

            var fake = A.Dummy<IFoo>();

            A.CallTo(() => 
                this.fakeCreator.CreateFake(typeof(IFoo), options, this.dummySession, true))
                .Returns(fake);
            
            // Act
            this.manager.CreateFake(typeof(IFoo), options);

            // Assert
            Assert.That(invokedWithFake, Is.SameAs(fake));
        }

        [Test]
        public void Should_perform_actions_from_options_on_created_fake_when_trying_to_create_fake()
        {
            // Arrange
            object invokedWithFake = null;

            var options = new FakeOptions();

            options.OnFakeCreatedActions.Add(x => invokedWithFake = x);

            var fake = A.Dummy<IFoo>();

            A.CallTo(() =>
                this.fakeCreator.CreateFake(typeof(IFoo), options, this.dummySession, false))
                .Returns(fake);

            // Act
            this.manager.TryCreateFake(typeof(IFoo), options, out Ignore.This<object>().Value);

            // Assert
            Assert.That(invokedWithFake, Is.SameAs(fake));
        }

        [Test]
        public void Should_not_perform_actions_froM_options_when_trying_to_create_fake_but_fails()
        {
            // Arrange
            var wasInvoked = false;

            var options = new FakeOptions();

            options.OnFakeCreatedActions.Add(x => wasInvoked = true);

            A.CallTo(() =>
                this.fakeCreator.CreateFake(A<Type>._, A<FakeOptions>._, A<IDummyValueCreationSession>._, A<bool>._))
                .Returns(null);

            // Act
            this.manager.TryCreateFake(typeof(IFoo), options, out Ignore.This<object>().Value);

            // Assert
            Assert.That(wasInvoked, Is.False);
        }
    }
}
