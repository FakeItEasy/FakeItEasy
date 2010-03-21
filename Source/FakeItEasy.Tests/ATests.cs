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
        private FakeObjectFactory factory;
        
        protected override void OnSetUp()
        {
            this.factory = A.Fake<FakeObjectFactory>(x => x.Wrapping(
                new FakeObjectFactory(ServiceLocator.Current.Resolve<IFakeObjectContainer>(), ServiceLocator.Current.Resolve<IProxyGenerator>(), ServiceLocator.Current.Resolve<FakeObject.Factory>())));

            this.StubResolve<FakeObjectFactory>(this.factory);
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
        public void Generic_Fake_with_no_arguments_should_call_fake_object_factory_with_correct_arguments()
        {
            A.Fake<IFoo>();

            OldFake.Assert(this.factory).WasCalled(x => x.CreateFake(typeof(IFoo), null, false));
        }

        [Test]
        public void Generic_Fake_with_no_arguments_should_return_fake_from_factory()
        {
            var foo = A.Fake<IFoo>();

            using (Fake.CreateScope())
            {
                Configure.Fake(this.factory)
                    .CallsTo(x => x.CreateFake(typeof(IFoo), null, false))
                    .Returns(foo);

                var returned = A.Fake<IFoo>();

                Assert.That(returned, Is.SameAs(foo));
            }
        }


        [Test]
        public void Dummy_should_return_fake_from_factory()
        {
            Configure.Fake(this.factory)
                .CallsTo(x => x.CreateFake(typeof(string), A<IEnumerable<object>>.Ignored.Argument, true))
                .Returns("return this");

            var result = A.Dummy<string>();

            Assert.That(result, Is.EqualTo("return this"));
        }

        private static IFoo CreateFoo()
        {
            return null;
        }

     



    }

    [TestFixture]
    public class CallToTests
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
    public class AArgumentValidationsTests
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