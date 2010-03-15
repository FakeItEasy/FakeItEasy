namespace FakeItEasy.Tests
{
    using System.Collections;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using FakeItEasy.Core;
    using FakeItEasy.Expressions;

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
            return ArgumentConstraint.Create(scope, x => object.Equals(expectedValue, ExpressionManager.GetValueProducedByExpression(x)), 
			                                string.Format(CultureInfo.InvariantCulture, "Expression that produces the value {0}", expectedValue));
        }

        public static ArgumentConstraint<FakeObject> Fakes(this ArgumentConstraintScope<FakeObject> scope, object fakedObject)
        {
            return ArgumentConstraint.Create(scope, x => x.Equals(Fake.GetFakeObject(fakedObject)), "Specified FakeObject");
        }
    }
}
