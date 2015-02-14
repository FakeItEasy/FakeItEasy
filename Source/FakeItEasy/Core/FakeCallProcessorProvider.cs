namespace FakeItEasy.Core
{
    using System;
    using FakeItEasy.Creation;

    internal static class FakeCallProcessorProvider
    {
        public delegate IFakeCallProcessorProvider Factory(Type typeOfFake, FakeOptions fakeOptions);
    }
}