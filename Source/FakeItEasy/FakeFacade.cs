namespace FakeItEasy
{
    using System.Collections.Generic;
    using FakeItEasy.Core;

    internal class FakeFacade
    {
        private IFakeManagerAccessor fakeManagerAccessor;
        private IFakeScopeFactory fakeScopeFactory;

        public FakeFacade(IFakeManagerAccessor fakeManagerAccessor, IFakeScopeFactory fakeScopeFactory)
        {
            this.fakeManagerAccessor = fakeManagerAccessor;
            this.fakeScopeFactory = fakeScopeFactory;
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
    }
}
