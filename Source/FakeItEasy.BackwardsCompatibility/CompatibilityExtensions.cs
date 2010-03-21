using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FakeItEasy.Configuration;
using System.Linq.Expressions;
using System.ComponentModel;
using FakeItEasy.Assertion;
using FakeItEasy.Expressions;

namespace FakeItEasy
{
    public static class CompatibilityExtensions
    {
        [Obsolete, EditorBrowsable(EditorBrowsableState.Never)]
        public static void MustHaveHappened(this IAssertConfiguration configuration, Expression<Func<int, bool>> repeatPredicate)
        {
            configuration.MustHaveHappened(Repeated.Like(repeatPredicate));
        }

        [Obsolete, EditorBrowsable(EditorBrowsableState.Never)]
        public static void Assert(this IAssertConfiguration configuration, Repeated repeatValidation)
        {
            configuration.MustHaveHappened(repeatValidation);
        }
    }

    public static class OldFake
    {
        public static IFakeAssertions<T> Assert<T>(T fakedObject)
        {
            var asserter = new FakeAssertions<T>(Fake.GetFakeObject(fakedObject), ServiceLocator.Current.Resolve<IExpressionCallMatcherFactory>(), ServiceLocator.Current.Resolve<FakeAsserter.Factory>());
            return asserter;
        }
    }
}