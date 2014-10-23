namespace FakeItEasy.Tests.TestHelpers
{
    using System;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;

    public static class BinarySerializationHelper
    {
        public static T SerializeAndDeserialize<T>(T value)
        {
            T result;

            var formatter = new BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, value);
                stream.Seek(0, SeekOrigin.Begin);
                result = (T)formatter.Deserialize(stream);
            }

            return result;
        }
    }
}