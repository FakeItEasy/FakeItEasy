namespace FakeItEasy.Core.Generation
{
    using System;
    using System.Diagnostics;
    using System.Collections.Generic;

    internal interface IFakeAndDummyManager
    {
        T CreateFake<T>(Action<IFakeBuilderOptionsBuilder<T>> options);
        T CreateDummy<T>();
    }
}
