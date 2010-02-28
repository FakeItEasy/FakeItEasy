using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using FakeItEasy.Api;
using System.Diagnostics;
using FakeItEasy.Api;
using System.Linq.Expressions;
using FakeItEasy.Expressions;
using System.Collections;
using System.Globalization;

namespace FakeItEasy.Tests
{
    public static class CustomArgumentValidators
    {
        public static ArgumentConstraint<T> IsThisSequence<T>(this ArgumentConstraintScope<T> validations, T collection) where T : IEnumerable
        {
            return ArgumentConstraint.Create(validations, x => x.Cast<object>().SequenceEqual(collection.Cast<object>()), "Same sequence");
        }

        public static ArgumentConstraint<T> IsThisSequence<T>(this ArgumentConstraintScope<T> validations, params object[] collection) where T : IEnumerable
        {
            return ArgumentConstraint.Create(validations, x => x.Cast<object>().SequenceEqual(collection.Cast<object>()), "Same sequence");
        }

        
        public static ArgumentConstraint<Expression> ProducesValue(this ArgumentConstraintScope<Expression> validations, object expectedValue)
        {
            return ArgumentConstraint.Create(validations, x => object.Equals(expectedValue, ExpressionManager.GetValueProducedByExpression(x)), 
			                                string.Format(CultureInfo.InvariantCulture, "Expression that produces the value {0}", expectedValue));
        }

        public static ArgumentConstraint<FakeObject> Fakes(this ArgumentConstraintScope<FakeObject> validations, object fakedObject)
        {
            return ArgumentConstraint.Create(validations, x => x.Equals(Fake.GetFakeObject(fakedObject)), "Specified FakeObject");
        }
    }
}
