namespace FakeItEasy.Tests.Core
{
    using System.Collections.Generic;
    using System.Linq;
    using FakeItEasy.Core;

    public class EnumerableOfFakeCallsDefinition : DummyDefinition<IEnumerable<IFakeObjectCall>>
    {
        protected override IEnumerable<IFakeObjectCall> CreateDummy()
        {
            return Enumerable.Empty<IFakeObjectCall>();
        }
    }
}
