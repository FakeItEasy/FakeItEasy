namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    internal class EventHandlerArgumentProviders : ConcurrentDictionary<Delegate, IArgumentProvider>
    {
        public EventHandlerArgumentProviders()
            : base(new Comparer())
        {            
        }

        /// <summary>
        /// Allows a more lenient comparison of delegates, chiefly so <see cref="EventHandler"/>s and
        /// <see cref="EventHandler{TEventArgs}"/>s that refer to the same method on the same instance
        /// will compare as equal. It relies on the fact that every time an event is raised, 
        /// <see cref="Raise"/> creates a new instance, and the delegate registered in the map
        /// targets that instance.
        /// </summary>
        private class Comparer : IEqualityComparer<Delegate>
        {
            public bool Equals(Delegate x, Delegate y)
            {
                return ReferenceEquals(x, y) ||
                    (x != null && y != null && ReferenceEquals(x.Target, y.Target));
            }

            public int GetHashCode(Delegate obj)
            {
                return obj == null || obj.Target == null
                    ? 17
                    : obj.Target.GetHashCode();
            }
        }
    }
}
