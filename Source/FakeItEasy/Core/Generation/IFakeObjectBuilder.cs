namespace FakeItEasy.Core.Generation
{
    using System;
    using System.Diagnostics;
    using System.Collections.Generic;

    internal interface IFakeObjectBuilder
    {
        object BuildFakeObject(Type typeOfFake, FakeObjectBuilderOptions options);
    }
}