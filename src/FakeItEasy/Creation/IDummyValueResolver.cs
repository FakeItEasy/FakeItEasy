namespace FakeItEasy.Creation
{
    using System;

    internal interface IDummyValueResolver
    {
        CreationResult TryResolveDummyValue(DummyCreationSession session, Type typeOfDummy);
    }
}
