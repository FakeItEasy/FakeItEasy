namespace FakeItEasy.Tests.Configuration
{
    using System;
    using System.Linq.Expressions;
    using FakeItEasy.Configuration;
    using FakeItEasy.Core;
    using NUnit.Framework;

    [TestFixture]
    public class AnyCallConfigurationTests
    {
        private IConfigurationFactory configurationFactory;
        private FakeManager fakeObject;
        private AnyCallCallRule callRule;
        private AnyCallConfiguration configuration;

        [SetUp]
        public void SetUp()
        {
            this.configurationFactory = A.Fake<IConfigurationFactory>();
            this.fakeObject = new FakeManager();
            this.callRule = A.Fake<AnyCallCallRule>();

            this.configuration = this.CreateConfiguration();
        }

        private AnyCallConfiguration CreateConfiguration()
        {
            return new AnyCallConfiguration(this.fakeObject, this.callRule, this.configurationFactory);
        }

        [Test]
        public void WithReturnType_should_return_configuration_from_factory()
        {
            // Arrange
            var returnConfig = this.StubReturnConfig<int>();

            // Act
            var result = this.configuration.WithReturnType<int>();

            // Assert
            Assert.That(returnConfig, Is.SameAs(result));
        }

        [Test]
        public void WithReturnType_should_set_the_type_to_the_configured_rule()
        {
            // Arrange
            
            // Act
            this.configuration.WithReturnType<string>();

            // Assert
            Assert.That(this.callRule.ApplicableToMembersWithReturnType, Is.EqualTo(typeof(string)));
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
            Assert.That(result, Is.SameAs(doesNothingConfig));
        }

        [Test]
        public void Throws_delegates_to_configuration_produced_by_factory()
        {
            // Arrange
            var factoryConfig = this.StubVoidConfig();
            var ex = new Exception();
            
            // Act
            this.configuration.Throws(ex);

            // Assert
            A.CallTo(() => factoryConfig.Throws(ex)).MustHaveHappened();
        }

        [Test]
        public void Throws_returns_configuration_produced_by_factory()
        {
            // Arrange
            var factoryConfig = this.StubVoidConfig();
            var throwsConfig = A.Fake<IAfterCallSpecifiedConfiguration>();

            A.CallTo(() => factoryConfig.Throws(A<Exception>._)).Returns(throwsConfig);

            // Act
            var result = this.configuration.Throws(new Exception());

            // Assert
            Assert.That(result, Is.SameAs(throwsConfig));
        }

        [Test]
        public void Invokes_delegates_to_configuration_produced_by_factory()
        {
            // Arrange
            var factoryConfig = this.StubVoidConfig();
            Action<IFakeObjectCall> invokation = x => { };

            // Act
            this.configuration.Invokes(invokation);

            // Assert
            A.CallTo(() => factoryConfig.Invokes(invokation)).MustHaveHappened();
        }

        [Test]
        public void Invokes_returns_configuration_produced_by_factory()
        {
            // Arrange
            Action<IFakeObjectCall> invokation = x => { };

            var factoryConfig = this.StubVoidConfig();
            var invokesConfig = A.Fake<IVoidConfiguration>();

            A.CallTo(() => factoryConfig.Invokes(invokation)).Returns(invokesConfig);

            // Act
            var result = this.configuration.Invokes(invokation);

            // Assert
            Assert.That(result, Is.SameAs(invokesConfig));
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
            Assert.That(result, Is.SameAs(callsBaseConfig));
        }

        [Test]
        public void AssignsOutAndRefParameters_delegates_to_configuration_produced_by_factory()
        {
            // Arrange
            var parameters = new object[] { "a", "b" };

            var factoryConfig = this.StubVoidConfig();
            var nextConfig = A.Fake<IAfterCallSpecifiedConfiguration>();

            A.CallTo(() => factoryConfig.AssignsOutAndRefParameters(parameters)).Returns(nextConfig);

            // Act
            this.configuration.AssignsOutAndRefParameters(parameters);

            // Assert
            A.CallTo(() => factoryConfig.AssignsOutAndRefParameters(parameters)).MustHaveHappened();
        }

        [Test]
        public void AssignsOutAndRefParameters_returns_configuration_produced_by_factory()
        {
            // Arrange
            var parameters = new object[] { "a", "b" };

            var factoryConfig = this.StubVoidConfig();
            var nextConfig = A.Fake<IAfterCallSpecifiedConfiguration>();

            A.CallTo(() => factoryConfig.AssignsOutAndRefParameters(parameters)).Returns(nextConfig);

            // Act
            var result = this.configuration.AssignsOutAndRefParameters(parameters);

            // Assert
            Assert.That(result, Is.SameAs(nextConfig));
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
            Expression<Func<IFakeObjectCall, bool>> predicate = x => true;

            // Act
            this.configuration.Where(predicate);

            // Assert
            A.CallTo(() => this.callRule.ApplyWherePredicate(predicate)).MustHaveHappened();
        }

        [Test]
        public void Where_should_return_self()
        {
            // Arrange

            // Act

            // Assert
            Assert.That(this.configuration.Where(x => true), Is.SameAs(this.configuration));
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
            Assert.That(this.configuration.WhenArgumentsMatch(x => true), Is.SameAs(this.configuration));
        }

        private IVoidArgumentValidationConfiguration StubVoidConfig()
        {
            var result = A.Fake<IVoidArgumentValidationConfiguration>();

            A.CallTo(() => this.configurationFactory.CreateConfiguration(this.fakeObject, this.callRule)).Returns(result);

            return result;
        }

        private IReturnValueArgumentValidationConfiguration<T> StubReturnConfig<T>()
        {
            var result = A.Fake<IReturnValueArgumentValidationConfiguration<T>>();

            A.CallTo(() => this.configurationFactory.CreateConfiguration<T>(this.fakeObject, this.callRule)).Returns(result);

            return result;
        }
    }
}
