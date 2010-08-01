namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class OrderedAssertion
    {
        public static IDisposable OrderedAssertions(this IEnumerable<ICompletedFakeObjectCall> calls)
        {
            throw new MustBeImplementedException();
        }

        private class AsserterResetter
            : IDisposable
        {
            public FakeAsserter.Factory ResetTo;

            public void Dispose()
            {
                throw new MustBeImplementedException();
            }
        }
    }
}
