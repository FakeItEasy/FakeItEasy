namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;

    internal static class FakeCallProcessorProvider
    {
        public delegate IFakeCallProcessorProvider Factory(Type typeOfFake, IEnumerable<Action<object>> onFakeConfigurationActions);
    }
}