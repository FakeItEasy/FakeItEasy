namespace FakeItEasy.SelfInitializedFakes
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization.Formatters.Binary;

    internal class FileStorage
        : ICallStorage
    {
        /// <summary>
        /// A factory responsible for creating instances of FileStorage.
        /// </summary>
        /// <param name="fileName">The file name of the storage.</param>
        /// <returns>A FileStorage instance.</returns>
        public delegate FileStorage Factory(string fileName);

        private readonly string fileName;
        private readonly IFileSystem fileSystem;

        public FileStorage(string fileName, IFileSystem fileSystem)
        {
            this.fileName = fileName;
            this.fileSystem = fileSystem;
        }

        public IEnumerable<CallData> Load()
        {
            if (!this.FileExists)
            {
                return null;
            }

            return this.LoadCallsFromFile();
        }

        public void Save(IEnumerable<CallData> calls)
        {
            this.EnsureThatFileExists();
            this.WriteCallsToFile(calls);
        }

        private void WriteCallsToFile(IEnumerable<CallData> calls)
        {
            var formatter = new BinaryFormatter();
            using (var file = this.fileSystem.Open(this.fileName, FileMode.Truncate))
            {
                formatter.Serialize(file, calls.ToArray());
            }
        }

        private IEnumerable<CallData> LoadCallsFromFile()
        {
            var formatter = new BinaryFormatter();
            using (var file = this.fileSystem.Open(this.fileName, FileMode.Open))
            {
                return (IEnumerable<CallData>)formatter.Deserialize(file);
            }
        }

        private bool FileExists
        {
            get
            {
                return this.fileSystem.FileExists(this.fileName);
            }
        }

        private void EnsureThatFileExists()
        {
            if (!this.FileExists)
            {
                this.fileSystem.Create(this.fileName);
            }
        }
    }
}