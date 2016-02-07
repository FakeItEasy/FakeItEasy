namespace FakeItEasy.Tests
{
    using FakeItEasy.Core;
    using NUnit.Framework;

    [TestFixture]
    public class FakeFacadeTests
    {
        private IFakeManagerAccessor managerAccessor;
        private IFakeScopeFactory fakeScopeFactory;
        private IFixtureInitializer fixtureInitializer;

        private FakeFacade facade;

        [SetUp]
        public void Setup()
        {
            this.managerAccessor = A.Fake<IFakeManagerAccessor>();
            this.fakeScopeFactory = A.Fake<IFakeScopeFactory>();
            this.fixtureInitializer = A.Fake<IFixtureInitializer>();

            this.facade = new FakeFacade(this.managerAccessor, this.fakeScopeFactory, this.fixtureInitializer);
        }

        [Test]
        public void GetFakeManager_should_return_manager_from_manager_accessor()
        {
            // Arrange
            var manager = A.Dummy<FakeManager>();
            var proxy = A.Dummy<object>();

            A.CallTo(() => this.managerAccessor.GetFakeManager(proxy)).Returns(manager);

            // Act
            var result = this.facade.GetFakeManager(proxy);

            // Assert
            Assert.That(result, Is.SameAs(manager));
        }

        [Test]
        public void GetFakeManager_should_be_guarded()
        {
            // Arrange

            // Act

            // Assert
            NullGuardedConstraint.Assert(() =>
                this.facade.GetFakeManager(A.Dummy<IFoo>()));
        }

        [Test]
        public void CreateScope_should_return_scope_from_scope_factory()
        {
            // Arrange
            var scope = A.Dummy<IFakeScope>();
            A.CallTo(() => this.fakeScopeFactory.Create()).Returns(scope);

            // Act
            var result = this.facade.CreateScope();

            // Assert
            Assert.That(result, Is.SameAs(scope));
        }

        [Test]
        public void CreateScope_with_container_should_return_scope_from_factory()
        {
            // Arrange
            var container = A.Dummy<IFakeObjectContainer>();

            var scope = A.Dummy<IFakeScope>();
            A.CallTo(() => this.fakeScopeFactory.Create(container)).Returns(scope);

            // Act
            var result = this.facade.CreateScope(container);

            // Assert
            Assert.That(result, Is.SameAs(scope));
        }

        [Test]
        public void CreateScope_with_container_should_be_guarded()
        {
            // Arrange

            // Act

            // Assert
            NullGuardedConstraint.Assert(() =>
                this.facade.CreateScope(A.Dummy<IFakeObjectContainer>()));
        }

        [Test]
        public void GetCalls_should_return_calls_from_manager_received_from_accessor()
        {
            // Arrange
            var fake = A.Dummy<object>();
            var manager = A.Fake<FakeManager>();
            var calls = A.CollectionOfFake<ICompletedFakeObjectCall>(2);

            A.CallTo(() => manager.RecordedCallsInScope).Returns(calls);
            A.CallTo(() => this.managerAccessor.GetFakeManager(fake)).Returns(manager);

            // Act
            var result = this.facade.GetCalls(fake);

            // Assert
            Assert.That(result, Is.EquivalentTo(calls));
        }

        [Test]
        public void ClearConfiguration_should_call_clear_configuration_on_manager()
        {
            // Arrange
            var fake = A.Dummy<object>();
            var manager = A.Fake<FakeManager>();

            A.CallTo(() => this.managerAccessor.GetFakeManager(fake)).Returns(manager);

            // Act
            this.facade.ClearConfiguration(fake);

            // Assert
            A.CallTo(() => manager.ClearUserRules()).MustHaveHappened();
        }

        [Test]
        public void GetCalls_should_be_guarded()
        {
            // Arrange

            // Act

            // Assert
            NullGuardedConstraint.Assert(() =>
                this.facade.GetCalls(A.Dummy<object>()));
        }

        [Test]
        public void ClearConfiguration_should_be_guarded()
        {
            // Arrange

            // Act

            // Assert
            NullGuardedConstraint.Assert(() =>
                this.facade.ClearConfiguration(A.Dummy<object>()));
        }

        [Test]
        public void Initialize_should_call_initialize_on_fake_initializer()
        {
            // Arrange
            var o = A.Dummy<object>();

            // Act
            this.facade.InitializeFixture(o);

            // Assert
            A.CallTo(() => this.fixtureInitializer.InitializeFakes(o)).MustHaveHappened();
        }

        [Test]
        public void Initialize_should_be_guarded()
        {
            // Arrange

            // Act

            // Assert
            NullGuardedConstraint.Assert(() =>
                this.facade.InitializeFixture(new object()));
        }
    }
}
