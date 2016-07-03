namespace FakeItEasy.Tests
{
    using System;
    using System.Runtime.Serialization;

#if FEATURE_BINARY_SERIALIZATION
    [Serializable]
#endif
    public class DummyException
        : Exception
    {
        public DummyException()
        {
        }

        public DummyException(string message)
            : base(message)
        {
        }

        public DummyException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

#if FEATURE_BINARY_SERIALIZATION
        protected DummyException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif
    }
}
