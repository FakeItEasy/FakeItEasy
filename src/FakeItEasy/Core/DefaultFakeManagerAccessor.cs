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
        /// <exception cref="ArgumentException">If <paramref name="proxy"/> is not actually a faked object.</exception>
        public FakeManager GetFakeManager(object proxy)
        {
            Guard.AgainstNull(proxy, nameof(proxy));

            FakeManager result = this.TryGetFakeManager(proxy);

            if (result == null)
            {
                throw new ArgumentException(ExceptionMessages.NotRecognizedAsAFake(proxy, proxy.GetType()));
            }

            return result;
        }

        /// <summary>
        /// Gets the fake manager associated with the proxy, if any.
        /// </summary>
        /// <param name="proxy">The proxy to get the manager from.</param>
        /// <returns>The fake manager, or <c>null</c> if <paramref name="proxy"/> is not actually a faked object.</returns>
        public FakeManager TryGetFakeManager(object proxy)
        {
            Guard.AgainstNull(proxy, nameof(proxy));

            var taggable = AsTaggable(proxy);

            return taggable.Tag as FakeManager;
        }

        public void TagProxy(object proxy, FakeManager manager)
        {
            Guard.AgainstNull(proxy, nameof(proxy));
            Guard.AgainstNull(manager, nameof(manager));

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
