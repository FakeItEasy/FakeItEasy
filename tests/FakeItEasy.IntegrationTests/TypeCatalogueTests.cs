namespace FakeItEasy.IntegrationTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Text;
#if FEATURE_NETCORE_REFLECTION
    using System.Reflection;
#endif
    using FakeItEasy.Core;
    using FluentAssertions;
    using Xunit;

    public class TypeCatalogueTests
    {
        [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "No exception is thrown.")]
#if FEATURE_NETCORE_REFLECTION
        [Fact(Skip = "Loading assemblies from duplicate files does not throw an error on this platform.")]
#else
        [Fact]
#endif
        public void Should_warn_of_duplicate_input_assemblies_with_different_paths()
        {
            // Arrange
            var originalExternalDll = this.GetPathToOriginalExternalDll();
            var copyOfExternalDll = this.GetPathToCopyOfExternalDll();
            File.Copy(originalExternalDll, copyOfExternalDll, overwrite: true);

            var expectedMessageFormat =
@"*Warning: FakeItEasy failed to load assembly '*{0}' while scanning for extension points. Any IArgumentValueFormatters, IDummyFactories, and IFakeOptionsBuilders in that assembly will not be available.*";
            var expectedMessage = string.Format(expectedMessageFormat, copyOfExternalDll);

            var catalogue = new TypeCatalogue();

            // Act
            var actualMessage = CaptureConsoleOutput(() => catalogue.Load(new[]
            {
                originalExternalDll, copyOfExternalDll
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
            catalogue.Load(new[] { this.GetPathToOriginalExternalDll() });

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

        private string GetPathToOriginalExternalDll()
        {
            var executingAssemblyPath = new Uri(this.GetType().GetTypeInfo().Assembly.CodeBase).LocalPath
                .Replace("FakeItEasy.IntegrationTests", "FakeItEasy.IntegrationTests.External")
                .Replace("netcoreapp1.0", "netstandard1.6");
            return Path.GetFullPath(executingAssemblyPath);
        }

        private string GetPathToCopyOfExternalDll()
        {
            var executingAssemblyPath = new Uri(this.GetType().GetTypeInfo().Assembly.Location).LocalPath;
            var executingAssemblyDirectory = new DirectoryInfo(executingAssemblyPath).Parent.FullName;
            return Path.GetFullPath(Path.Combine(
                executingAssemblyDirectory, "FakeItEasy.IntegrationTests.External.dll"));
        }
    }
}
