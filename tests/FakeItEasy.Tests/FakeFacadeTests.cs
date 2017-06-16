namespace FakeItEasy.Tests
{
    using System.Linq.Expressions;
    using FakeItEasy.Core;
    using FluentAssertions;
    using Xunit;

    public class FakeFacadeTests
    {
        private readonly IFakeManagerAccessor managerAccessor;

        private readonly FakeFacade facade;

        public FakeFacadeTests()
        {
            this.managerAccessor = A.Fake<IFakeManagerAccessor>();

            this.facade = new FakeFacade(this.managerAccessor);
        }

        [Fact]
        public void GetFakeManager_should_return_manager_from_manager_accessor()
        {
            // Arrange
            var manager = A.Dummy<FakeManager>();
            var proxy = A.Dummy<object>();

            A.CallTo(() => this.managerAccessor.GetFakeManager(proxy)).Returns(manager);

            // Act
            var result = this.facade.GetFakeManager(proxy);

            // Assert
            result.Should().BeSameAs(manager);
        }

        [Fact]
        public void GetFakeManager_should_be_guarded()
        {
            // Arrange

            // Act

            // Assert
            Expression<System.Action> call = () => this.facade.GetFakeManager(A.Dummy<IFoo>());
            call.Should().BeNullGuarded();
        }

        [Fact]
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

        [Fact]
        public void GetCalls_should_be_guarded()
        {
            // Arrange

            // Act

            // Assert
            Expression<System.Action> call = () => this.facade.GetCalls(A.Dummy<object>());
            call.Should().BeNullGuarded();
        }

        [Fact]
        public void ClearConfiguration_should_be_guarded()
        {
            // Arrange

            // Act

            // Assert
            Expression<System.Action> call = () => this.facade.ClearConfiguration(A.Dummy<object>());
            call.Should().BeNullGuarded();
        }

        [Fact]
        public void ClearRecordedCalls_should_be_guarded()
        {
            // Arrange

            // Act

            // Assert
            Expression<System.Action> call = () => this.facade.ClearRecordedCalls(A.Dummy<object>());
            call.Should().BeNullGuarded();
        }

        [Fact]
        public void Initialize_should_be_guarded()
        {
            // Arrange

            // Act

            // Assert
            Expression<System.Action> call = () => this.facade.InitializeFixture(new object());
            call.Should().BeNullGuarded();
        }
    }
}
