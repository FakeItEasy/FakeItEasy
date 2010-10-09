using NUnit.Framework;
using FakeItEasy.Expressions;
using FakeItEasy.Tests;
using System;
using FakeItEasy.Core;

namespace FakeItEasy.IntegrationTests
{
    [TestFixture]
    public class ApplicationDirectoryAssembliesTypeCatalogueIntegrationTests
    {
        [Test]
        public void Should_be_able_to_get_types_from_assembly_in_same_directory()
        {
            // Arrange
            var catalogue = new ApplicationDirectoryAssembliesTypeCatalogue();

            // Act

            // Assert
            Assert.That(catalogue.GetAvailableTypes(), Has.Some.EqualTo(typeof(A)));
        }
    }
}
