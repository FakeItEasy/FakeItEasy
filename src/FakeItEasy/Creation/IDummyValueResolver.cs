namespace FakeItEasy.Creation
{
    using System;

    internal interface IDummyValueResolver
    {
        bool TryResolveDummyValue(DummyCreationSession session, Type typeOfDummy, out object result);
    }
}
