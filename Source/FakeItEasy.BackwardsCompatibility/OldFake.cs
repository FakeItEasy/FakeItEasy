namespace FakeItEasy
{
    using System;
    using FakeItEasy.Assertion;
    using FakeItEasy.Core;
    using FakeItEasy.Expressions;

    /// <summary>
    /// Provides backwards compatibility methods for the FakeItEasy.Fake-class.
    /// </summary>
    public static class OldFake
    {
        [Obsolete("Use the A.CallTo(() => foo.Bar()).MustHaveHappened()-syntax instead.")]
        public static IFakeAssertions<T> Assert<T>(T fakedObject)
        {
            var asserter = new FakeAssertions<T>(Fake.GetFakeManager(fakedObject), ServiceLocator.Current.Resolve<IExpressionCallMatcherFactory>(), ServiceLocator.Current.Resolve<FakeAsserter.Factory>());
            return asserter;
        }
    }
}
