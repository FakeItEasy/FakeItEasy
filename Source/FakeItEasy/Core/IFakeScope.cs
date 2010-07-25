namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;
    public interface IFakeScope
            : IDisposable, IEnumerable<ICompletedFakeObjectCall>
    {

    }
}
