using System;
using FakeItEasy.Core;
using FakeItEasy.Tests;
using NUnit.Framework;

namespace FakeItEasy.IntegrationTests
{
    [TestFixture]
    public class DummyTests
    {
        [Test]
        public void Type_registered_in_container_should_be_returned_when_a_dummy_is_requested()
        {
            var container = new DelegateFakeObjectContainer();
            container.Register<string>(() => "dummy");

            using (Fake.CreateScope(container))
            {
                Assert.That(A.Dummy<string>(), Is.EqualTo("dummy"));
            }
        }

        [Test]
        public void Proxy_should_be_returned_when_nothing_is_registered_in_the_container_for_the_type()
        {
            using (Fake.CreateScope(new NullFakeObjectContainer()))
            {
                Assert.That(A.Dummy<IFoo>(), Is.InstanceOf<IFakedProxy>());
            }
        }

        [Test]
        public void Correct_exception_should_be_thrown_when_dummy_is_requested_for_non_fakeable_type_not_in_container()
        {
            using (Fake.CreateScope(new NullFakeObjectContainer()))
            {
                Assert.Throws<FakeCreationException>(() =>
                    A.Dummy<NonInstance>());
            }
        }

        private class NonInstance
        {
            private NonInstance()
            {

            }
        }
    }
}
