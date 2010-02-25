namespace FakeItEasy.Mef
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;

    [InheritedExport(typeof(IFakeDefinition))]
    public interface IFakeDefinition
    {
        Type ForType { get; }
        object CreateFake();
    }
}
