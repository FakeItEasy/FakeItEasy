namespace FakeItEasy
{
    using System.IO;

    /// <summary>
    /// Provides access to the file system.
    /// </summary>
    internal interface IFileSystem
    {
        /// <summary>
        /// Opens the specified file in the specified mode.
        /// </summary>
        /// <param name="fileName">The full path and name of the file to open.</param>
        /// <param name="mode">The mode to open the file in.</param>
        /// <returns>A stream for reading and writing the file.</returns>
        Stream Open(string fileName, FileMode mode);

        /// <summary>
        /// Gets a value indicating whether the specified file exists.
        /// </summary>
        /// <param name="fileName">The path and name of the file to check.</param>
        /// <returns>True if the file exists.</returns>
        bool FileExists(string fileName);

        /// <summary>
        /// Creates a file with the specified name.
        /// </summary>
        /// <param name="fileName">The name of the file to create.</param>
        void Create(string fileName);
    }
}