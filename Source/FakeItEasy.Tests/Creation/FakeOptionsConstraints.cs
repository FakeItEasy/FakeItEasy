namespace FakeItEasy.Tests.Creation
{
    using System.Collections.Generic;
    using System.Linq;
    using FakeItEasy.Creation;
    using FakeItEasy.SelfInitializedFakes;

    public static class FakeOptionsConstraints
    {
        internal static FakeOptions HasRecorder(this IArgumentConstraintManager<FakeOptions> scope, ISelfInitializingFakeRecorder recorder)
        {
            return scope.Matches(x => recorder.Equals(x.Wrapper.Recorder), "Specified recorder");
        }

        internal static FakeOptions HasArgumentsForConstructor(this IArgumentConstraintManager<FakeOptions> scope, IEnumerable<object> argumentsForConstructor)
        {
            return scope.Matches(x => argumentsForConstructor.SequenceEqual(x.ArgumentsForConstructor), "Constructor arguments ({0})".FormatInvariant(string.Join(", ", argumentsForConstructor.Select(x => x.ToString()).ToArray())));
        }

        internal static FakeOptions Wraps(this IArgumentConstraintManager<FakeOptions> scope, object wrappedInstance)
        {
            return scope.Matches(x => object.ReferenceEquals(x.Wrapper.WrappedObject, wrappedInstance), "Wraps {0}".FormatInvariant(wrappedInstance));
        }
    }
}
