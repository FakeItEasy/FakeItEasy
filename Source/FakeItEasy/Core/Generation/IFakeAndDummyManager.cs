namespace FakeItEasy.Core.Generation
{
    using System;

    internal interface IFakeAndDummyManager
    {
        object CreateDummy();
        object CreateFake(Type typeOfFake, FakeOptions options);
        bool TryCreateDummy(Type typeOfFake);
        bool TryCreateFake(Type typeOfFake, FakeOptions options, out object result);
    }
}