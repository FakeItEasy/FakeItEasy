namespace FakeItEasy.Core.Creation
{
    using System;

    internal interface IDummyValueCreationSession
    {
        bool TryResolveDummyValue(Type typeOfDummy, out object result);
    }
}
