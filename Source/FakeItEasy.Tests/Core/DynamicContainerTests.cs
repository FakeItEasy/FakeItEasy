namespace FakeItEasy.Core.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using NUnit.Framework;

    [TestFixture]
    public class DynamicContainerTests
    {
        private List<IDummyDefinition> availableDummyDefinitions;
        private List<IFakeConfigurator> availableConfigurers;

        private IDisposable scope;

        [SetUp]
        public void Setup()
        {
            this.scope = Fake.CreateScope(new NullFakeObjectContainer());

            this.availableConfigurers = new List<IFakeConfigurator>();
            this.availableDummyDefinitions = new List<IDummyDefinition>();
        }

        [TearDown]
        public void Teardown()
        {
            this.scope.Dispose();
        }

        [Test]
        public void TryCreateFakeObject_should_create_fake_for_type_that_has_definition()
        {
            this.availableDummyDefinitions.Add(new DummyDefinitionForTypeWithDefinition());

            var container = this.CreateContainer();

            object fake;

            Assert.That(container.TryCreateDummyObject(typeof(TypeWithDummyDefinition), out fake), Is.True);
            Assert.That(fake, Is.InstanceOf<TypeWithDummyDefinition>());
        }

        [Test]
        public void TryCreateFakeObject_should_return_false_when_no_definition_exists()
        {
            var container = this.CreateContainer();

            object fake;

            Assert.That(container.TryCreateDummyObject(typeof(TypeWithDummyDefinition), out fake), Is.False);
        }

        [Test]
        public void ConfigureFake_should_apply_configuration_for_registered_configuration()
        {
            this.availableConfigurers.Add(new ConfigurationForTypeWithDummyDefinition());

            var container = this.CreateContainer();

            var fake = A.Fake<TypeWithDummyDefinition>();

            container.ConfigureFake(typeof(TypeWithDummyDefinition), fake);

            Assert.That(fake.WasConfigured, Is.True);
        }

        [Test]
        public void ConfigureFake_should_do_nothing_when_fake_type_has_no_configuration_specified()
        {
            this.CreateContainer();
            A.Fake<TypeWithDummyDefinition>();
        }

        [Test]
        public void Should_not_fail_when_more_than_one_definition_exists_for_a_given_type()
        {
            // Arrange
            this.availableDummyDefinitions.Add(new DummyDefinitionForTypeWithDefinition());
            this.availableDummyDefinitions.Add(new DuplicateDummyDefinitionForTypeWithDefinition());

            var container = this.CreateContainer();

            // Act
            object fake = null;
            var result = container.TryCreateDummyObject(typeof(TypeWithDummyDefinition), out fake);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void Should_not_fail_when_more_than_one_configurator_exists_for_a_given_type()
        {
            // Arrange
            this.availableConfigurers.Add(new ConfigurationForTypeWithDummyDefinition());
            this.availableConfigurers.Add(new DuplicateConfigurationForTypeWithDummyDefinition());

            // Act

            // Assert
            Assert.DoesNotThrow(() =>
                this.CreateContainer());
        }

        private DynamicContainer CreateContainer()
        {
            return new DynamicContainer(this.availableDummyDefinitions, this.availableConfigurers);
        }

        public class ConfigurationForTypeWithDummyDefinition : FakeConfigurator<TypeWithDummyDefinition>
        {
            public override void ConfigureFake(TypeWithDummyDefinition fakeObject)
            {
                A.CallTo(() => fakeObject.WasConfigured).Returns(true);
            }
        }

        public class DuplicateConfigurationForTypeWithDummyDefinition : FakeConfigurator<TypeWithDummyDefinition>
        {
            public override void ConfigureFake(TypeWithDummyDefinition fakeObject)
            {
                A.CallTo(() => fakeObject.WasConfigured).Returns(true);
            }
        }

        public class DummyDefinitionForTypeWithDefinition : DummyDefinition<TypeWithDummyDefinition>
        {
            protected override TypeWithDummyDefinition CreateDummy()
            {
                return new TypeWithDummyDefinition();
            }
        }

        public class DuplicateDummyDefinitionForTypeWithDefinition : DummyDefinition<TypeWithDummyDefinition>
        {
            protected override TypeWithDummyDefinition CreateDummy()
            {
                return new TypeWithDummyDefinition();
            }
        }

        public class TypeWithDummyDefinition
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