namespace FakeItEasy.DynamicProxy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FakeItEasy.Core;
    using FakeItEasy.Core.Creation;

    public interface IDummyValueCreator
    {
        bool TryCreateDummyValue(Type type, out object dummy);
    }
}