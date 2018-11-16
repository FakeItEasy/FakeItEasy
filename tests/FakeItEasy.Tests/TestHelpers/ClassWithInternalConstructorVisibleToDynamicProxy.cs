namespace FakeItEasy.Tests.TestHelpers
{
#if FEATURE_BINARY_SERIALIZATION
    using System;

    [Serializable]
#endif
    public class ClassWithInternalConstructorVisibleToDynamicProxy
    {
        internal ClassWithInternalConstructorVisibleToDynamicProxy()
        {
        }
    }
}
