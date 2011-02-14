namespace FakeItEasy.Tests.Creation
{
    using System.Collections.Generic;
    using System.Linq;
    using FakeItEasy.Creation;
    using FakeItEasy.Expressions;
    using FakeItEasy.SelfInitializedFakes;

    public static class FakeOptionsConstraints
    {
        internal static ArgumentConstraint<FakeOptions> HasRecorder(this ArgumentConstraintScope<FakeOptions> scope, ISelfInitializingFakeRecorder recorder)
        {
            return ArgumentConstraint.Create(scope, x => recorder.Equals(x.SelfInitializedFakeRecorder), "Specified recorder");
        }

        internal static ArgumentConstraint<FakeOptions> HasArgumentsForConstructor(this ArgumentConstraintScope<FakeOptions> scope, IEnumerable<object> argumentsForConstructor)
        {
            return ArgumentConstraint.Create(scope, x => argumentsForConstructor.SequenceEqual(x.ArgumentsForConstructor), "Constructor arguments ({0})".FormatInvariant(string.Join(", ", argumentsForConstructor.Select(x => x.ToString()).ToArray())));
        }

        internal static ArgumentConstraint<FakeOptions> Wraps(this ArgumentConstraintScope<FakeOptions> scope, object wrappedInstance)
        {
            return ArgumentConstraint.Create(scope, x => object.ReferenceEquals(x.WrappedInstance, wrappedInstance), "Wraps {0}".FormatInvariant(wrappedInstance));
        }
    }
}
