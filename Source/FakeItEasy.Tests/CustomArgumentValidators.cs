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
        public static ArgumentValidator<T> IsThisSequence<T>(this ArgumentValidatorScope<T> validations, T collection) where T : IEnumerable
        {
            return ArgumentValidator.Create(validations, x => x.Cast<object>().SequenceEqual(collection.Cast<object>()), "Same sequence");
        }

        public static ArgumentValidator<T> IsThisSequence<T>(this ArgumentValidatorScope<T> validations, params object[] collection) where T : IEnumerable
        {
            return ArgumentValidator.Create(validations, x => x.Cast<object>().SequenceEqual(collection.Cast<object>()), "Same sequence");
        }

        
        public static ArgumentValidator<Expression> ProducesValue(this ArgumentValidatorScope<Expression> validations, object expectedValue)
        {
            return ArgumentValidator.Create(validations, x => object.Equals(expectedValue, ExpressionManager.GetValueProducedByExpression(x)), 
			                                string.Format(CultureInfo.InvariantCulture, "Expression that produces the value {0}", expectedValue));
        }

        public static ArgumentValidator<FakeObject> Fakes(this ArgumentValidatorScope<FakeObject> validations, object fakedObject)
        {
            return ArgumentValidator.Create(validations, x => x.Equals(Fake.GetFakeObject(fakedObject)), "Specified FakeObject");
        }
    }
}
