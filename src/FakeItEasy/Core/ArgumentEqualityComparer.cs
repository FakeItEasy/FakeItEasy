namespace FakeItEasy.Core
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    internal class ArgumentEqualityComparer
    {
        private readonly IEnumerable<IArgumentEqualityComparer> argumentEqualityComparers;
        private readonly ConcurrentDictionary<Type, IArgumentEqualityComparer?> cachedComparers = new();

        public ArgumentEqualityComparer(IEnumerable<IArgumentEqualityComparer> argumentEqualityComparers)
        {
            this.argumentEqualityComparers = argumentEqualityComparers
                .OrderByDescending(c => c.Priority)
                .Concat(new EnumerableComparer())
                .ToArray();
        }

        public bool AreEqual(object expectedValue, object argumentValue)
        {
            var comparer = this.cachedComparers.GetOrAdd(expectedValue.GetType(), this.FindHighestPriorityComparer);

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

        private IArgumentEqualityComparer? FindHighestPriorityComparer(Type type)
            => this.argumentEqualityComparers.FirstOrDefault(c => c.CanCompare(type));

        private class EnumerableComparer : IArgumentEqualityComparer
        {
            public Priority Priority => Priority.Internal;

            public bool CanCompare(Type type) => typeof(IEnumerable).IsAssignableFrom(type);

            public bool AreEqual(object expectedValue, object argumentValue)
            {
                if (expectedValue is string expectedString && argumentValue is string argumentString)
                {
                    return string.Equals(expectedString, argumentString, StringComparison.Ordinal);
                }

                return argumentValue is IEnumerable argumentEnumerable &&
                    Enumerable.SequenceEqual(
                        ((IEnumerable)expectedValue).Cast<object?>(),
                        argumentEnumerable.Cast<object?>());
            }
        }
    }
}
