namespace FakeItEasy.IntegrationTests
{
    using System.Linq;
    using Core;
    using NUnit.Framework;

    [TestFixture]
    public class ApplicationDirectoryAssembliesTypeCatalogueTests
    {
        private ApplicationDirectoryAssembliesTypeCatalogue catalogue;

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            this.catalogue = new ApplicationDirectoryAssembliesTypeCatalogue();
        }

        [Test]
        public void Should_be_able_to_get_types_from_fakeiteasy()
        {
            // Assert
            Assert.That(this.catalogue.GetAvailableTypes(), Has.Some.EqualTo(typeof(A)));
        }

        [Test]
        public void Should_be_able_to_get_types_from_assembly_in_app_domain()
        {
            // Assert
            Assert.That(this.catalogue.GetAvailableTypes(), Has.Some.EqualTo(typeof(DoubleValueFormatter)));
        }

        [Test]
        public void Should_be_able_to_get_types_from_external_assembly_in_directory()
        {
            // Assert
            Assert.That(this.catalogue.GetAvailableTypes().Select(type => type.FullName), Has.Some.EqualTo("FakeItEasy.IntegrationTests.External.GuidValueFormatter"));
        }

        [Test]
        public void Should_not_be_able_to_get_types_from_assembly_that_does_not_reference_fakeiteasy()
        {
            // Assert
            Assert.That(this.catalogue.GetAvailableTypes(), Has.None.EqualTo(typeof(string)));
        }
    }
}