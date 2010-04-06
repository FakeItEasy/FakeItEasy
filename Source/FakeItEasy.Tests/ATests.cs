using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using FakeItEasy.Core;
using System.Diagnostics;
using FakeItEasy.Core;
using FakeItEasy.SelfInitializedFakes;
using System.Linq.Expressions;
using FakeItEasy.Configuration;
using FakeItEasy.Expressions;
using FakeItEasy.Tests.TestHelpers;
using FakeItEasy.Core.Creation;

namespace FakeItEasy.Tests
{
    [TestFixture]
    public class ATests
        : ConfigurableServiceLocatorTestBase
    {
        private IFakeCreator fakeCreator;

        protected override void OnSetUp()
        {
            this.fakeCreator = A.Fake<IFakeCreator>();

            this.StubResolve<IFakeCreator>(this.fakeCreator);
        }

        [Test]
        public void Static_equals_delegates_to_static_method_on_object()
        {
            Assert.That(A.Equals("foo", "foo"), Is.True);
        }

        [Test]
        public void Static_ReferenceEquals_delegates_to_static_method_on_object()
        {
            var s = "";

            Assert.That(A.ReferenceEquals(s, s), Is.True);
        }

        [Test]
        public void Fake_without_arguments_should_call_fake_creator_with_empty_action()
        {
            // Arrange
            var fake = A.Fake<IFoo>();
            A.CallTo(() => this.fakeCreator.CreateFake<IFoo>(A<Action<IFakeOptionsBuilder<IFoo>>>.Ignored)).Returns(fake);


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
    }

    [TestFixture]
    public class ACallToTests
        : ConfigurableServiceLocatorTestBase
    {
        IFakeConfigurationManager configurationManager;

        protected override void OnSetUp()
        {
            this.configurationManager = A.Fake<IFakeConfigurationManager>(x => x.Wrapping(ServiceLocator.Current.Resolve<IFakeConfigurationManager>()));
            this.StubResolve<IFakeConfigurationManager>(this.configurationManager);
        }
        
        [Test]
        public void CallTo_with_void_call_should_return_configuration_from_configuration_manager()
        {
            // Arrange
            var foo = A.Fake<IFoo>();
            Expression<Action> callSpecification = () => foo.Bar();

            var configuration = A.Fake<IVoidArgumentValidationConfiguration>();
            A.CallTo(() => this.configurationManager.CallTo(callSpecification)).Returns(configuration);

            // Act
            var result = A.CallTo(callSpecification);

            // Assert
            Assert.That(result, Is.SameAs(configuration));
        }

        [Test]
        public void CallTo_with_function_call_should_return_configuration_from_configuration_manager()
        {
            // Arrange
            var foo = A.Fake<IFoo>();
            Expression<Func<int>> callSpecification = () => foo.Baz();

            var configuration = A.Fake<IReturnValueArgumentValidationConfiguration<int>>();
            A.CallTo(() => this.configurationManager.CallTo(callSpecification)).Returns(configuration);

            // Act
            var result = A.CallTo(callSpecification);

            // Assert
            Assert.That(result, Is.SameAs(configuration));
        }
    }

    [TestFixture]
    public class GenericATests
    {
        [Test]
        public void That_should_return_root_validations()
        {
            // Arrange

            // Act
            var validations = A<string>.That;

            // Assert
            Assert.That(validations, Is.InstanceOf<RootValidations<string>>());
        }

        [Test]
        public void Ignored_should_return_validator_that_passes_any_argument(
            [Values(null, "", "hello world", "foo")] string argument)
        {
            // Arrange

            // Act
            var isValid = A<string>.Ignored.IsValid(argument);

            // Assert
            Assert.That(isValid, Is.True);
        }

        [Test]
        public void Ignored_should_return_validator_with_correct_description()
        {
            // Arrange
            
            // Act
            var description = A<string>.Ignored.ToString();

            // Assert
            Assert.That(description, Is.EqualTo("<Ignored>"));
        }

        [Test]
        public void Ignored_should_return_validator_with_root_validations_set()
        {
            // Arrange

            // Act
            var validator = A<string>.Ignored;

            // Assert
            Assert.That(validator.Scope, Is.InstanceOf<RootValidations<string>>());
        }
    }
}