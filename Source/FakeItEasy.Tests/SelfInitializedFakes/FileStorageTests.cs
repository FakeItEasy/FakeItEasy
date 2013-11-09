namespace FakeItEasy.Tests.SelfInitializedFakes
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization.Formatters.Binary;
    using FakeItEasy.SelfInitializedFakes;
    using NUnit.Framework;

    [TestFixture]
    public class FileStorageTests
    {
        private IFileSystem fileSystem;

        [SetUp]
        public void Setup()
        {
            this.fileSystem = A.Fake<IFileSystem>();
            A.CallTo(() => this.fileSystem.FileExists(A<string>._)).Returns(true);
            A.CallTo(() => this.fileSystem.Open(A<string>._, A<FileMode>._)).Returns(new MemoryStream());
        }

        [Test]
        public void Load_should_deserialize_calls_from_file()
        {
            // Arrange
            var calls = new[]
                {
                    this.CreateDummyCallData(),
                    this.CreateDummyCallData()
                };

            var serializedCalls = this.SerializeCalls(calls);
            A.CallTo(() => this.fileSystem.Open("c:\\file.dat", FileMode.Open)).ReturnsLazily(x => new MemoryStream(serializedCalls));

            // Act
            var storage = this.CreateStorage("c:\\file.dat");
            var loadedCalls = storage.Load();

            // Assert
            Assert.That(loadedCalls.SequenceEqual(calls, new CallDataComparer()));
        }

        [Test]
        public void Load_should_return_null_when_file_does_not_exist()
        {
            // Arrange
            A.CallTo(() => this.fileSystem.FileExists("c:\\file.dat")).Returns(false);

            // Act
            var storage = this.CreateStorage("c:\\file.dat");
            var loadedCalls = storage.Load();

            // Assert
            Assert.That(loadedCalls, Is.Null);
        }

        [Test]
        public void Save_should_serialize_calls_to_file()
        {
            // Arrange
            var fileStream = new MemoryStream();

            var calls =
                from index in Enumerable.Range(1, 2)
                select this.CreateDummyCallData();

            A.CallTo(() => this.fileSystem.Open("c:\\file.dat", FileMode.Truncate)).Returns(fileStream);

            // Act
            var storage = this.CreateStorage("c:\\file.dat");
            storage.Save(calls);

            // Assert
            var savedCalls = this.DeserializeCalls(fileStream.GetBuffer());

            Assert.That(savedCalls.SequenceEqual(calls, new CallDataComparer()));
        }

        [Test]
        public void Save_should_create_file_when_it_does_not_exist()
        {
            // Arrange
            A.CallTo(() => this.fileSystem.FileExists("c:\\file.dat")).Returns(false);

            // Act
            var storage = this.CreateStorage("c:\\file.dat");
            storage.Save(Enumerable.Empty<CallData>());

            // Assert
            A.CallTo(() => this.fileSystem.Create("c:\\file.dat")).MustHaveHappened();
        }

        private FileStorage CreateStorage(string fileName)
        {
            return new FileStorage(fileName, this.fileSystem);
        }

        private byte[] SerializeCalls(IEnumerable<CallData> calls)
        {
            using (var stream = new MemoryStream())
            {
                new BinaryFormatter().Serialize(stream, calls);
                return stream.GetBuffer();
            }
        }

        private IEnumerable<CallData> DeserializeCalls(byte[] serializedCalls)
        {
            using (var stream = new MemoryStream(serializedCalls))
            {
                return (IEnumerable<CallData>)new BinaryFormatter().Deserialize(stream);
            }
        }

        private CallData CreateDummyCallData()
        {
            return new CallData(typeof(IFoo).GetMethod("Bar", new Type[] { }), new object[] { }, null);
        }

        private class CallDataComparer : IEqualityComparer<CallData>
        {
            public bool Equals(CallData x, CallData y)
            {
                if (x == null)
                {
                    return y == null;
                }

                if (y == null)
                {
                    return false;
                }

                return object.Equals(x.Method, y.Method) && x.OutputArguments.SequenceEqual(y.OutputArguments) && object.Equals(x.ReturnValue, y.ReturnValue);
            }

            public int GetHashCode(CallData obj)
            {
                throw new NotImplementedException();
            }
        }
    }
}
