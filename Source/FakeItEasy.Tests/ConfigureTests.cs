using System;
using FakeItEasy.Core;
using FakeItEasy.Configuration;
using NUnit.Framework;

namespace FakeItEasy.Tests
{
    [TestFixture]
    public class ConfigureTests
        : ConfigurableServiceLocatorTestBase
    {
        protected override void OnSetUp()
        {

        }

        [Test]
        public void Fake_should_return_start_configuration_from_factory()
        {
            var factory = A.Fake<IStartConfigurationFactory>();
            var config = A.Fake<IStartConfiguration<IFoo>>();

            Configure.Fake(factory)
                .CallsTo(x => x.CreateConfiguration<IFoo>(A<FakeObject>.Ignored))
                .Returns(config);

            var foo = A.Fake<IFoo>();

            using (Fake.CreateScope())
            {
                this.StubResolve<IStartConfigurationFactory>(factory);

                var result = Configure.Fake(foo);

                Assert.That(result, Is.SameAs(config));
            }
        }

        [Test]
        public void Fake_should_be_null_guarded()
        {
            NullGuardedConstraint.Assert(() =>
                Configure.Fake(A.Fake<IFoo>()));
        }

        [Test]
        [SetCulture("en-US")]
        public void Fake_should_throw_when_argument_is_not_a_faked_object()
        {
            var thrown = Assert.Throws<ArgumentException>(() =>
                Configure.Fake(""));

            Assert.That(thrown.Message, Text.StartsWith("Error when accessing FakeObject, the specified argument is of the type 'System.String' which is not faked."));
        }

        [Test]
        public void Configure_should_throw_when_object_is_not_a_fake_object()
        {
            Assert.Throws<ArgumentException>(() =>
                Configure.Fake("non fake object"));
        }

        [Test]
        public void Configure_should_throw_when_FakedObject_is_not_a_faked_object()
        {
            Assert.Throws<ArgumentException>(() =>
                Configure.Fake(""));
        }

        [Test]
        public void Configure_should_throw_when_fakedObject_is_null()
        {
            Assert.Throws<ArgumentNullException>(() =>
                Configure.Fake((IFoo)null));
        }
    }
}
