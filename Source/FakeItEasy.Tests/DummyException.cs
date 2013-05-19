namespace FakeItEasy.Tests
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
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

        protected DummyException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
