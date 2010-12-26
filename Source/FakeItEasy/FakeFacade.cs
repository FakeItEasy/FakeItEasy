namespace FakeItEasy
{
    using System.Collections.Generic;
    using FakeItEasy.Core;

    internal class FakeFacade
    {
        private readonly IFixtureInitializer fakeInitializer;
        private readonly IFakeManagerAccessor fakeManagerAccessor;
        private readonly IFakeScopeFactory fakeScopeFactory;

        public FakeFacade(IFakeManagerAccessor fakeManagerAccessor, IFakeScopeFactory fakeScopeFactory, IFixtureInitializer fakeInitializer)
        {
            this.fakeManagerAccessor = fakeManagerAccessor;
            this.fakeScopeFactory = fakeScopeFactory;
            this.fakeInitializer = fakeInitializer;
        }

        public virtual FakeManager GetFakeManager(object fakedObject)
        {
            Guard.AgainstNull(fakedObject, "fakedObject");

            return this.fakeManagerAccessor.GetFakeManager(fakedObject);
        }

        public virtual IFakeScope CreateScope()
        {
            return this.fakeScopeFactory.Create();
        }

        public virtual IFakeScope CreateScope(IFakeObjectContainer container)
        {
            Guard.AgainstNull(container, "container");

            return this.fakeScopeFactory.Create(container);
        }

        public virtual IEnumerable<ICompletedFakeObjectCall> GetCalls(object fakedObject)
        {
            Guard.AgainstNull(fakedObject, "fakedObject");

            var manager = this.fakeManagerAccessor.GetFakeManager(fakedObject);
            return manager.RecordedCallsInScope;
        }

        public virtual void ClearConfiguration(object fakedObject)
        {
            Guard.AgainstNull(fakedObject, "fakedObject");

            var manager = this.fakeManagerAccessor.GetFakeManager(fakedObject);
            manager.ClearUserRules();
        }

        public void InitializeFixture(object fixture)
        {
            Guard.AgainstNull(fixture, "fixture");

            this.fakeInitializer.InitializeFakes(fixture);
        }
    }
}