namespace FakeItEasy.Tests.TestHelpers
{
    using System;
    using System.IO;
#if FEATURE_BINARY_SERIALIZATION
    using System.Runtime.Serialization.Formatters.Binary;
#endif

    public static class BinarySerializationHelper
    {
#pragma warning disable CA1801 // parameter not used
        public static T SerializeAndDeserialize<T>(T value)
#pragma warning restore CA1801 // parameter not used
        {
#if FEATURE_BINARY_SERIALIZATION
            T result;

            var formatter = new BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, value);
                stream.Seek(0, SeekOrigin.Begin);
                result = (T)formatter.Deserialize(stream);
            }

            return result;
#else
            throw new System.PlatformNotSupportedException();
#endif
        }
    }
}
