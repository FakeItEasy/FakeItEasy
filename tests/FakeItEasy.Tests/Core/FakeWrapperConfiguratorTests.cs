namespace FakeItEasy.Tests.Core
{
    using System.Reflection;
    using FakeItEasy.Core;
    using FakeItEasy.Creation;
    using FluentAssertions;
    using Xunit;

    public class FakeWrapperConfiguratorTests
    {
        private readonly IFoo faked;
        private readonly FakeWrapperConfigurator<IFoo> wrapperConfigurator;

        public FakeWrapperConfiguratorTests()
        {
            this.faked = A.Fake<IFoo>();

            IFoo wrapped = A.Fake<IFoo>();
            this.wrapperConfigurator = new FakeWrapperConfigurator<IFoo>(A.Fake<IFakeOptions<IFoo>>(), wrapped);
        }

        [Fact]
        public void ConfigureFakeToWrap_should_add_WrappedFakeObjectRule_to_fake_object()
        {
            // Arrange

            // Act
            this.wrapperConfigurator.ConfigureFakeToWrap(this.faked);

            // Assert
            Fake.GetFakeManager(this.faked).Rules
                .Should().Contain(item => item.GetType().IsAssignableFrom(typeof(WrappedObjectRule)));
        }
    }
}
