namespace FakeItEasy.Core.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using NUnit.Framework;

    [TestFixture]
    public class DynamicContainerTests
    {
        private List<IDummyFactory> availableDummyFactories;
        private List<IFakeConfigurator> availableConfigurers;

        private IDisposable scope;

        [SetUp]
        public void Setup()
        {
            this.scope = Fake.CreateScope(new NullFakeObjectContainer());

            this.availableConfigurers = new List<IFakeConfigurator>();
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

            Assert.That(container.TryCreateDummyObject(typeof(TypeWithDummyFactory), out fake), Is.True);
            Assert.That(fake, Is.InstanceOf<TypeWithDummyFactory>());
        }

        [Test]
        public void TryCreateFakeObject_should_return_false_when_no_factory_exists()
        {
            var container = this.CreateContainer();

            object fake;

            Assert.That(container.TryCreateDummyObject(typeof(TypeWithDummyFactory), out fake), Is.False);
        }

        [Test]
        public void ConfigureFake_should_apply_configuration_for_registered_configuration()
        {
            this.availableConfigurers.Add(new ConfigurationForTypeWithDummyFactory());

            var container = this.CreateContainer();

            var fake = A.Fake<TypeWithDummyFactory>();

            container.ConfigureFake(typeof(TypeWithDummyFactory), fake);

            Assert.That(fake.WasConfigured, Is.True);
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
            object fake = null;
            var result = container.TryCreateDummyObject(typeof(TypeWithDummyFactory), out fake);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void Should_not_fail_when_more_than_one_configurator_exists_for_a_given_type()
        {
            // Arrange
            this.availableConfigurers.Add(new ConfigurationForTypeWithDummyFactory());
            this.availableConfigurers.Add(new DuplicateConfigurationForTypeWithDummyFactory());

            // Act

            // Assert
            Assert.DoesNotThrow(() =>
                this.CreateContainer());
        }

        private DynamicContainer CreateContainer()
        {
            return new DynamicContainer(this.availableDummyFactories, this.availableConfigurers);
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
            public virtual bool WasConfigured { get; set; }

            [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for testing.")]
            public void Bar()
            {
                throw new NotImplementedException();
            }
        }
    }
}