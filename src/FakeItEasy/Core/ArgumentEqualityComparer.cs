namespace FakeItEasy.Core;

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy.Expressions.ArgumentConstraints;

internal class ArgumentEqualityComparer
{
    private readonly IEnumerable<IArgumentEqualityComparer> argumentEqualityComparers;
    private readonly ConcurrentDictionary<Type, IArgumentEqualityComparer?> cachedComparers = new();

    public ArgumentEqualityComparer(IEnumerable<IArgumentEqualityComparer> argumentEqualityComparers)
    {
        this.argumentEqualityComparers = argumentEqualityComparers
            .OrderByDescending(c => c.Priority)
            .Concat(new EnumerableComparer(new ArgumentConstraintEqualityComparer()))
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
        private readonly IEqualityComparer<object?> elementComparer;

        public EnumerableComparer(IEqualityComparer<object?> elementComparer)
        {
            this.elementComparer = elementComparer;
        }

        public Priority Priority => Priority.Internal;

        public bool CanCompare(Type type) => typeof(IEnumerable).IsAssignableFrom(type);

        public bool AreEqual(object expectedValue, object argumentValue)
        {
            if (argumentValue is not IEnumerable argumentEnumerable)
            {
                return false;
            }

            if (expectedValue is string expectedString && argumentValue is string argumentString)
            {
                return string.Equals(expectedString, argumentString, StringComparison.Ordinal);
            }

            IEnumerable expectedEnumerable = (IEnumerable)expectedValue;
            return expectedEnumerable.Cast<object?>().SequenceEqual(
                argumentEnumerable.Cast<object?>(),
                this.elementComparer);
        }
    }

    private class ArgumentConstraintEqualityComparer : IEqualityComparer<object?>
    {
        public new bool Equals(object? x, object? y)
            => EqualityArgumentConstraint.FromExpectedValue(x).IsValid(y);

        public int GetHashCode(object? obj) => obj is null ? 0 : obj.GetHashCode();
    }
}
