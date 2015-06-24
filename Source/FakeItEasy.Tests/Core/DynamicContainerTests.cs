namespace FakeItEasy.Tests.Core
{
    using System;
    using System.Collections.Generic;
    using FakeItEasy.Core;
    using FluentAssertions;
    using NUnit.Framework;

    [TestFixture]
    public class DynamicContainerTests
    {
        private List<IDummyFactory> availableDummyFactories;
        private List<IFakeConfigurator> availableConfigurators;

        private IDisposable scope;

        [SetUp]
        public void Setup()
        {
            this.scope = Fake.CreateScope(new NullFakeObjectContainer());

            this.availableConfigurators = new List<IFakeConfigurator>();
            this.availableDummyFactories = new List<IDummyFactory>();
        }

        [TearDown]
        public void Teardown()
        {
            this.scope.Dispose();
        }

        [Test]
        public void TryCreateFakeObject_should_create_fake_for_type_that_has_factory()
        {
            this.availableDummyFactories.Add(new DummyFactoryForTypeWithFactory());

            var container = this.CreateContainer();

            object fake;

            container.TryCreateDummyObject(typeof(TypeWithDummyFactory), out fake).Should().BeTrue();
            fake.Should().BeOfType<TypeWithDummyFactory>();
        }

        [Test]
        public void TryCreateFakeObject_should_return_false_when_no_factory_exists()
        {
            var container = this.CreateContainer();

            object fake;

            container.TryCreateDummyObject(typeof(TypeWithDummyFactory), out fake).Should().BeFalse();
        }

        [Test]
        public void ConfigureFake_should_apply_configuration_for_registered_configuration()
        {
            this.availableConfigurators.Add(new ConfigurationForTypeWithDummyFactory());

            var container = this.CreateContainer();

            var fake = A.Fake<TypeWithDummyFactory>();

            container.ConfigureFake(typeof(TypeWithDummyFactory), fake);

            fake.WasConfigured.Should().BeTrue();
        }

        [Test]
        public void ConfigureFake_should_do_nothing_when_fake_type_has_no_configuration_specified()
        {
            this.CreateContainer();
            A.Fake<TypeWithDummyFactory>();
        }

        [Test]
        public void Should_not_fail_when_more_than_one_factory_exists_for_a_given_type()
        {
            // Arrange
            this.availableDummyFactories.Add(new DummyFactoryForTypeWithFactory());
            this.availableDummyFactories.Add(new DuplicateDummyFactoryForTypeWithFactory());

            var container = this.CreateContainer();

            // Act
            object fake;
            var result = container.TryCreateDummyObject(typeof(TypeWithDummyFactory), out fake);

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void Should_not_fail_when_more_than_one_configurator_exists_for_a_given_type()
        {
            // Arrange
            this.availableConfigurators.Add(new ConfigurationForTypeWithDummyFactory());
            this.availableConfigurators.Add(new DuplicateConfigurationForTypeWithDummyFactory());

            // Act

            // Assert
            Assert.DoesNotThrow(() =>
                this.CreateContainer());
        }

        private DynamicContainer CreateContainer()
        {
            return new DynamicContainer(this.availableDummyFactories, this.availableConfigurators);
        }

        public class ConfigurationForTypeWithDummyFactory : FakeConfigurator<TypeWithDummyFactory>
        {
            protected override void ConfigureFake(TypeWithDummyFactory fakeObject)
            {
                A.CallTo(() => fakeObject.WasConfigured).Returns(true);
            }
        }

        public class DuplicateConfigurationForTypeWithDummyFactory : FakeConfigurator<TypeWithDummyFactory>
        {
            protected override void ConfigureFake(TypeWithDummyFactory fakeObject)
            {
                A.CallTo(() => fakeObject.WasConfigured).Returns(true);
            }
        }

        public class DummyFactoryForTypeWithFactory : DummyFactory<TypeWithDummyFactory>
        {
            protected override TypeWithDummyFactory Create()
            {
                return new TypeWithDummyFactory();
            }
        }

        public class DuplicateDummyFactoryForTypeWithFactory : DummyFactory<TypeWithDummyFactory>
        {
            protected override TypeWithDummyFactory Create()
            {
                return new TypeWithDummyFactory();
            }
        }

        public class TypeWithDummyFactory
        {
            public virtual bool WasConfigured
            {
                get { return false; }
            }
        }
    }
}