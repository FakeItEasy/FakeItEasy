namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    internal class ArgumentEqualityComparer
    {
        private readonly IEnumerable<IArgumentEqualityComparer> argumentEqualityComparers;
        private readonly ConcurrentDictionary<Type, IArgumentEqualityComparer?> cachedComparers = new();

        public ArgumentEqualityComparer(IEnumerable<IArgumentEqualityComparer> argumentEqualityComparers)
        {
            this.argumentEqualityComparers = argumentEqualityComparers.OrderByDescending(c => c.Priority).ToArray();
        }

        public bool AreEqual(object? expectedValue, object? argumentValue, Type parameterType)
        {
            var comparer = this.cachedComparers.GetOrAdd(parameterType, t => this.argumentEqualityComparers.FirstOrDefault(c => c.CanCompare(t)));

            if (comparer is null)
            {
                return object.Equals(expectedValue, argumentValue);
            }

            try
            {
                return comparer.AreEqual(expectedValue, argumentValue);
            }
            catch (Exception exception)
            {
                throw new UserCallbackException(ExceptionMessages.UserCallbackThrewAnException("Argument Equality Comparer"), exception);
            }
        }
    }
}
