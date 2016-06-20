namespace FakeItEasy.Tests.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using FakeItEasy.Core;
    using FluentAssertions;
    using Xunit;

    public class TypeCatalogueInstanceProviderTests
    {
        private readonly ITypeCatalogue typeCatalogue;
        private readonly List<Type> typesInCatalogue;
        private readonly TypeCatalogueInstanceProvider instanceProvider;

        public TypeCatalogueInstanceProviderTests()
        {
            this.typeCatalogue = A.Fake<ITypeCatalogue>();

            this.typesInCatalogue = new List<Type>();

            A.CallTo(() => this.typeCatalogue.GetAvailableTypes()).Returns(this.typesInCatalogue);

            this.instanceProvider = new TypeCatalogueInstanceProvider(this.typeCatalogue);
        }

        private interface ISomeInterface
        {
        }

        [Fact]
        public void Should_return_one_instance_of_each_type()
        {
            // Arrange
            this.typesInCatalogue.Add(typeof(SomeInterfaceImplementor));
            this.typesInCatalogue.Add(typeof(SomeInterfaceImplementor2));

            // Act
            var result = this.instanceProvider.InstantiateAllOfType<ISomeInterface>().ToArray();

            // Assert
            result.Should().HaveCount(2).And
                .Contain(item => item.GetType().CanBeInstantiatedAs(typeof(SomeInterfaceImplementor))).And
                .Contain(item => item.GetType().CanBeInstantiatedAs(typeof(SomeInterfaceImplementor2)));
        }

        [Fact]
        public void Should_ignore_types_not_assignable_to_T()
        {
            // Arrange
            this.typesInCatalogue.Add(typeof(string));

            // Act
            var result = this.instanceProvider.InstantiateAllOfType<ISomeInterface>().ToArray();

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void Should_be_same_instance_when_enumerated_twice()
        {
            // Arrange
            this.typesInCatalogue.Add(typeof(SomeInterfaceImplementor));

            // Act
            var result = this.instanceProvider.InstantiateAllOfType<SomeInterfaceImplementor>();

            // Assert
            result.Single().Should().BeSameAs(result.Single());
        }

        [Fact]
        public void Should_ignore_types_with_no_default_constructor()
        {
            // Arrange
            this.typesInCatalogue.Add(typeof(SomeInterfaceImplementor));
            this.typesInCatalogue.Add(typeof(SomeInterfaceImplementorWithoutDefaultConstructor));

            // Act
            var result = this.instanceProvider.InstantiateAllOfType<ISomeInterface>();

            // Assert
            result.Should()
                .Contain(item => item.GetType().CanBeInstantiatedAs(typeof(SomeInterfaceImplementor))).And
                .NotContain(item => item.GetType().CanBeInstantiatedAs(typeof(SomeInterfaceImplementorWithoutDefaultConstructor)));
        }

        private class SomeInterfaceImplementor : ISomeInterface
        {
        }

        private class SomeInterfaceImplementor2 : ISomeInterface
        {
        }

        private class SomeInterfaceImplementorWithoutDefaultConstructor : ISomeInterface
        {
            [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "argument", Justification = "Required for testing.")]
            public SomeInterfaceImplementorWithoutDefaultConstructor(string argument)
            {
            }
        }
    }
}
