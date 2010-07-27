namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class OrderedAssertion
    {
        public static IDisposable OrderedAssertions(this IEnumerable<ICompletedFakeObjectCall> calls)
        {
#if DEBUG
            throw new NotImplementedException();
#else
#error "Must be implemented"
#endif
        }

        private class AsserterResetter
            : IDisposable
        {
            public FakeAsserter.Factory ResetTo;

            public void Dispose()
            {
#if DEBUG
                throw new NotImplementedException();
#else
#error "Must be implemented"
#endif
            }
        }
    }
}
