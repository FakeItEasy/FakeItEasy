namespace FakeItEasy.Tests.Core
{
    using System.Linq;
    using System.Reflection;
    using FakeItEasy.Core;
    using FakeItEasy.Creation;
#if FEATURE_SELF_INITIALIZED_FAKES
    using FakeItEasy.SelfInitializedFakes;
#endif
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

#if FEATURE_SELF_INITIALIZED_FAKES
        [Fact]
        public void ConfigureFakeToWrap_should_add_self_initialization_rule_when_recorder_is_specified()
        {
            // Arrange
            this.wrapperConfigurator.RecordedBy(A.Fake<ISelfInitializingFakeRecorder>());

            // Act
            this.wrapperConfigurator.ConfigureFakeToWrap(this.faked);

            // Assert
            Fake.GetFakeManager(this.faked).Rules.First()
                .Should().BeOfType<SelfInitializationRule>();
        }

        [Fact]
        public void ConfigureFakeToWrap_should_not_add_self_initialization_rule_when_recorder_is_not_specified()
        {
            // Arrange

            // Act
            this.wrapperConfigurator.ConfigureFakeToWrap(this.faked);

            // Assert
            Fake.GetFakeManager(this.faked).Rules
                .Should().NotContain(item => item.GetType().IsAssignableFrom(typeof(SelfInitializationRule)));
        }
#endif
    }
}
