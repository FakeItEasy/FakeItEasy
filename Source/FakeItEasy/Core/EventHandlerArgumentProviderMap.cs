namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;

    internal class EventHandlerArgumentProviderMap
    {
        private readonly Dictionary<object, Func<object, object[]>> map = new Dictionary<object, Func<object, object[]>>();

        public void AddArgumentProvider(object eventHandler, Func<object, object[]> provider)
        {
            lock (this.map)
            {
                this.map[eventHandler] = provider;
            }
        }

        public bool TryGetArgumentProviderFor(object eventHandler, out Func<object, object[]> provider)
        {
            lock (this.map)
            {
                bool wasFound = this.map.TryGetValue(eventHandler, out provider);
                if (wasFound)
                {
                    this.map.Remove(eventHandler);
                }

                return wasFound;
            }
        }

        public bool Contains(object eventHandler)
        {
            return this.map.ContainsKey(eventHandler);
        }
    }
}
