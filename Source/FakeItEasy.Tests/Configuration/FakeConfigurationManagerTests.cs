namespace FakeItEasy.Tests.Configuration
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using FakeItEasy.Configuration;
    using FakeItEasy.Core;
    using FakeItEasy.Expressions;
    using NUnit.Framework;

    [TestFixture]
    public class FakeConfigurationManagerTests
    {
        private IConfigurationFactory configurationFactory;
        private IExpressionParser expressionParser;
        private FakeConfigurationManager configurationManager;
        private ExpressionCallRule.Factory ruleFactory;
        private ExpressionCallRule ruleReturnedFromFactory;
        private FakeManager fakeObjectReturnedFromParser;
        private CallExpressionParser callExpressionParser;
        private IInterceptionAsserter interceptionAsserter;

        [SetUp]
        public void Setup()
        {
            this.OnSetup();
        }

        // Callto
        [Test]
        public void CallTo_with_void_call_should_call_configuration_factory_with_fake_object()
        {
            // Arrange
            var foo = A.Fake<IFoo>();

            // Act
            this.configurationManager.CallTo(() => foo.Bar());

            // Assert
            A.CallTo(() => this.configurationFactory.CreateConfiguration(this.fakeObjectReturnedFromParser, A<BuildableCallRule>._)).MustHaveHappened();
        }

        [Test]
        public void CallTo_with_void_call_should_call_configuration_factory_with_call_rule_from_factory()
        {
            // Arrange
            var foo = A.Fake<IFoo>();

            // Act
            this.configurationManager.CallTo(() => foo.Bar());

            // Assert
            A.CallTo(() => this.configurationFactory.CreateConfiguration(A<FakeManager>._, this.ruleReturnedFromFactory)).MustHaveHappened();
        }

        [Test]
        public void CallTo_with_void_call_should_return_configuration_from_factory()
        {
            // Arrange
            var foo = A.Fake<IFoo>();
            var returnedConfiguration = A.Fake<IVoidArgumentValidationConfiguration>();

            A.CallTo(() =>
                this.configurationFactory.CreateConfiguration(this.fakeObjectReturnedFromParser, this.ruleReturnedFromFactory))
                .Returns(returnedConfiguration);

            // Act
            var result = this.configurationManager.CallTo(() => foo.Bar());

            // Assert
            Assert.That(result, Is.SameAs(returnedConfiguration));
        }

        [Test]
        public void CallTo_with_void_call_should_add_rule_to_fake_object()
        {
            // Arrange
            var foo = A.Fake<IFoo>();

            // Act
            this.configurationManager.CallTo(() => foo.Bar());

            // Assert
            Assert.That(this.fakeObjectReturnedFromParser.Rules, Has.Some.EqualTo(this.ruleReturnedFromFactory));
        }

        [Test]
        public void CallTo_with_void_call_should_be_null_guarded()
        {
            // Arrange
            var foo = A.Fake<IFoo>();

            // Act

            // Assert
            NullGuardedConstraint.Assert(() =>
                this.configurationManager.CallTo(() => foo.Bar()));
        }

        // CallTo with function calls
        [Test]
        public void CallTo_with_function_call_should_call_configuration_factory_with_fake_object()
        {
            // Arrange
            var foo = A.Fake<IFoo>();

            // Act
            this.configurationManager.CallTo(() => foo.Baz());

            // Assert
            A.CallTo(() => this.configurationFactory.CreateConfiguration<int>(this.fakeObjectReturnedFromParser, A<BuildableCallRule>._)).MustHaveHappened();
        }

        [Test]
        public void CallTo_with_function_call_should_call_configuration_factory_with_call_rule_from_factory()
        {
            // Arrange
            var foo = A.Fake<IFoo>();

            // Act
            this.configurationManager.CallTo(() => foo.Baz());

            // Assert
            A.CallTo(() => this.configurationFactory.CreateConfiguration<int>(A<FakeManager>._, this.ruleReturnedFromFactory)).MustHaveHappened();
        }

        [Test]
        public void CallTo_with_function_call_should_return_configuration_from_factory()
        {
            // Arrange
            var foo = A.Fake<IFoo>();
            var returnedConfiguration = A.Fake<IAnyCallConfigurationWithReturnTypeSpecified<int>>();

            A.CallTo(() => this.configurationFactory.CreateConfiguration<int>(this.fakeObjectReturnedFromParser, this.ruleReturnedFromFactory)).Returns(returnedConfiguration);

            // Act
            var result = this.configurationManager.CallTo(() => foo.Baz());

            // Assert
            Assert.That(result, Is.SameAs(returnedConfiguration));
        }

        [Test]
        public void CallTo_with_function_call_should_add_rule_to_fake_object()
        {
            // Arrange
            var foo = A.Fake<IFoo>();

            // Act
            this.configurationManager.CallTo(() => foo.Baz());

            // Assert
            Assert.That(this.fakeObjectReturnedFromParser.Rules, Has.Some.EqualTo(this.ruleReturnedFromFactory));
        }

        [Test]
        public void CallTo_with_function_call_should_be_null_guarded()
        {
            // Arrange

            // Act

            // Assert
            NullGuardedConstraint.Assert(() =>
                this.configurationManager.CallTo(() => string.Empty.Length));
        }

        [Test]
        public void Should_call_interception_asserter_when_configuring_function_call()
        {
            // Arrange
            var foo = A.Fake<IFoo>();
            Expression<Func<int>> call = () => foo.Baz();

            var parsedCall = this.callExpressionParser.Parse(call);

            // Act
            this.configurationManager.CallTo(call);

            // Assert
            A.CallTo(() => this.interceptionAsserter.AssertThatMethodCanBeInterceptedOnInstance(
                parsedCall.CalledMethod,
                parsedCall.CallTarget)).MustHaveHappened();
        }

        [Test]
        public void Should_call_interception_asserter_when_configuring_void_call()
        {
            // Arrange
            var foo = A.Fake<IFoo>();
            Expression<Action> call = () => foo.Bar();

            var parsedCall = this.callExpressionParser.Parse(call);

            // Act
            this.configurationManager.CallTo(call);

            // Assert
            A.CallTo(() => this.interceptionAsserter.AssertThatMethodCanBeInterceptedOnInstance(
                parsedCall.CalledMethod,
                parsedCall.CallTarget)).MustHaveHappened();
        }

        [Test]
        public void Should_call_configuration_factory_with_manager_from_fake()
        {
            // Arrange
            var fake = A.Fake<IFoo>();
            var manager = Fake.GetFakeManager(fake);

            // Act
            this.configurationManager.CallTo(fake);

            // Assert
            A.CallTo(() => this.configurationFactory.CreateAnyCallConfiguration(
                manager, A<AnyCallCallRule>.That.Not.IsNull())).MustHaveHappened();
        }

        [Test]
        public void Should_return_configuration_when_configuring_any_call()
        {
            // Arrange
            var fake = A.Fake<IFoo>();
            var expectedConfiguration = A.Dummy<IAnyCallConfigurationWithNoReturnTypeSpecified>();
            A.CallTo(() => this.configurationFactory.CreateAnyCallConfiguration(
                A<FakeManager>._, A<AnyCallCallRule>._))
                .Returns(expectedConfiguration);

            // Act
            var result = this.configurationManager.CallTo(fake);

            // Assert
            Assert.That(result, Is.SameAs(expectedConfiguration));
        }

        [Test]
        public void Should_add_call_rule_to_fake_manager_when_configuring_any_call()
        {
            // Arrange
            var fake = A.Fake<IFoo>();
            var manager = Fake.GetFakeManager(fake);

            // Act
            this.configurationManager.CallTo(fake);

            // Assert
            Assert.That(manager.AllUserRules.Single().Rule, Is.InstanceOf<AnyCallCallRule>());
        }

        private void OnSetup()
        {
            this.configurationFactory = A.Fake<IConfigurationFactory>();
            this.expressionParser = A.Fake<IExpressionParser>();
            this.callExpressionParser = new CallExpressionParser();
            this.interceptionAsserter = A.Fake<IInterceptionAsserter>();

            Expression<Action<IFoo>> dummyExpression = x => x.Bar();
            this.ruleReturnedFromFactory = ServiceLocator.Current.Resolve<ExpressionCallRule.Factory>().Invoke(dummyExpression);
            this.ruleFactory = x =>
            {
                return this.ruleReturnedFromFactory;
            };

            this.fakeObjectReturnedFromParser = A.Fake<FakeManager>(o => o.CallsBaseMethods());

            A.CallTo(() => this.expressionParser.GetFakeManagerCallIsMadeOn(A<LambdaExpression>._)).ReturnsLazily(x => this.fakeObjectReturnedFromParser);

            this.configurationManager = this.CreateManager();
        }

        private FakeConfigurationManager CreateManager()
        {
            return new FakeConfigurationManager(
                this.configurationFactory,
                this.expressionParser,
                this.ruleFactory,
                this.callExpressionParser,
                this.interceptionAsserter);
        }
    }
}
