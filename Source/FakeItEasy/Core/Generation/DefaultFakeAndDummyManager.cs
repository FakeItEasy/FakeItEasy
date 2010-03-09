namespace FakeItEasy.Core.Generation
{
    using System;

    internal class DefaultFakeAndDummyManager
        : IFakeAndDummyManager
    {
        public object CreateDummy(Type typeOfDummy)
        {
            throw new NotImplementedException();
        }

        public object CreateFake(Type typeOfDummy, FakeOptions options)
        {
            throw new NotImplementedException();
        }

        public bool TryCreateDummy(Type typeOfFake)
        {
            throw new NotImplementedException();
        }

        public bool TryCreateFake(Type typeOfFake, FakeOptions options, out object result)
        {
            throw new NotImplementedException();
        }
    }
}
