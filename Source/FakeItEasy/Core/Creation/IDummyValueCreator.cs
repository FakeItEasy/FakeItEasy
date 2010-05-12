namespace FakeItEasy.Core.Creation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    
    public interface IDummyValueCreator
    {
        bool TryCreateDummyValue(Type type, out object dummy);
    }
}