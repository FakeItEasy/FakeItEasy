namespace FakeItEasy.IntegrationTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using Core;
    using NUnit.Framework;

    [TestFixture]
    public class FakingClassesTests
    {
        [Test]
        public void Should_be_able_to_get_a_fake_value_of_uri_type()
        {
            using (Fake.CreateScope(new NullFakeObjectContainer()))
            {
                A.Fake<Uri>();
            }
        }

        [Test]
        public void Should_be_able_to_serialize_a_fake()
        {
            // arrange
            var person = A.Fake<Person>();

            var formatter = new BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                // act
                formatter.Serialize(stream, person);
            }
        }

        [Serializable]
        public class Person
        {
        }
    }

    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass",
        Justification = "Reviewed. Suppression is OK here."), TestFixture]
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