namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using FakeItEasy.Creation;

    /// <summary>
    /// Default implementation of <see cref="IFakeManagerAccessor"/>.
    /// </summary>
    internal class DefaultFakeManagerAccessor
        : IFakeManagerAccessor
    {
        /// <summary>
        /// Gets the fake manager associated with the proxy.
        /// </summary>
        /// <param name="proxy">The proxy to get the manager from.</param>
        /// <returns>A fake manager.</returns>
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

        public void TagProxy(object proxy, FakeManager manager)
        {
            Guard.AgainstNull(proxy, "proxy");
            Guard.AgainstNull(manager, "manager");

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