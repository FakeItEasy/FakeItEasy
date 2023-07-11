namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class ArgumentEqualityComparer
    {
        private readonly IEnumerable<IArgumentEqualityComparer> argumentEqualityComparers;

        public ArgumentEqualityComparer(IEnumerable<IArgumentEqualityComparer> argumentEqualityComparers)
        {
            this.argumentEqualityComparers = argumentEqualityComparers;
        }

        public bool AreEqual(object? expectedValue, object? argumentValue, Type parameterType)
        {
            var comparer = this.argumentEqualityComparers
                .Where(c => c.CanCompare(parameterType))
                .OrderByDescending(c => c.Priority)
                .FirstOrDefault();

            if (comparer is null)
            {
                return object.Equals(expectedValue, argumentValue);
            }

            return comparer.AreEqual(expectedValue, argumentValue);
        }
    }
}
