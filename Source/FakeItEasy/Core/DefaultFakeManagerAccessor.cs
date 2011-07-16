namespace FakeItEasy.Core
{
    using System;
    using FakeItEasy.Creation;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Default implementation of the fake manager attacher.
    /// </summary>
    internal class DefaultFakeManagerAccessor
        : IFakeManagerAccessor
    {
        private readonly FakeManager.Factory managerFactory;

        public DefaultFakeManagerAccessor(FakeManager.Factory managerFactory)
        {
            this.managerFactory = managerFactory;
        }

        /// <summary>
        /// Attaches a fakemanager to the specified proxy, listening to
        /// the event raiser.
        /// </summary>
        /// <param name="typeOfFake">The type of the fake object proxy.</param>
        /// <param name="proxy">The proxy to attach to.</param>
        /// <param name="eventRaiser">The event raiser to listen to.</param>
        public void AttachFakeManagerToProxy(Type typeOfFake, object proxy, ICallInterceptedEventRaiser eventRaiser)
        {
            var manager = this.managerFactory.Invoke();

            TagProxy(proxy, manager);

            manager.AttachProxy(typeOfFake, proxy, eventRaiser);
        }

        /// <summary>
        /// Gets the fake manager associated with the proxy.
        /// </summary>
        /// <param name="proxy">The proxy to get the manager from.</param>
        /// <returns>A fake manager</returns>
        public FakeManager GetFakeManager(object proxy)
        {
            Guard.AgainstNull(proxy, "proxy");

            var taggable = AsTaggable(proxy);

            var result = taggable.Tag as FakeManager;

            if (result == null)
            {
                throw new ArgumentException("The specified object is not recognized as a fake object.");
            }

            return result;
        }

        private static void TagProxy(object proxy, FakeManager manager)
        {
            var taggable = AsTaggable(proxy);

            taggable.Tag = manager;
        }

        private static ITaggable AsTaggable(object proxy)
        {
            var taggable = proxy as ITaggable;

            if (taggable == null)
            {
                taggable = new TaggableAdaptor(proxy);
            }

            return taggable;
        }

        private class TaggableAdaptor : ITaggable
        {
            private static readonly ConditionalWeakTable<object, object> Tags = new ConditionalWeakTable<object, object>();
            private readonly object taggedInstance;

            public TaggableAdaptor(object taggedInstance)
            {
                this.taggedInstance = taggedInstance;
            }

            public object Tag
            {
                get
                {
                    object result = null;

                    Tags.TryGetValue(this.taggedInstance, out result);

                    return result;
                }
                set
                {
                    Tags.Add(this.taggedInstance, value);
                }
            }
        }

    }
}