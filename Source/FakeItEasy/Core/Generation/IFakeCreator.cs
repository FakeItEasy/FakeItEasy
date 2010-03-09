namespace FakeItEasy.Core.Generation
{
    using System;

    internal interface IFakeCreator
    {
        T CreateFake<T>(Action<IFakeOptionsBuilder<T>> options);
        T CreateDummy<T>();
    }
}
