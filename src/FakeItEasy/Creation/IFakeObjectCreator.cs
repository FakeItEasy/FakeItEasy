namespace FakeItEasy.Creation
{
    using System;

    internal interface IFakeObjectCreator
    {
        bool TryCreateFakeObject(DummyCreationSession session, Type typeOfFake, DummyValueResolver resolver, out object result);
    }
}
