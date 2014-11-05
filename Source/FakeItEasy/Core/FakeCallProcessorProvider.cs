namespace FakeItEasy.Core
{
    using System;

    internal static class FakeCallProcessorProvider
    {
        public delegate IFakeCallProcessorProvider Factory(Type typeOfFake);
    }
}