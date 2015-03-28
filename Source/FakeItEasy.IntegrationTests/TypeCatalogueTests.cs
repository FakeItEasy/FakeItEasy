namespace FakeItEasy.IntegrationTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using FakeItEasy.Core;
    using FluentAssertions;
    using NUnit.Framework;

    public class TypeCatalogueTests
    {
        [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "No exception is thrown.")]
        [Test]
        public void Should_warn_of_duplicate_input_assemblies_with_different_paths()
        {
            // Arrange
            var currentDirectoryName = new DirectoryInfo(Environment.CurrentDirectory).Name;

            // FakeItEasy.IntegrationTests.External has copies of many of the assemblies used in these
            // tests as well. By specifying assembly paths from that directory, the catalog will see
            // those assemblies in both locations, and should fail to load the duplicates.
            var directoryToScan = Path.Combine(
                Environment.CurrentDirectory,
                Path.Combine(@"..\..\..\FakeItEasy.IntegrationTests.External\bin", currentDirectoryName));

            var expectedMessageFormat =
@"*Warning: FakeItEasy failed to load assembly '*FakeItEasy.IntegrationTests.External\bin\{0}\FakeItEasy.IntegrationTests.External.dll' while scanning for extension points. Any IArgumentValueFormatters, IDummyFactories, and IFakeConfigurators in that assembly will not be available.*";

            var expectedMessage = string.Format(CultureInfo.InvariantCulture, expectedMessageFormat, currentDirectoryName);
            string actualMessage;

            using (var messageStream = new MemoryStream())
            using (var messageWriter = new StreamWriter(messageStream))
            {
                var catalogue = new TypeCatalogue();

                var originalWriter = Console.Out;
                Console.SetOut(messageWriter);
                try
                {
                    // Act
                    catalogue.Load(Directory.GetFiles(directoryToScan, "*.dll"));
                }
                finally
                {
                    Console.SetOut(originalWriter);
                }
                
                messageWriter.Flush();
                actualMessage = messageWriter.Encoding.GetString(messageStream.GetBuffer());
            }

            // Assert
            actualMessage.Should().Match(expectedMessage);
        }

        [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "No exception is thrown.")]
        [Test]
        public void Should_warn_of_bad_assembly_files()
        {
            // Arrange
            var badAssemblyFile = Path.GetTempFileName() + ".dll";
            var expectedMessageFormat =
@"*Warning: FakeItEasy failed to load assembly '{0}' while scanning for extension points. Any IArgumentValueFormatters, IDummyFactories, and IFakeConfigurators in that assembly will not be available.
  *{0}*";

            var expectedMessage = string.Format(CultureInfo.InvariantCulture, expectedMessageFormat, badAssemblyFile);
            string actualMessage = null;

            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            {
                var catalogue = new TypeCatalogue();

                File.CreateText(badAssemblyFile).Close();
                try
                {
                    var originalWriter = Console.Out;
                    Console.SetOut(writer);
                    try
                    {
                        // Act
                        catalogue.Load(new[] { badAssemblyFile });
                    }
                    finally
                    {
                        Console.SetOut(originalWriter);
                    }
                }
                finally
                {
                    File.Delete(badAssemblyFile);
                }

                writer.Flush();
                actualMessage = writer.Encoding.GetString(stream.GetBuffer());
            }

            // Assert
            actualMessage.Should().Match(expectedMessage);
        }

        [Test]
        public void Should_be_able_to_get_types_from_fakeiteasy()
        {
            // Arrange
            var catalogue = new TypeCatalogue();

            // Act
            catalogue.Load(Directory.GetFiles(Environment.CurrentDirectory, "*.dll"));

            // Assert
            catalogue.GetAvailableTypes().Should().Contain(typeof(A));
        }

        [Test]
        public void Should_be_able_to_get_types_from_assembly_in_app_domain()
        {
            // Arrange
            var catalogue = new TypeCatalogue();

            // Act
            catalogue.Load(Directory.GetFiles(Environment.CurrentDirectory, "*.dll"));

            // Assert
            catalogue.GetAvailableTypes().Should().Contain(typeof(DoubleValueFormatter));
        }

        [Test]
        public void Should_be_able_to_get_types_from_external_assembly_in_directory()
        {
            // Arrange
            var catalogue = new TypeCatalogue();

            // Act
            catalogue.Load(Directory.GetFiles(Environment.CurrentDirectory, "*.dll"));
            
            // Assert
            catalogue.GetAvailableTypes().Select(type => type.FullName).Should().Contain("FakeItEasy.IntegrationTests.External.GuidValueFormatter");
        }

        [Test]
        public void Should_not_be_able_to_get_types_from_assembly_that_does_not_reference_fakeiteasy()
        {
            // Arrange
            var catalogue = new TypeCatalogue();

            // Act
            catalogue.Load(Directory.GetFiles(Environment.CurrentDirectory, "*.dll"));
            
            // Assert
            catalogue.GetAvailableTypes().Should().NotContain(typeof(string));
        }
    }
}