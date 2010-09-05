namespace FakeItEasy.Core.Creation
{
    using System;

    internal interface IFakeObjectCreator
    {
        bool TryCreateFakeObject(Type typeOfFake, DummyValueCreationSession session, out object result);
    }
}
