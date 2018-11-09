namespace FakeItEasy.Tests.TestHelpers
{
#if FEATURE_BINARY_SERIALIZATION
    using System;

    [Serializable]
#endif
    internal class InternalClassVisibleToDynamicProxy
    {
        public virtual void Foo(int argument1, int argument2)
        {
        }
    }
}
