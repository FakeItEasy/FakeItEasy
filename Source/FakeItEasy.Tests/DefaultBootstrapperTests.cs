namespace FakeItEasy.Tests
{
    using FluentAssertions;
    using NUnit.Framework;

    public static class DefaultBootstrapperTests
    {
        [Test]
        public static void GetAssemblyFileNamesToScanForExtensions_should_return_empty_list()
        {
            // Arrange
            var bootstrapper = new DefaultBootstrapper();
            
            // Act
            var assemblyFileNames = bootstrapper.GetAssemblyFileNamesToScanForExtensions();

            // Assert
            assemblyFileNames.Should().BeEmpty();
        }
    }
}
