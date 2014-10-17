namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;

    internal static class LazyInterceptionSinkProvider
    {
        public delegate ILazyInterceptionSinkProvider Factory(Type typeOfFake);
    }
}