namespace FakeItEasy.Tests.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using FakeItEasy.Core;
    using NUnit.Framework;

    [TestFixture]
    public class TypeCatalogueInstanceProviderTests
    {
        private ITypeCatalogue typeCatalogue;
        private List<Type> typesInCatalogue;
        private TypeCatalogueInstanceProvider instanceProvider;

        private interface ISomeInterface
        {
        }

        [SetUp]
        public void Setup()
        {
            this.typeCatalogue = A.Fake<ITypeCatalogue>();

            this.typesInCatalogue = new List<Type>();

            A.CallTo(() => this.typeCatalogue.GetAvailableTypes()).Returns(this.typesInCatalogue);

            this.instanceProvider = new TypeCatalogueInstanceProvider(this.typeCatalogue);
        }

        [Test]
        public void Should_return_one_instance_of_each_type()
        {
            // Arrange
            this.typesInCatalogue.Add(typeof(SomeInterfaceImplementor));
            this.typesInCatalogue.Add(typeof(SomeInterfaceImplementor2));

            // Act
            var result = this.instanceProvider.InstantiateAllOfType<ISomeInterface>().ToArray();

            // Assert
            Assert.That(
                   result,
                   Has.Length.EqualTo(2).And.Some.InstanceOf<SomeInterfaceImplementor>().And.Some.InstanceOf<SomeInterfaceImplementor2>());
        }

        [Test]
        public void Should_ignore_types_not_assignable_to_T()
        {
            // Arrange
            this.typesInCatalogue.Add(typeof(string));

            // Act
            var result = this.instanceProvider.InstantiateAllOfType<ISomeInterface>().ToArray();

            // Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void Should_be_same_instance_when_enumerated_twice()
        {
            // Arrange
            this.typesInCatalogue.Add(typeof(SomeInterfaceImplementor));

            // Act
            var result = this.instanceProvider.InstantiateAllOfType<SomeInterfaceImplementor>();

            // Assert
            Assert.That(result.Single(), Is.SameAs(result.Single()));
        }

        [Test]
        public void Should_ignore_types_with_no_default_constructor()
        {
            // Arrange
            this.typesInCatalogue.Add(typeof(SomeInterfaceImplementor));
            this.typesInCatalogue.Add(typeof(SomeInterfaceImpelementorWithoutDefaultConstructor));

            // Act
            var result = this.instanceProvider.InstantiateAllOfType<ISomeInterface>();

            // Assert
            Assert.That(result, Has.Some.InstanceOf<SomeInterfaceImplementor>().And.None.InstanceOf<SomeInterfaceImpelementorWithoutDefaultConstructor>());
        }

        private class SomeInterfaceImplementor : ISomeInterface
        {
        }

        private class SomeInterfaceImplementor2 : ISomeInterface
        {
        }

        private class SomeInterfaceImpelementorWithoutDefaultConstructor : ISomeInterface
        {
            [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "argument", Justification = "Required for testing.")]
            public SomeInterfaceImpelementorWithoutDefaultConstructor(string argument)
            {
            }
        }
    }
}