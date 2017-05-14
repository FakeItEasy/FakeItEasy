namespace FakeItEasy.IntegrationTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
#if FEATURE_NETCORE_REFLECTION
    using System.Reflection;
#endif
    using FakeItEasy.Core;
    using FluentAssertions;
    using Xunit;

    // Note: The console output is captured in all tests in this class, even when we don't need to use it.
    // This is done in order to suppress a warning that sometimes occurs when these tests run after a fake
    // has already been created. This situation normally happens only during tests, not during normal usage
    // of the library.
    public class TypeCatalogueTests : IClassFixture<ExternalAssemblyGenerator>
    {
        public TypeCatalogueTests(ExternalAssemblyGenerator externalAssemblyGenerator)
        {
            this.externalAssemblyGenerator = externalAssemblyGenerator;
        }

        private readonly ExternalAssemblyGenerator externalAssemblyGenerator;

        [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "No exception is thrown.")]
#if FEATURE_NETCORE_REFLECTION
        [Fact(Skip = "Loading assemblies from duplicate files does not throw an error on this platform.")]
#else
        [Fact]
#endif
        public void Should_warn_of_duplicate_input_assemblies_with_different_paths()
        {
            // Arrange
            var expectedMessageFormat =
@"*Warning: FakeItEasy failed to load assembly '*{0}' while scanning for extension points. Any IArgumentValueFormatters, IDummyFactories, and IFakeOptionsBuilders in that assembly will not be available.*";
            var expectedMessage = string.Format(expectedMessageFormat, this.externalAssemblyGenerator.AssemblyCopyPath);

            var catalogue = new TypeCatalogue();

            // Act
            var actualMessage = CaptureConsoleOutput(() => catalogue.Load(new[]
            {
                this.externalAssemblyGenerator.AssemblyOriginalPath, this.externalAssemblyGenerator.AssemblyCopyPath
            }));

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
@"*Warning: FakeItEasy failed to load assembly '{0}' while scanning for extension points. Any IArgumentValueFormatters, IDummyFactories, and IFakeOptionsBuilders in that assembly will not be available.*";

            var expectedMessage = string.Format(expectedMessageFormat, badAssemblyFile);
            string actualMessage;

            var catalogue = new TypeCatalogue();

            try
            {
                File.CreateText(badAssemblyFile).Dispose();

                // Act
                actualMessage = CaptureConsoleOutput(() => catalogue.Load(new[] { badAssemblyFile }));
            }
            finally
            {
                File.Delete(badAssemblyFile);
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
            CaptureConsoleOutput(() => catalogue.Load(Enumerable.Empty<string>()));

            // Assert
            catalogue.GetAvailableTypes().Should().Contain(typeof(A));
        }

        [Fact]
        public void Should_be_able_to_get_types_from_assembly_in_app_domain()
        {
            // Arrange
            var catalogue = new TypeCatalogue();

            // Act
            CaptureConsoleOutput(() => catalogue.Load(Enumerable.Empty<string>()));

            // Assert
            catalogue.GetAvailableTypes().Should().Contain(typeof(DoubleValueFormatter));
        }

        [Fact]
        public void Should_be_able_to_get_types_from_external_assembly()
        {
            // Arrange
            var catalogue = new TypeCatalogue();

            // Act
            CaptureConsoleOutput(() => catalogue.Load(new[] { this.externalAssemblyGenerator.AssemblyOriginalPath }));

            // Assert
            catalogue.GetAvailableTypes().Select(type => type.FullName).Should().Contain("FakeItEasy.IntegrationTests.External.GuidValueFormatter");
        }

        [Fact]
        public void Should_not_be_able_to_get_types_from_assembly_that_does_not_reference_fakeiteasy()
        {
            // Arrange
            var catalogue = new TypeCatalogue();

            // Act
            CaptureConsoleOutput(() => catalogue.Load(Enumerable.Empty<string>()));

            // Assert
            catalogue.GetAvailableTypes().Should().NotContain(typeof(string));
        }

        [Fact]
        public void Should_warn_if_some_types_cannot_be_loaded_from_external_assembly()
        {
            // Arrange
            var catalogue = new TypeCatalogue();

            // Act
            string output = CaptureConsoleOutput(
                () => catalogue.Load(new[] { this.externalAssemblyGenerator.AssemblyOriginalPath }));

            // Assert
            output.Should().Match(@"*Warning: FakeItEasy failed to get some types from assembly 'FakeItEasy.ExtensionPoints.External, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null' while scanning for extension points. Some IArgumentValueFormatters, IDummyFactories, and IFakeOptionsBuilders in that assembly might not be available.
  System.Reflection.ReflectionTypeLoadException: *.
  1 type(s) were not loaded for the following reasons:
   - System.IO.FileNotFoundException: *");
        }

        [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "Safe in this case")]
        private static string CaptureConsoleOutput(Action action)
        {
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            {
                var originalWriter = Console.Out;
                Console.SetOut(writer);
                try
                {
                    action();
                }
                finally
                {
                    Console.SetOut(originalWriter);
                }

                writer.Flush();
                return writer.Encoding.GetString(stream.ToArray());
            }
        }
    }
}
