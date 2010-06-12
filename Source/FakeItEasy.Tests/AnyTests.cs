using System;
using FakeItEasy.Core;
using FakeItEasy.Configuration;
using NUnit.Framework;

namespace FakeItEasy.Tests
{
	[TestFixture]
    public class AnyTests
        : ConfigurableServiceLocatorTestBase
    {
        protected override void OnSetUp()
        {

        }

        [Test]
        public void CallTo_should_return_configuration_from_start_configuration()
        {
            // Arrange
            var foo = A.Fake<IFoo>();

            var anyCallConfiguration = A.Fake<IAnyCallConfiguration>();
            var startConfiguration = A.Fake<IStartConfiguration<IFoo>>();
            var configurationFactory = A.Fake<IStartConfigurationFactory>();
            A.CallTo(() => configurationFactory.CreateConfiguration<IFoo>(A<FakeManager>.That.Fakes(foo))).Returns(startConfiguration);
            A.CallTo(() => startConfiguration.AnyCall()).Returns(anyCallConfiguration);
            
            using (Fake.CreateScope())
            {
                this.StubResolve<IStartConfigurationFactory>(configurationFactory);

                // Act
                    
                // Assert
                Assert.That(Any.CallTo(foo), Is.SameAs(anyCallConfiguration));
            }
        }

        [Test]
        public void Static_equals_delegates_to_static_method_on_object()
        {
            Assert.That(Any.Equals("foo", "foo"), Is.True);
        }

        [Test]
        public void Static_ReferenceEquals_delegates_to_static_method_on_object()
        {
            var s = "";

            Assert.That(Any.ReferenceEquals(s, s), Is.True);
        }
    }
}
