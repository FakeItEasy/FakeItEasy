namespace FakeItEasy.Creation
{
    using System;

    internal interface IFakeObjectCreator
    {
        bool TryCreateFakeObject(Type typeOfFake, DummyValueResolver resolver, out object result);
    }
}
