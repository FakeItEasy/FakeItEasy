namespace FakeItEasy.Tests
{
    using System.Collections;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using FakeItEasy.Core;
    using FakeItEasy.Expressions;
using FakeItEasy.Core.Creation;

    public static class CustomArgumentConstraints
    {
        public static ArgumentConstraint<T> IsThisSequence<T>(this ArgumentConstraintScope<T> scope, T collection) where T : IEnumerable
        {
            return ArgumentConstraint.Create(scope, x => x.Cast<object>().SequenceEqual(collection.Cast<object>()), "Same sequence");
        }

        public static ArgumentConstraint<T> IsThisSequence<T>(this ArgumentConstraintScope<T> scope, params object[] collection) where T : IEnumerable
        {
            return ArgumentConstraint.Create(scope, x => x != null && x.Cast<object>().SequenceEqual(collection.Cast<object>()), "Same sequence");
        }

        
        public static ArgumentConstraint<Expression> ProducesValue(this ArgumentConstraintScope<Expression> scope, object expectedValue)
        {
            return ArgumentConstraint.Create(scope, x => object.Equals(expectedValue, Helpers.GetValueProducedByExpression(x)), 
			                                string.Format(CultureInfo.InvariantCulture, "Expression that produces the value {0}", expectedValue));
        }

        public static ArgumentConstraint<FakeManager> Fakes(this ArgumentConstraintScope<FakeManager> scope, object fakedObject)
        {
            return ArgumentConstraint.Create(scope, x => x.Equals(Fake.GetFakeManager(fakedObject)), "Specified FakeObject");
        }

        internal static ArgumentConstraint<FakeOptions> IsEmpty(this ArgumentConstraintScope<FakeOptions> scope)
        {
            return ArgumentConstraint.Create(scope,
                x => 
                {
                    return x.AdditionalInterfacesToImplement == null
                        && x.ArgumentsForConstructor == null
                        && x.SelfInitializedFakeRecorder == null
                        && x.WrappedInstance == null;
                }, "Empty fake options");
        }
    }
}
