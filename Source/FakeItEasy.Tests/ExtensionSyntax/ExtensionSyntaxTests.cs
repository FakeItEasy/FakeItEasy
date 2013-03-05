﻿namespace FakeItEasy.Tests.ExtensionSyntax
{
    using System;
    using FakeItEasy.Configuration;
    using FakeItEasy.ExtensionSyntax;
    using NUnit.Framework;

    [TestFixture]
    public class ExtensionSyntaxTests
        : ConfigurableServiceLocatorTestBase
    {
        [Test]
        public void Configure_should_return_fake_configuration_when_called_on_fake_object()
        {
            var foo = A.Fake<IFoo>();

            var configuration = A.Fake<IStartConfiguration<IFoo>>();
            var configurationFactory = A.Fake<IStartConfigurationFactory>();
            A.CallTo(() => configurationFactory.CreateConfiguration<IFoo>(Fake.GetFakeManager(foo)))
                .Returns(configuration);

            using (Fake.CreateScope())
            {
                this.StubResolve<IStartConfigurationFactory>(configurationFactory);

                var result = foo.Configure();

                Assert.That(result, Is.SameAs(configuration));
            }
        }

        [Test]
        public void Configure_should_throw_when_FakedObject_is_not_a_faked_object()
        {
            Assert.Throws<ArgumentException>(() =>
                string.Empty.Configure());
        }

        [Test]
        public void Configure_should_throw_when_fakedObject_is_null()
        {
            Assert.Throws<ArgumentNullException>(() =>
                FakeItEasy.ExtensionSyntax.Syntax.Configure((IFoo)null));
        }

        protected override void OnSetUp()
        {
        }
    }
}
