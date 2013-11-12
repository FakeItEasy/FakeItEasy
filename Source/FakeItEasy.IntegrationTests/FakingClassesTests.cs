namespace FakeItEasy.IntegrationTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization.Formatters.Binary;
    using Core;
    using FluentAssertions;
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
        private ApplicationDirectoryAssembliesTypeCatalogue catalogue;

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            this.catalogue = new ApplicationDirectoryAssembliesTypeCatalogue();
        }

        [Test]
        public void Should_warn_of_duplicate_input_assemblies_with_different_paths()
        {
            string actualMessage;

            // Arrange
            var originalDirectory = Environment.CurrentDirectory;
            var originalStandardOut = Console.Out;

            try
            {
                // FakeItEasy.IntegrationTests.External has copies of many of the assemblies used in these
                // tests as well. By changing the working directory before creating the
                // ApplicationDirectoryAssembliesTypeCatalogue, the scanning will get those assemblies
                // from the current AppDomain as well as the other path.
                Environment.CurrentDirectory = System.IO.Path.Combine(Environment.CurrentDirectory, @"..\..\..\FakeItEasy.IntegrationTests.External\bin\Debug");
                using (var messageStream = new MemoryStream())
                using (var messageWriter = new StreamWriter(messageStream))
                {
                    Console.SetOut(messageWriter);

                    // Act
                    new ApplicationDirectoryAssembliesTypeCatalogue();
                    messageWriter.Flush();
                    actualMessage = messageWriter.Encoding.GetString(messageStream.GetBuffer());
                }
            }
            finally
            {
                Console.SetOut(originalStandardOut);
                Environment.CurrentDirectory = originalDirectory;
            }

            // Assert
            const string ExpectedMessagePattern = @"*Warning: FakeItEasy failed to load assembly '*FakeItEasy.IntegrationTests.External\bin\Debug\FakeItEasy.IntegrationTests.External.dll' while scanning for extension points. Any IArgumentValueFormatters, IDummyDefinitions, and IFakeConfigurators in that assembly will not be available.
  API restriction: The assembly '*FakeItEasy.IntegrationTests.External\bin\Debug\FakeItEasy.IntegrationTests.External.dll' has already loaded from a different location. It cannot be loaded from a new location within the same appdomain.*";
            actualMessage.Should().Match(ExpectedMessagePattern);
        }

        [Test]
        public void Should_be_able_to_get_types_from_fakeiteasy()
        {
            // Arrange

            // Act

            // Assert
            this.catalogue.GetAvailableTypes().Should().Contain(typeof(A));
        }

        [Test]
        public void Should_be_able_to_get_types_from_assembly_in_app_domain()
        {
            // Arrange

            // Act

            // Assert
            this.catalogue.GetAvailableTypes().Should().Contain(typeof(DoubleValueFormatter));
        }

        [Test]
        public void Should_be_able_to_get_types_from_external_assembly_in_directory()
        {
            // Arrange

            // Act

            // Assert
            this.catalogue.GetAvailableTypes().Select(type => type.FullName).Should().Contain("FakeItEasy.IntegrationTests.External.GuidValueFormatter");
        }

        [Test]
        public void Should_not_be_able_to_get_types_from_assembly_that_does_not_reference_fakeiteasy()
        {
            // Arrange

            // Act

            // Assert
            this.catalogue.GetAvailableTypes().Should().NotContain(typeof(string));
        }
    }
}