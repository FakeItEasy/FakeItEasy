namespace FakeItEasy.Creation
{
    using System;

    internal interface IFakeObjectCreator
    {
        CreationResult CreateFake(Type typeOfFake, IProxyOptions proxyOptions, DummyCreationSession session, IDummyValueResolver resolver);
    }
}
