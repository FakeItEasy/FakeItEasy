namespace FakeItEasy
{
    using System;
    using System.Collections.Generic;
    using FakeItEasy.Core;

    internal class FakeFacade
    {
        private readonly IFakeManagerAccessor fakeManagerAccessor;

        public FakeFacade(IFakeManagerAccessor fakeManagerAccessor)
        {
            this.fakeManagerAccessor = fakeManagerAccessor;
        }

        public virtual FakeManager GetFakeManager(object fakedObject)
        {
            Guard.AgainstNull(fakedObject, nameof(fakedObject));

            return this.fakeManagerAccessor.GetFakeManager(fakedObject);
        }

        public virtual FakeManager TryGetFakeManager(object fakedObject)
        {
            Guard.AgainstNull(fakedObject, nameof(fakedObject));

            return this.fakeManagerAccessor.TryGetFakeManager(fakedObject);
        }

        public virtual IEnumerable<ICompletedFakeObjectCall> GetCalls(object fakedObject)
        {
            Guard.AgainstNull(fakedObject, nameof(fakedObject));

            var manager = this.fakeManagerAccessor.GetFakeManager(fakedObject);
            return manager.GetRecordedCalls();
        }

        public virtual void ClearConfiguration(object fakedObject)
        {
            Guard.AgainstNull(fakedObject, nameof(fakedObject));

            var manager = this.fakeManagerAccessor.GetFakeManager(fakedObject);
            manager.ClearUserRules();
        }

        public virtual void ClearRecordedCalls(object fakedObject)
        {
            Guard.AgainstNull(fakedObject, nameof(fakedObject));

            var manager = this.fakeManagerAccessor.GetFakeManager(fakedObject);
            manager.ClearRecordedCalls();
        }

        public void InitializeFixture(object fixture)
        {
            Guard.AgainstNull(fixture, nameof(fixture));

#pragma warning disable CS0618 // Type or member is obsolete
            FixtureInitializer.InitializeFakes(fixture);
#pragma warning restore CS0618 // Type or member is obsolete
        }
    }
}
