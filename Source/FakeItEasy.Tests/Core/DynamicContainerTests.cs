using System;
using System.Collections.Generic;
using NUnit.Framework;
using FakeItEasy.Tests;

namespace FakeItEasy.Core.Tests
{
    [TestFixture]
    public class DynamicContainerTests
    {
        private ITypeAccessor typeAccessor;
        private List<Type> availableTypes;
        private DynamicContainer container;

        [SetUp]
        public void SetUp()
        {
            this.availableTypes = new List<Type>();

            this.typeAccessor = A.Fake<ITypeAccessor>();
            A.CallTo(() => this.typeAccessor.GetAvailableTypes()).Returns(this.availableTypes);
        }

        private DynamicContainer CreateContainer()
        {
            return new DynamicContainer(this.typeAccessor);
        }

        [Test]
        public void TryCreateFakeObject_should_create_fake_for_type_that_has_definition()
        {
            this.availableTypes.Add(typeof(DummyDefinitionForTypeWithDefinition));

            var container = this.CreateContainer();

            object fake;

            Assert.That(container.TryCreateFakeObject(typeof(TypeWithDummyDefinition), out fake), Is.True);
            Assert.That(fake, Is.InstanceOf<TypeWithDummyDefinition>());
        }

        [Test]
        public void TryCreateFakeObject_should_return_false_when_no_definition_exists()
        {
            var container = this.CreateContainer();

            object fake;

            Assert.That(container.TryCreateFakeObject(typeof(TypeWithDummyDefinition), out fake), Is.False);
        }

        [Test]
        public void ConfigureFake_should_apply_configuration_for_registered_configuration()
        {
            this.availableTypes.Add(typeof(ConfigurationForTypeWithDummyDefintion));

            var container = this.CreateContainer();

            var fake = A.Fake<TypeWithDummyDefinition>();

            container.ConfigureFake(typeof(TypeWithDummyDefinition), fake);

            Assert.That(fake.WasConfigured, Is.True);
        }

        [Test]
        public void ConfigureFake_should_do_nothing_when_fake_type_has_no_configuration_specified()
        {
            var container = this.CreateContainer();

            var fake = A.Fake<TypeWithDummyDefinition>();
        }

        [Test]
        public void TryCreateFakeObject_should_return_false_when_existing_definition_has_no_default_constructor()
        {
            // Arrange
            this.availableTypes.Add(typeof(DefinitionWithNoConstructor));

            var container = this.CreateContainer();

            // Act
            object outputVariable;
            var result = container.TryCreateFakeObject(typeof(TypeWithDummyDefinition), out outputVariable);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void ConfigureFake_should_apply_no_configuration_when_existing_configuration_has_no_default_constructor()
        {
            // Arrange
            this.availableTypes.Add(typeof(ConfigurationWithNoConstructor));
            var fake = A.Fake<TypeWithDummyDefinition>();

            var container = this.CreateContainer();
            
            // Act
            container.ConfigureFake(typeof(TypeWithDummyDefinition), fake);

            // Assert
            Assert.That(fake.WasConfigured, Is.False);
        }

        [Test]
        public void Should_not_fail_when_more_than_one_defintion_exists_for_a_given_type()
        {
            // Arrange
            this.availableTypes.Add(typeof(DummyDefinitionForTypeWithDefinition));
            this.availableTypes.Add(typeof(DuplicateDummyDefinitionForTypeWithDefinition));

            var container = this.CreateContainer();

            // Act
            object fake = null;
            var result = container.TryCreateFakeObject(typeof(TypeWithDummyDefinition), out fake);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void Should_not_fail_when_more_than_one_configurator_exists_for_a_given_type()
        {
            // Arrange
            this.availableTypes.Add(typeof(ConfigurationForTypeWithDummyDefintion));
            this.availableTypes.Add(typeof(DuplicateConfigurationForTypeWithDummyDefintion));

            // Act

            // Assert
            Assert.DoesNotThrow(() =>
                this.CreateContainer());
        }

        public class ConfigurationWithNoConstructor : FakeConfigurator<TypeWithDummyDefinition>
        {
            public ConfigurationWithNoConstructor(string argumentToConstructor)
            {
            }

            public override void ConfigureFake(TypeWithDummyDefinition fakeObject)
            {
                A.CallTo(() => fakeObject.WasConfigured).Returns(true);
            }
        }
        
        public class DefinitionWithNoConstructor : DummyDefinition<TypeWithDummyDefinition>
        {
            public DefinitionWithNoConstructor(string argumentToConstructor)
            {
            }

            protected override TypeWithDummyDefinition CreateFake()
            {
                return new TypeWithDummyDefinition();
            }
        }


        public class ConfigurationForTypeWithDummyDefintion : FakeConfigurator<TypeWithDummyDefinition>
        {
            public override void ConfigureFake(TypeWithDummyDefinition fakeObject)
            {
                A.CallTo(() => fakeObject.WasConfigured).Returns(true);
            }
        }

        public class DuplicateConfigurationForTypeWithDummyDefintion : FakeConfigurator<TypeWithDummyDefinition>
        {
            public override void ConfigureFake(TypeWithDummyDefinition fakeObject)
            {
                A.CallTo(() => fakeObject.WasConfigured).Returns(true);
            }
        }

        public class DummyDefinitionForTypeWithDefinition
            : DummyDefinition<TypeWithDummyDefinition>
        {
            protected override TypeWithDummyDefinition CreateFake()
            {
                return new TypeWithDummyDefinition();
            }
        }

        public class DuplicateDummyDefinitionForTypeWithDefinition
            : DummyDefinition<TypeWithDummyDefinition>
        {
            protected override TypeWithDummyDefinition CreateFake()
            {
                return new TypeWithDummyDefinition();
            }
        }

        public class TypeWithDummyDefinition
        {
            public void Bar()
            {
                throw new NotImplementedException();
            }

            public virtual bool WasConfigured { get; set; }
        }
    }
}