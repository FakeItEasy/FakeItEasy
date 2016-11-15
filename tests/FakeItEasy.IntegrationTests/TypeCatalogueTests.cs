namespace FakeItEasy.IntegrationTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using FakeItEasy.Core;
    using FluentAssertions;
    using Xunit;

    public class TypeCatalogueTests
    {
        [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "No exception is thrown.")]
        [Fact]
        public void Should_warn_of_duplicate_input_assemblies_with_different_paths()
        {
            // Arrange
            var originalExternalDll = GetPathToOriginalExternalDll();
            var copyOfExternalDll = GetPathToCopyOfExternalDll();

            var expectedMessageFormat =
@"*Warning: FakeItEasy failed to load assembly '*{0}' while scanning for extension points. Any IArgumentValueFormatters, IDummyFactories, and IFakeOptionsBuilders in that assembly will not be available.*";
            var expectedMessage = string.Format(CultureInfo.InvariantCulture, expectedMessageFormat, copyOfExternalDll);
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
                    catalogue.Load(new[] { originalExternalDll, copyOfExternalDll });
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
        [Fact]
        public void Should_warn_of_bad_assembly_files()
        {
            // Arrange
            var badAssemblyFile = Path.GetTempFileName() + ".dll";
            var expectedMessageFormat =
@"*Warning: FakeItEasy failed to load assembly '{0}' while scanning for extension points. Any IArgumentValueFormatters, IDummyFactories, and IFakeOptionsBuilders in that assembly will not be available.
  *{0}*";

            var expectedMessage = string.Format(CultureInfo.InvariantCulture, expectedMessageFormat, badAssemblyFile);
            string actualMessage;

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

        [Fact]
        public void Should_be_able_to_get_types_from_fakeiteasy()
        {
            // Arrange
            var catalogue = new TypeCatalogue();

            // Act
            catalogue.Load(Enumerable.Empty<string>());

            // Assert
            catalogue.GetAvailableTypes().Should().Contain(typeof(A));
        }

        [Fact]
        public void Should_be_able_to_get_types_from_assembly_in_app_domain()
        {
            // Arrange
            var catalogue = new TypeCatalogue();

            // Act
            catalogue.Load(Enumerable.Empty<string>());

            // Assert
            catalogue.GetAvailableTypes().Should().Contain(typeof(DoubleValueFormatter));
        }

        [Fact]
        public void Should_be_able_to_get_types_from_external_assembly()
        {
            // Arrange
            var catalogue = new TypeCatalogue();

            // Act
            catalogue.Load(new[] { GetPathToOriginalExternalDll() });

            // Assert
            catalogue.GetAvailableTypes().Select(type => type.FullName).Should().Contain("FakeItEasy.IntegrationTests.External.GuidValueFormatter");
        }

        [Fact]
        public void Should_not_be_able_to_get_types_from_assembly_that_does_not_reference_fakeiteasy()
        {
            // Arrange
            var catalogue = new TypeCatalogue();

            // Act
            catalogue.Load(Enumerable.Empty<string>());

            // Assert
            catalogue.GetAvailableTypes().Should().NotContain(typeof(string));
        }

        private static string GetPathToOriginalExternalDll()
        {
            var currentDirectory = Environment.CurrentDirectory;

            return Path.GetFullPath(Path.Combine(
                currentDirectory,
                @"..\..\..\FakeItEasy.IntegrationTests.External\bin",
                new DirectoryInfo(currentDirectory).Name,
                "FakeItEasy.IntegrationTests.External.dll"));
        }

        private static string GetPathToCopyOfExternalDll()
        {
            return Path.GetFullPath(Path.Combine(
                Environment.CurrentDirectory,
                "FakeItEasy.IntegrationTests.External.dll"));
        }
    }
}
