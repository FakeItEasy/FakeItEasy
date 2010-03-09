namespace FakeItEasy.Core.Generation
{
    using System;

    internal class DefaultFakeAndDummyManager
        : IFakeAndDummyManager
    {

        public bool TryCreateFake(Type typeOfFake, FakeOptions options, out object result)
        {
            throw new NotImplementedException();
        }

        public object CreateFake(Type typeOfFake, FakeOptions options)
        {
            throw new NotImplementedException();
        }

        public bool TryCreateDummy(Type typeOfFake)
        {
            throw new NotImplementedException();
        }

        public object CreateDummy()
        {
            throw new NotImplementedException();
        }
    }
}
