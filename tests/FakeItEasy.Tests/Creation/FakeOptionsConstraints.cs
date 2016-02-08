namespace FakeItEasy.Tests.Creation
{
    using System.Collections.Generic;
    using System.Linq;
    using FakeItEasy.Creation;

    public static class FakeOptionsConstraints
    {
        internal static IProxyOptions HasArgumentsForConstructor(this IArgumentConstraintManager<IProxyOptions> scope, IEnumerable<object> argumentsForConstructor)
        {
            return scope.Matches(x => argumentsForConstructor.SequenceEqual(x.ArgumentsForConstructor), "Constructor arguments ({0})".FormatInvariant(string.Join(", ", argumentsForConstructor.Select(x => x.ToString()).ToArray())));
        }
    }
}
