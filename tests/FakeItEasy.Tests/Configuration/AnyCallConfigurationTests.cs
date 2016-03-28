namespace FakeItEasy.Tests.Configuration
{
    using System;
    using FakeItEasy.Configuration;
    using FakeItEasy.Core;
    using FluentAssertions;
    using NUnit.Framework;
    using ExceptionFactory = System.Func<FakeItEasy.Core.IFakeObjectCall, System.Exception>;

    [TestFixture]
    internal class AnyCallConfigurationTests : AutoInitializedFixture
    {
#pragma warning disable 649
        [Fake]
        private IConfigurationFactory configurationFactory;

        [Fake]
        private FakeManager fakeObject;

        [Fake]
        private AnyCallCallRule callRule;

        [UnderTest]
        private AnyCallConfiguration configuration;
#pragma warning restore 649

        [Test]
        public void WithReturnType_should_return_configuration_from_factory()
        {
            // Arrange
            var returnConfig = this.StubReturnConfig<int>();

            // Act
            var result = this.configuration.WithReturnType<int>();

            // Assert
            returnConfig.Should().BeSameAs(result);
        }

        [Test]
        public void WithReturnType_should_set_the_type_to_the_configured_rule()
        {
            // Arrange

            // Act
            this.configuration.WithReturnType<string>();

            // Assert
            this.callRule.ApplicableToMembersWithReturnType.Should().Be(typeof(string));
        }

        [Test]
        public void DoesNothing_delegates_to_configuration_produced_by_factory()
        {
            // Arrange
            var factoryConfig = this.StubVoidConfig();

            // Act
            this.configuration.DoesNothing();

            // Assert
            A.CallTo(() => factoryConfig.DoesNothing()).MustHaveHappened();
        }

        [Test]
        public void DoesNothing_returns_configuration_produced_by_factory()
        {
            // Arrange
            var factoryConfig = this.StubVoidConfig();
            var doesNothingConfig = A.Fake<IAfterCallSpecifiedConfiguration>();
            A.CallTo(() => factoryConfig.DoesNothing()).Returns(doesNothingConfig);

            // Act
            var result = this.configuration.DoesNothing();

            // Assert
            result.Should().BeSameAs(doesNothingConfig);
        }

        [Test]
        public void ThrowsLazily_delegates_to_configuration_produced_by_factory()
        {
            // Arrange
            var factoryConfig = this.StubVoidConfig();

            var exceptionFactory = A.Dummy<ExceptionFactory>();

            // Act
            this.configuration.Throws(exceptionFactory);

            // Assert
            A.CallTo(() => factoryConfig.Throws(exceptionFactory)).MustHaveHappened();
        }

        [Test]
        public void ThrowsLazily_returns_configuration_produced_by_factory()
        {
            // Arrange
            var factoryConfig = this.StubVoidConfig();
            var throwsConfig = A.Fake<IAfterCallSpecifiedConfiguration>();

            A.CallTo(() => factoryConfig.Throws(A<ExceptionFactory>._)).Returns(throwsConfig);

            // Act
            var result = this.configuration.Throws(A.Dummy<ExceptionFactory>());

            // Assert
            result.Should().BeSameAs(throwsConfig);
        }

        [Test]
        public void Invokes_delegates_to_configuration_produced_by_factory()
        {
            // Arrange
            var factoryConfig = this.StubVoidConfig();
            Action<IFakeObjectCall> invocation = x => { };

            // Act
            this.configuration.Invokes(invocation);

            // Assert
            A.CallTo(() => factoryConfig.Invokes(invocation)).MustHaveHappened();
        }

        [Test]
        public void Invokes_returns_configuration_produced_by_factory()
        {
            // Arrange
            Action<IFakeObjectCall> invocation = x => { };

            var factoryConfig = this.StubVoidConfig();
            var invokesConfig = A.Fake<IVoidConfiguration>();

            A.CallTo(() => factoryConfig.Invokes(invocation)).Returns(invokesConfig);

            // Act
            var result = this.configuration.Invokes(invocation);

            // Assert
            result.Should().BeSameAs(invokesConfig);
        }

        [Test]
        public void CallsBaseMethod_delegates_to_configuration_produced_by_factory()
        {
            // Arrange
            var factoryConfig = this.StubVoidConfig();

            // Act
            this.configuration.CallsBaseMethod();

            // Assert
            A.CallTo(() => factoryConfig.CallsBaseMethod()).MustHaveHappened();
        }

        [Test]
        public void CallsBaseMethod_returns_configuration_produced_by_factory()
        {
            // Arrange
            var factoryConfig = this.StubVoidConfig();
            var callsBaseConfig = A.Fake<IAfterCallSpecifiedConfiguration>();

            A.CallTo(() => factoryConfig.CallsBaseMethod()).Returns(callsBaseConfig);

            // Act
            var result = this.configuration.CallsBaseMethod();

            // Assert
            result.Should().BeSameAs(callsBaseConfig);
        }

        [Test]
        public void AssignsOutAndRefParametersLazily_delegates_to_configuration_produced_by_factory()
        {
            // Arrange
            Func<IFakeObjectCall, object[]> valueProducer = x => new object[] { "a", "b" };

            var factoryConfig = this.StubVoidConfig();
            var nextConfig = A.Fake<IAfterCallSpecifiedConfiguration>();

            A.CallTo(() => factoryConfig.AssignsOutAndRefParametersLazily(valueProducer)).Returns(nextConfig);

            // Act
            this.configuration.AssignsOutAndRefParametersLazily(valueProducer);

            // Assert
            A.CallTo(() => factoryConfig.AssignsOutAndRefParametersLazily(valueProducer)).MustHaveHappened();
        }

        [Test]
        public void AssignsOutAndRefParametersLazily_returns_configuration_produced_by_factory()
        {
            // Arrange
            Func<IFakeObjectCall, object[]> valueProducer = x => new object[] { "a", "b" };

            var factoryConfig = this.StubVoidConfig();
            var nextConfig = A.Fake<IAfterCallSpecifiedConfiguration>();

            A.CallTo(() => factoryConfig.AssignsOutAndRefParametersLazily(valueProducer)).Returns(nextConfig);

            // Act
            var result = this.configuration.AssignsOutAndRefParametersLazily(valueProducer);

            // Assert
            result.Should().BeSameAs(nextConfig);
        }

        [Test]
        public void Assert_delegates_to_configuration_produced_by_factory()
        {
            // Arrange
            var factoryConfig = this.StubVoidConfig();

            // Act
            this.configuration.MustHaveHappened(Repeated.AtLeast.Twice);

            // Assert
            A.CallTo(() => factoryConfig.MustHaveHappened(A<Repeated>._)).MustHaveHappened();
        }

        [Test]
        public void Where_should_apply_where_predicate_on_rule()
        {
            // Arrange
            Func<IFakeObjectCall, bool> predicate = x => true;
            Action<IOutputWriter> writer = x => { };

            // Act
            this.configuration.Where(predicate, writer);

            // Assert
            A.CallTo(() => this.callRule.ApplyWherePredicate(predicate, writer)).MustHaveHappened();
        }

        [Test]
        public void Where_should_return_self()
        {
            // Arrange

            // Act

            // Assert
            this.configuration.Where(x => true, x => { }).Should().BeSameAs(this.configuration);
        }

        [Test]
        public void WhenArgumentsMatch_should_delegate_to_buildable_rule()
        {
            // Arrange
            Func<ArgumentCollection, bool> predicate = x => true;

            // Act
            this.configuration.WhenArgumentsMatch(predicate);

            // Assert
            A.CallTo(() => this.callRule.UsePredicateToValidateArguments(predicate))
                .MustHaveHappened();
        }

        [Test]
        public void WhenArgumentsMatch_should_return_self()
        {
            // Arrange

            // Act

            // Assert
            this.configuration.WhenArgumentsMatch(x => true).Should().BeSameAs(this.configuration);
        }

        private IVoidArgumentValidationConfiguration StubVoidConfig()
        {
            var result = A.Fake<IVoidArgumentValidationConfiguration>();

            A.CallTo(() => this.configurationFactory.CreateConfiguration(this.fakeObject, this.callRule)).Returns(result);

            return result;
        }

        private IReturnValueArgumentValidationConfiguration<T> StubReturnConfig<T>()
        {
            var result = A.Fake<IAnyCallConfigurationWithReturnTypeSpecified<T>>();

            A.CallTo(() => this.configurationFactory.CreateConfiguration<T>(this.fakeObject, this.callRule)).Returns(result);

            return result;
        }
    }
}
