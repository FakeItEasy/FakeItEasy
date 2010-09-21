using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using FakeItEasy.Tests;
using FakeItEasy.ExtensionSyntax;
using FakeItEasy.VisualBasic;
using System.IO;
using System.Web.UI;
using FakeItEasy.Core;
using FakeItEasy.Configuration;

namespace FakeItEasy.IntegrationTests
{
    [TestFixture]
    public class GeneralTests
    {
        [Test]
        public void Faked_object_with_fakeable_properties_should_have_fake_as_default_value()
        {
            var fake = A.Fake<ITypeWithFakeableProperties>();

            Assert.That(fake.Collection, Is.Not.Null);
            Assert.That(fake.Foo, Is.Not.Null);
        }

        [Test]
        public void Faked_object_with_method_that_returns_fakeable_type_should_return_fake_by_default()
        {
            // Arrange
            var fake = A.Fake<IEnumerable<string>>();

            // Act
            var enumerator = fake.GetEnumerator();

            // Assert
            Assert.That(enumerator, Is.Not.Null);
            Assert.That(Fake.GetFakeManager(enumerator), Is.Not.Null);
        }

        [Test]
        public void Should_not_be_able_to_fake_Uri_when_no_container_is_used()
        {
            using (Fake.CreateScope(new NullFakeObjectContainer()))
            {
                Assert.Throws<FakeCreationException>(() =>
                    A.Fake<Guid>());
            }
        }

        [Test]
        public void ErrorMessage_when_type_can_not_be_faked_should_specify_non_resolvable_constructor_arguments()
        {
            using (Fake.CreateScope())
            {
                var thrown = Assert.Throws<FakeCreationException>(() =>
                    A.Fake<NonResolvableType>());
                
                Assert.That(thrown.Message, Is.EqualTo(@"
  Failed to create fake of type ""FakeItEasy.IntegrationTests.GeneralTests+NonResolvableType"".

  Below is a list of reasons for failure per attempted constructor:
    No constructor arguments failed:
      No default constructor was found on the type FakeItEasy.IntegrationTests.GeneralTests+NonResolvableType.
    The following constructors were not tried:
      (FakeItEasy.Tests.IFoo, *FakeItEasy.IntegrationTests.GeneralTests+NoInstanceType)
      (*FakeItEasy.IntegrationTests.GeneralTests+NoInstanceType)

      Types marked with * could not be resolved, register them in the current
      IFakeObjectContainer to enable these constructors.

"));
            }   
        }

        public class NonResolvableType
        {
            public NonResolvableType(IFoo foo, NoInstanceType bar)
            {

            }

            protected NonResolvableType(NoInstanceType bar)
            {

            }
        }

        public class NoInstanceType
        {
            private NoInstanceType()
            {

            }
        }

        [Test]
        public void ErrorMessage_when_configuring_void_call_that_can_not_be_configured_should_be_correct()
        {
            // Arrange
            var fake = A.Fake<TypeWithNonConfigurableMethod>();

            // Act

            // Assert
            var thrown = Assert.Throws<FakeConfigurationException>(() =>
                A.CallTo(() => fake.NonVirtualVoidMethod("", 1)).DoesNothing());

            Assert.That(thrown.Message, Is.EqualTo("The specified method can not be configured since it can not be intercepted by the current IProxyGenerator."));
        }

        [Test]
        public void ErrorMessage_when_configuring_function_call_that_can_not_be_configured_should_be_correct()
        {
            // Arrange
            var fake = A.Fake<TypeWithNonConfigurableMethod>();

            // Act

            // Assert
            var thrown = Assert.Throws<FakeConfigurationException>(() =>
                A.CallTo(() => fake.NonVirtualFunction("", 1)).Returns(10));

            Assert.That(thrown.Message, Is.EqualTo("The specified method can not be configured since it can not be intercepted by the current IProxyGenerator."));
        }

        [Test]
        public void ErrorMessage_when_configuring_void_call_that_can_not_be_configured_through_old_api_should_be_correct()
        {
            // Arrange
            var fake = A.Fake<TypeWithNonConfigurableMethod>();

            // Act

            // Assert
            var thrown = Assert.Throws<FakeConfigurationException>(() =>
                A.CallTo(() => fake.NonVirtualVoidMethod("", 1)).DoesNothing());

            Assert.That(thrown.Message, Is.EqualTo("The specified method can not be configured since it can not be intercepted by the current IProxyGenerator."));
        }

        [Test]
        public void ErrorMessage_when_configuring_function_call_that_can_not_be_configured_through_old_api_should_be_correct()
        {
            // Arrange
            var fake = A.Fake<TypeWithNonConfigurableMethod>();

            // Act

            // Assert
            var thrown = Assert.Throws<FakeConfigurationException>(() =>
                A.CallTo(() => fake.NonVirtualFunction("", 1)).Returns(10));

            Assert.That(thrown.Message, Is.EqualTo("The specified method can not be configured since it can not be intercepted by the current IProxyGenerator."));
        }

        [Test]
        public void Should_be_able_to_generate_class_fake_that_implements_additional_interface()
        {
            // Arrange
            var fake = A.Fake<FakeableClass>(x => x.Implements(typeof(IFoo)).Implements(typeof(IFormattable)));

            // Act

            // Assert
            Assert.That(fake, Is.InstanceOf<IFoo>());
            Assert.That(fake, Is.InstanceOf<IFormattable>());
            Assert.That(fake, Is.InstanceOf<FakeableClass>());
        }

        [Test]
        public void Should_be_able_to_generate_interface_fake_that_implements_additional_interface()
        {
            // Arrange
            var fake = A.Fake<IFoo>(x => x.Implements(typeof(IFormatProvider)).Implements(typeof(IFormattable)));

            // Act

            // Assert
            Assert.That(fake, Is.InstanceOf<IFoo>());
            Assert.That(fake, Is.InstanceOf<IFormattable>());
            Assert.That(fake, Is.InstanceOf<IFormatProvider>());
        }

        [Test]
        public void FakeCollection_should_return_list_where_all_objects_are_fakes()
        {
            // Arrange
            
            // Act
            var result = A.CollectionOfFake<IFoo>(10);

            // Assert
            Assert.That(result, Is.InstanceOf<IList<IFoo>>().And.All.InstanceOf<IFoo>().And.All.Matches(new IsProxyConstraint()));
        }

        [Test]
        public void Returns_from_sequence_only_applies_the_number_as_many_times_as_the_number_of_specified_values()
        {
            // Arrange
            var foo = A.Fake<IFoo>();

            // Act
            A.CallTo(() => foo.Baz()).Throws(new Exception());
            A.CallTo(() => foo.Baz()).ReturnsNextFromSequence(1, 2);

            foo.Baz();
            foo.Baz();

            // Assert
            Assert.Throws<Exception>(() =>
                foo.Baz());
        }

        public class FakeableClass
        {
            public virtual void Foo()
            { }
        }

        public class TypeWithNonConfigurableMethod
        {
            public void NonVirtualVoidMethod(string argument, int otherArgument)
            {
                
            }

            public int NonVirtualFunction(string argument, int otherArgument)
            {
                return 1;
            }
        }
        
       
        public interface ITypeWithFakeableProperties
        {
            IEnumerable<object> Collection { get; }
            IFoo Foo { get; }
        }
    }
}
