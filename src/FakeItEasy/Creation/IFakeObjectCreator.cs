namespace FakeItEasy.Creation
{
    using System;

    internal interface IFakeObjectCreator
    {
        CreationResult TryCreateFakeObject(DummyCreationSession session, Type typeOfFake, DummyValueResolver resolver);
    }
}
