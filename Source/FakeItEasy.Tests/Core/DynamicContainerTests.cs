using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace FakeItEasy.Core.Tests
{
    [TestFixture]
    public class DynamicContainerTests
    {
        private ITypeCatalogue typeCatalogue;
        private List<Type> availableTypes;
        private IDisposable scope;

        [SetUp]
        public void SetUp()
        {
            this.scope = Fake.CreateScope(new NullFakeObjectContainer());

            this.availableTypes = new List<Type>();

            this.typeCatalogue = A.Fake<ITypeCatalogue>();
            A.CallTo(() => this.typeCatalogue.GetAvailableTypes()).Returns(this.availableTypes);
        }

        [TearDown]
        public void TearDown()
        {
            this.scope.Dispose();
        }

        private DynamicContainer CreateContainer()
        {
            return new DynamicContainer(this.typeCatalogue);
        }

        [Test]
        public void TryCreateFakeObject_should_create_fake_for_type_that_has_definition()
        {
            this.availableTypes.Add(typeof(DummyDefinitionForTypeWithDefinition));

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
            var result = container.TryCreateDummyObject(typeof(TypeWithDummyDefinition), out outputVariable);

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
            var result = container.TryCreateDummyObject(typeof(TypeWithDummyDefinition), out fake);

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

        public class ConfigurationWithNoConstructor : FakeConfigurer<TypeWithDummyDefinition>
        {
            public ConfigurationWithNoConstructor(string argumentToConstructor)
            {
            }

            public override void ConfigureFake(TypeWithDummyDefinition fakeObject)
            {
                Console.WriteLine("Applying form no default constructor");
                A.CallTo(() => fakeObject.WasConfigured).Returns(true);
            }
        }
        
        public class DefinitionWithNoConstructor : DummyDefinition<TypeWithDummyDefinition>
        {
            public DefinitionWithNoConstructor(string argumentToConstructor)
            {
            }

            protected override TypeWithDummyDefinition CreateDummy()
            {
                return new TypeWithDummyDefinition();
            }
        }


        public class ConfigurationForTypeWithDummyDefintion : FakeConfigurer<TypeWithDummyDefinition>
        {
            public override void ConfigureFake(TypeWithDummyDefinition fakeObject)
            {
                A.CallTo(() => fakeObject.WasConfigured).Returns(true);
            }
        }

        public class DuplicateConfigurationForTypeWithDummyDefintion : FakeConfigurer<TypeWithDummyDefinition>
        {
            public override void ConfigureFake(TypeWithDummyDefinition fakeObject)
            {
                A.CallTo(() => fakeObject.WasConfigured).Returns(true);
            }
        }

        public class DummyDefinitionForTypeWithDefinition
            : DummyDefinition<TypeWithDummyDefinition>
        {
            protected override TypeWithDummyDefinition CreateDummy()
            {
                return new TypeWithDummyDefinition();
            }
        }

        public class DuplicateDummyDefinitionForTypeWithDefinition
            : DummyDefinition<TypeWithDummyDefinition>
        {
            protected override TypeWithDummyDefinition CreateDummy()
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