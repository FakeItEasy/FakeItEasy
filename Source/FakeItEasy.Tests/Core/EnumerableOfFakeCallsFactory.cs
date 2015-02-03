namespace FakeItEasy.Tests.Core
{
    using System.Collections.Generic;
    using System.Linq;
    using FakeItEasy.Core;

    public class EnumerableOfFakeCallsFactory : DummyFactory<IEnumerable<IFakeObjectCall>>
    {
        protected override IEnumerable<IFakeObjectCall> Create()
        {
            return Enumerable.Empty<IFakeObjectCall>();
        }
    }
}
