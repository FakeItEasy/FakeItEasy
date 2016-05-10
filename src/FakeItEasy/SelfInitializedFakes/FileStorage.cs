namespace FakeItEasy.SelfInitializedFakes
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Newtonsoft.Json;

    internal class FileStorage
        : ICallStorage
    {
        private readonly string fileName;
        private readonly IFileSystem fileSystem;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileStorage"/> class.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="fileSystem">The file system.</param>
        public FileStorage(string fileName, IFileSystem fileSystem)
        {
            this.fileName = fileName;
            this.fileSystem = fileSystem;
        }

        /// <summary>
        /// A factory responsible for creating instances of FileStorage.
        /// </summary>
        /// <param name="fileName">The file name of the storage.</param>
        /// <returns>A FileStorage instance.</returns>
        public delegate FileStorage Factory(string fileName);

        private bool FileExists
        {
            get { return this.fileSystem.FileExists(this.fileName); }
        }

        /// <summary>
        /// Loads the recorded calls for the specified recording.
        /// </summary>
        /// <returns>
        /// The recorded calls for the recording with the specified id.
        /// </returns>
        public IEnumerable<CallData> Load()
        {
            if (!this.FileExists)
            {
                return null;
            }

            return this.LoadCallsFromFile();
        }

        /// <summary>
        /// Saves the specified calls as the recording with the specified id,
        /// overwriting any previous recording.
        /// </summary>
        /// <param name="calls">The calls to save.</param>
        public void Save(IEnumerable<CallData> calls)
        {
            this.EnsureThatFileExists();
            this.WriteCallsToFile(calls);
        }

        private void WriteCallsToFile(IEnumerable<CallData> calls)
        {
            using (var file = this.fileSystem.Open(this.fileName, FileMode.Truncate))
            using (var stream = new StreamWriter(file))
            {
                var serialized = JsonConvert.SerializeObject(calls.ToArray(),
                    Formatting.Indented,
                    new JsonSerializerSettings { NullValueHandling = NullValueHandling.Include });
                stream.Write(serialized);
            }
        }

        private IEnumerable<CallData> LoadCallsFromFile()
        {
            using (var file = this.fileSystem.Open(this.fileName, FileMode.Open))
            using (var stream = new StreamReader(file))
            {
                var json = stream.ReadToEnd();
                return JsonConvert.DeserializeObject<List<CallData>>(json,
                    new JsonSerializerSettings { NullValueHandling = NullValueHandling.Include });
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
