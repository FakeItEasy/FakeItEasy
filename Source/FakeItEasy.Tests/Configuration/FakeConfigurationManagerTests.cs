using System;
using System.Linq.Expressions;
using System.Reflection;
using FakeItEasy.Configuration;
using FakeItEasy.Core;
using FakeItEasy.Creation;
using FakeItEasy.Expressions;
using FakeItEasy.Tests.TestHelpers;
using NUnit.Framework;

namespace FakeItEasy.Tests.Configuration
{
    [TestFixture]
    public class FakeConfigurationManagerTests
    {
        private IConfigurationFactory configurationFactory;
        private IExpressionParser expressionParser;
        private FakeConfigurationManager configurationManager;
        private ExpressionCallRule.Factory ruleFactory;
        private ExpressionCallRule ruleReturnedFromFactory;
        private FakeManager fakeObjectReturnedFromParser;
        private IProxyGenerator proxyGenerator;
        private ICallExpressionParser callExpressionParser;

        [SetUp]
        public void SetUp()
        {
            this.OnSetUp();
        }

        private void OnSetUp()
        {
            this.configurationFactory = A.Fake<IConfigurationFactory>();
            this.expressionParser = A.Fake<IExpressionParser>();
            this.proxyGenerator = A.Fake<IProxyGenerator>();
            this.callExpressionParser = A.Fake<ICallExpressionParser>();
            A.CallTo(() => this.proxyGenerator.MemberCanBeIntercepted(A<MemberInfo>.Ignored)).Returns(true);

            Expression<Action<IFoo>> dummyExpression = x => x.Bar();
            this.ruleReturnedFromFactory = ServiceLocator.Current.Resolve<ExpressionCallRule.Factory>().Invoke(dummyExpression);
            this.ruleFactory = x =>
                {
                    return this.ruleReturnedFromFactory;
                };

            this.fakeObjectReturnedFromParser = new FakeManager();

            A.CallTo(() => this.expressionParser.GetFakeManagerCallIsMadeOn(A<LambdaExpression>.Ignored)).ReturnsLazily(x => this.fakeObjectReturnedFromParser);

            this.configurationManager = this.CreateManager();
        }

        private FakeConfigurationManager CreateManager()
        {
            return new FakeConfigurationManager(this.configurationFactory, this.expressionParser, this.ruleFactory, this.proxyGenerator, this.callExpressionParser);
        }

        //Callto
        [Test]
        public void CallTo_with_void_call_should_call_configuration_factory_with_fake_object()
        {
            // Arrange
            var foo = A.Fake<IFoo>();

            // Act
            this.configurationManager.CallTo(() => foo.Bar());

            // Assert
            A.CallTo(() => this.configurationFactory.CreateConfiguration(this.fakeObjectReturnedFromParser, A<BuildableCallRule>.Ignored)).MustHaveHappened();
        }

        [Test]
        public void CallTo_with_void_call_should_call_configuration_factory_with_call_rule_from_factory()
        {
            // Arrange
            var foo = A.Fake<IFoo>();

            // Act
            this.configurationManager.CallTo(() => foo.Bar());

            // Assert
            A.CallTo(() => this.configurationFactory.CreateConfiguration(A<FakeManager>.Ignored, this.ruleReturnedFromFactory)).MustHaveHappened();
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

        //[Test]
        //public void CallTo_with_void_call_that_can_not_be_faked_should_throw()
        //{
        //    // Arrange
        //    var foo = A.Fake<Foo>();

        //    Expression<Action> fooMember = () => foo.Bar();
        //    var methodCall = fooMember.Body as MethodCallExpression;

        //    A.CallTo(() => this.proxyGenerator.MemberCanBeIntercepted(methodCall.Method)).Returns(false);

        //    // Act
        //    var thrown = Assert.Throws<FakeConfigurationException>(() =>
        //        this.configurationManager.CallTo(fooMember));

        //    // Assert
        //    Assert.That(thrown.Message, Is.EqualTo("The specified method can not be configured since it can not be intercepted by the current IProxyGenerator."));
        //}


        // CallTo with function calls
        [Test]
        public void CallTo_with_function_call_should_call_configuration_factory_with_fake_object()
        {
            // Arrange
            var foo = A.Fake<IFoo>();

            // Act
            this.configurationManager.CallTo(() => foo.Baz());

            // Assert
            A.CallTo(() => this.configurationFactory.CreateConfiguration<int>(this.fakeObjectReturnedFromParser, A<BuildableCallRule>.Ignored)).MustHaveHappened();
        }

        [Test]
        public void CallTo_with_function_call_should_call_configuration_factory_with_call_rule_from_factory()
        {
            // Arrange
            var foo = A.Fake<IFoo>();

            // Act
            this.configurationManager.CallTo(() => foo.Baz());

            // Assert
            A.CallTo(() => this.configurationFactory.CreateConfiguration<int>(A<FakeManager>.Ignored, this.ruleReturnedFromFactory)).MustHaveHappened();
        }

        [Test]
        public void CallTo_with_function_call_should_return_configuration_from_factory()
        {
            // Arrange
            var foo = A.Fake<IFoo>();
            var returnedConfiguration = A.Fake<IReturnValueArgumentValidationConfiguration<int>>();
            
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
        public void CallTo_with_function_call_that_can_not_be_itercepted_should_throw()
        {
            // Arrange
            var foo = A.Fake<IFoo>();
            A.CallTo(() => this.proxyGenerator.MemberCanBeIntercepted(A<MemberInfo>.Ignored)).Returns(false);
            
            // Act

            // Assert
            Assert.That(() => { this.configurationManager.CallTo(() => foo.Baz()); },
                Throws.Exception.InstanceOf<FakeConfigurationException>()
                .With.Message.EqualTo("The specified method can not be configured since it can not be intercepted by the current IProxyGenerator."));
        }

        //[Test]
        //public void CallTo_with_function_call_that_can_not_be_faked_should_throw()
        //{
        //    // Arrange
        //    var foo = A.Fake<IFoo>();

        //    Expression<Func<int>> fooMember = () => foo.Baz();
        //    var methodCall = fooMember.Body as MethodCallExpression;

        //    A.CallTo(() => this.proxyGenerator.MemberCanBeIntercepted(methodCall.Method)).Returns(false);

        //    // Act
        //    var thrown = Assert.Throws<FakeConfigurationException>(() =>
        //        this.configurationManager.CallTo(fooMember));
            
        //    // Assert
        //    Assert.That(thrown.Message, Is.EqualTo("The specified method can not be configured since it can not be intercepted by the current IProxyGenerator."));
        //}

        //[Test]
        //public void CallTo_with_property_getter_call_that_can_not_be_faked_should_throw()
        //{
        //    // Arrange
        //    var foo = A.Fake<IFoo>();

        //    Expression<Func<int>> fooMember = () => foo.SomeProperty;
        //    var memberAccess = fooMember.Body as MemberExpression;

        //    A.CallTo(() => this.proxyGenerator.MemberCanBeIntercepted(memberAccess.Member)).Returns(false);

        //    // Act
        //    var thrown = Assert.Throws<FakeConfigurationException>(() =>
        //        this.configurationManager.CallTo(fooMember));

        //    // Assert
        //    Assert.That(thrown.Message, Is.EqualTo("The specified member can not be configured since it can not be intercepted by the current IProxyGenerator."));
        //}

        [Test]
        public void CallTo_with_function_call_should_be_null_guarded()
        {
            // Arrange

            // Act

            // Assert
            NullGuardedConstraint.Assert(() =>
                this.configurationManager.CallTo(() => "".Length));
        }
    }
}
